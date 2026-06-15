import { useState, useEffect, useCallback, useRef } from "react"
import { apiGet } from "@/lib/api-client"
import { defaultDirectionFor } from "@/lib/sort"

export function useOrders(pageSize = 50) {
    const [data, setData] = useState({ items: [], stats: {}, totalCount: 0, totalPages: 1, hasNext: false, hasPrev: false })
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    const [page, setPage] = useState(1)
    const [sort, setSort] = useState({ sortBy: "OrderDate", direction: "Desc" })
    const [search, setSearch] = useState("")

    const abortRef = useRef(null)

    const fetchOrders = useCallback(async () => {
        abortRef.current?.abort()
        abortRef.current = new AbortController()

        setLoading(true)
        try {
            const res = await apiGet("/orders", {
                page, pageSize,
                sortBy: sort.sortBy,
                direction: sort.direction,
                search: search || undefined,
            }, abortRef.current.signal)
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
            if (e.name === 'AbortError') return
            setError("Не вдалося завантажити замовлення")
            setData(prev => ({ ...prev, items: [] }))
        } finally {
            setLoading(false)
        }
    }, [page, sort.sortBy, sort.direction, search, pageSize])

    useEffect(() => { fetchOrders() }, [fetchOrders])

    const handleSort = useCallback((key) => {
        setSort(prev => ({
            sortBy: key,
            direction: prev.sortBy === key
                ? (prev.direction === "Asc" ? "Desc" : "Asc")
                : defaultDirectionFor(key),
        }))
        setPage(1)
    }, [])

    return {
        ...data, loading, error,
        page, setPage,
        sortBy: sort.sortBy,
        direction: sort.direction,
        handleSort,
        search, setSearch,
        refetch: fetchOrders
    }
}
