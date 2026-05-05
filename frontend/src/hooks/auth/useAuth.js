import { getToken } from "@/api/auth"

function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split(".")[1]))
    } catch {
        return null
    }
}

export function useAuth() {
    const token = getToken()
    const payload = token ? parseJwt(token) : null

    return {
        isAuthenticated: Boolean(payload),
        role: payload?.role ?? null,
        userId: payload?.userId ?? null,
        tenantId: payload?.tenantId ?? null,
        isAdmin: payload?.role === "Admin",
        isAnalyst: payload?.role === "Analyst",
        isViewer: payload?.role === "Viewer",
    }
}