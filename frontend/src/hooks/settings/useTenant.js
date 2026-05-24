import { useState, useCallback } from "react"
import { apiGet } from "@/lib/api-client"

export function useTenant() {
    const [tenant, setTenant] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    const fetchTenant = useCallback(async () => {
        setLoading(true)
        try {
            const data = await apiGet("/tenant/info")
            setTenant(data)
        } catch (err) {
            setError(err.message || "Помилка завантаження даних організації")
        } finally {
            setLoading(false)
        }
    }, [])

    return { tenant, loading, error, fetchTenant }
}