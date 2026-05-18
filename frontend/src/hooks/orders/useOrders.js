import { useState, useEffect, useCallback } from "react"
import { apiGet } from "@/lib/api-client"

export function useOrders(pageSize = 50) {
    const [data, setData] = useState({ items: [], stats: {}, totalCount: 0, totalPages: 1, hasNext: false, hasPrev: false })
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    const [page, setPage] = useState(1)
    const [sortBy, setSortBy] = useState("OrderDate")
    const [direction, setDirection] = useState("Desc")
    const [search, setSearch] = useState("")

    const fetchOrders = useCallback(async () => {
        setLoading(true)
        try {
            const res = await apiGet("/orders", { page, pageSize, sortBy, direction, search: search || undefined })
            setData({
                items: res.orderItems?.items ?? [],
                stats: res.orderStats ?? { totalOrders: 0, successfulOrders: 0, processingOrders: 0 },
                totalCount: res.orderItems?.totalCount ?? 0,
                totalPages: res.orderItems?.totalPages ?? 1,
                hasNext: res.orderItems?.hasNextPage ?? false,
                hasPrev: res.orderItems?.hasPreviousPage ?? false,
            })
            setError(null)
        } catch (e) {
            setError("Не вдалося завантажити замовлення")
            setData(prev => ({ ...prev, items: [] }))
        } finally {
            setLoading(false)
        }
    }, [page, sortBy, direction, search, pageSize])

    useEffect(() => { fetchOrders() }, [fetchOrders])

    const handleSort = (key) => {
        setSortBy(key)
        setDirection(prev => sortBy === key && prev === "Asc" ? "Desc" : "Asc")
        setPage(1)
    }

    return {
        ...data, loading, error,
        page, setPage,
        sortBy, direction, handleSort,
        search, setSearch,
        refetch: fetchOrders
    }
}