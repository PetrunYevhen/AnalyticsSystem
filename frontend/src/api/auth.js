import { apiPost, getToken } from "@/lib/api-client"

export { getToken }

export function saveAuthSession({ jwtToken, apiKey, tenantId }) {
    if (jwtToken) localStorage.setItem("token", jwtToken)
    if (apiKey)   localStorage.setItem("apiKey", apiKey)
    if (tenantId) localStorage.setItem("tenantId", tenantId)
}

export function clearAuthSession() {
    localStorage.removeItem("token")
    localStorage.removeItem("apiKey")
    localStorage.removeItem("tenantId")
}

export function isAuthenticated() {
    return Boolean(getToken())
}

export function registerTenant(payload) {
    return apiPost("/auth/register", payload)
}

export function loginUser(payload) {
    return apiPost("/auth/login", payload)
}

export function validatePassword(password) {
    if (password.length < 8)       return "Пароль має бути не менше 8 символів"
    if (!/[A-Z]/.test(password))   return "Пароль має містити хоча б одну велику літеру"
    if (!/[0-9]/.test(password))   return "Пароль має містити хоча б одну цифру"
    return null
}

export function validateEmail(email) {
    const ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
    return ok ? null : "Некоректний email"
}