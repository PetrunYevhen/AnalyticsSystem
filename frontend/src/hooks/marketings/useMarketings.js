import { useState, useEffect, useCallback } from "react"
import { apiGet } from "@/lib/api-client"

const toExclusiveEndUtc = (date) => {
    if (!date) return undefined
    const next = new Date(date)
    next.setUTCHours(0, 0, 0, 0)
    next.setUTCDate(next.getUTCDate() + 1)
    return next.toISOString()
}

const toStartUtc = (date) => {
    if (!date) return undefined
    const d = new Date(date)
    d.setUTCHours(0, 0, 0, 0)
    return d.toISOString()
}

export function useMarketingDashboard(fromDate, toDate) {
    const [data, setData] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [tick, setTick] = useState(0)

    const refetch = useCallback(() => setTick(t => t + 1), [])

    useEffect(() => {
        if (!fromDate || !toDate) return

        const ctrl = new AbortController()

        const fetchData = async () => {
            setLoading(true)
            setError(null)

            const params = {
                fromDate: toStartUtc(fromDate),
                toDate:   toExclusiveEndUtc(toDate),
            }

            try {
                const [dashboardData, campaignsData] = await Promise.all([
                    apiGet('/marketing/dashboard', params, ctrl.signal),
                    apiGet('/marketing/campaigns',  params, ctrl.signal)
                ])

                setData({ ...dashboardData, campaigns: campaignsData })
            } catch (err) {
                if (err.name !== 'AbortError') setError(err.message)
            } finally {
                setLoading(false)
            }
        }

        fetchData()
        return () => ctrl.abort()
    }, [fromDate, toDate, tick])

    return { data, loading, error, refetch }
}