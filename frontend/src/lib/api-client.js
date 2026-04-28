const API_BASE = ((import.meta.env.VITE_API_BASE_URL || window.location.origin).replace(/\/+$/, '')) + '/api/';

let onUnauthorized = null;
export function setUnauthorizedHandler(fn) { onUnauthorized = fn; }
export class ApiError extends Error {
    constructor(message, { status, problemDetails, traceId } = {}) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.problemDetails = problemDetails;
        this.traceId = traceId;
    }
    get fieldErrors() {
        return this.problemDetails?.errors ?? null;
    }
}

async function parseResponseError(res) {
    let problemDetails = null;
    let message = `HTTP ${res.status}`;

    try {
        const text = await res.text();
        if (text) {
            problemDetails = JSON.parse(text);
            if (problemDetails?.errors && typeof problemDetails.errors === 'object') {
                message = Object.entries(problemDetails.errors)
                    .map(([f, e]) => `${f}: ${Array.isArray(e) ? e.join(', ') : e}`)
                    .join('\n');
            } else {
                message = problemDetails?.detail || problemDetails?.title || message;
            }
        }
    } catch {
    }

    return new ApiError(message, {
        status: res.status,
        problemDetails,
        traceId: res.headers.get('x-trace-id') || res.headers.get('traceparent') || null,
    });
}

function buildUrl(path, params) {
    const url = new URL(path.replace(/^\/+/, ''), API_BASE);
    if (params) {
        for (const [k, v] of Object.entries(params)) {
            if (v !== undefined && v !== null && v !== '') {
                if (Array.isArray(v)) v.forEach(item => url.searchParams.append(k, String(item)));
                else url.searchParams.set(k, String(v));
            }
        }
    }
    return url.toString();
}

async function parseSuccess(res) {
    if (res.status === 204) return null;
    const ct = res.headers.get('content-type') || '';
    if (!ct.includes('application/json')) return null;
    const text = await res.text();
    return text ? JSON.parse(text) : null;
}

const DEFAULT_TIMEOUT_MS = 30_000;

async function request(method, path, { params, body, signal, headers, timeoutMs = DEFAULT_TIMEOUT_MS } = {}) {
    const url = buildUrl(path, params);
    const token = getToken();
    const hasBody = body !== undefined && body !== null;

    const timeoutCtrl = new AbortController();
    const timer = setTimeout(() => timeoutCtrl.abort(new DOMException('Timeout', 'TimeoutError')), timeoutMs);
    const composedSignal = signal
        ? AbortSignal.any([signal, timeoutCtrl.signal])
        : timeoutCtrl.signal;
    try {
        const res = await fetch(url, {
            method,
            signal: composedSignal,
            headers: {
                Accept: 'application/json',
                ...(hasBody && { 'Content-Type': 'application/json' }),
                ...(token && { Authorization: `Bearer ${token}` }),
                ...(headers || {}),
            },
            body: hasBody ? JSON.stringify(body) : undefined,
        });

        if (!res.ok) {
            const err = await parseResponseError(res);
            if (err.status === 401 && onUnauthorized) onUnauthorized(err);
            throw err;
        }
        return await parseSuccess(res);
    } catch (e) {
        if (e?.name === 'AbortError' || e?.name === 'TimeoutError') throw e;
        if (e instanceof ApiError) throw e;
        throw new ApiError(e?.message || 'Network error', { status: 0 });
    } finally {
        clearTimeout(timer);
    }
}

export const apiGet    = (path, params, signal) => request('GET',    path, { params, signal });
export const apiPost   = (path, body, opts = {}) => request('POST',   path, { body, ...opts });
export const apiPut    = (path, body, opts = {}) => request('PUT',    path, { body, ...opts });
export const apiPatch  = (path, body, opts = {}) => request('PATCH',  path, { body, ...opts });
export const apiDelete = (path, body, opts = {}) => request('DELETE', path, { body, ...opts });
export const getToken = () => localStorage.getItem('token');
