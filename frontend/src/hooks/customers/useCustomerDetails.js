import { useState, useCallback } from "react"
import { apiGet } from "@/lib/api-client"

export function useCustomerDetails(customerId) {
    const [customerInfo, setCustomerInfo] = useState(null)
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    const fetchCustomer = useCallback(async () => {
        if (!customerId) return
        setLoading(true)
        setError(null)
        try {
            const data = await apiGet(`/customers/${customerId}`)
            setCustomerInfo(data)
        } catch (err) {
            setError(err.message || "Не вдалося завантажити деталі клієнта")
        } finally {
            setLoading(false)
        }
    }, [customerId])

    return { customerInfo, loading, error, fetchCustomer }
}