import { useState, useCallback } from "react"
import { apiGet } from "@/lib/api-client"

export function useOrderItems(orderId) {
    const [items, setItems] = useState([])
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    const fetchItems = useCallback(async () => {
        if (!orderId) return
        setLoading(true)
        setError(null)
        try {
            const data = await apiGet(`/orders/${orderId}/items`)
            setItems(data ?? [])
        } catch (err) {
            setError("Помилка завантаження товарів.")
        } finally {
            setLoading(false)
        }
    }, [orderId])

    return { items, loading, error, fetchItems }
}