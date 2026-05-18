import { useState, useMemo, useEffect, useCallback } from "react"
import { apiGet, apiPost, apiDelete } from "@/lib/api-client"

export function useCampaignCustomers(campaign, isOpen, onSuccess) {
    const [customers, setCustomers] = useState([])
    const [search, setSearch] = useState("")
    const [selectedIds, setSelectedIds] = useState(new Set())
    const [fetching, setFetching] = useState(false)
    const [loading, setLoading] = useState(false)
    const [removingId, setRemovingId] = useState(null)
    const [error, setError] = useState(null)

    const loadCustomers = useCallback(async () => {
        if (!campaign?.id) return
        setFetching(true)
        try {
            const [availableRes, assignedRes] = await Promise.all([
                apiGet(`/marketing/campaigns/available-customers`),
                apiGet(`/marketing/campaigns/${campaign.id}/customers`)
            ])
            const availableList = availableRes?.items ?? availableRes ?? []
            const assignedList  = assignedRes?.items  ?? assignedRes  ?? []
            const unified = [
                ...availableList.map(c => ({ ...c, isAssigned: false })),
                ...assignedList.map(c  => ({ ...c, isAssigned: true  })),
            ]
            setCustomers(unified)
        } catch {
            setError("Не вдалося завантажити списки клієнтів.")
        } finally {
            setFetching(false)
        }
    }, [campaign?.id])

    useEffect(() => {
        if (!isOpen || !campaign?.id) {
            setSearch("")
            setSelectedIds(new Set())
            setError(null)
            return
        }
        loadCustomers()
    }, [isOpen, campaign?.id, loadCustomers])

    const { available, assigned, totalAssignedCount } = useMemo(() => {
        const q = search.trim().toLowerCase()
        const filtered = q
            ? customers.filter(c =>
                c.fullName?.toLowerCase().includes(q) ||
                c.email?.toLowerCase().includes(q))
            : customers

        return {
            available: filtered.filter(c => !c.isAssigned),
            assigned: filtered.filter(c =>  c.isAssigned),
            totalAssignedCount: customers.filter(c => c.isAssigned).length,
        }
    }, [customers, search])

    const handleAssign = async () => {
        if (selectedIds.size === 0) return
        setLoading(true)
        setError(null)
        try {
            await apiPost(`/marketing/campaigns/${campaign.id}/customers`, {
                customerIds: Array.from(selectedIds)
            })
            setSelectedIds(new Set())
            await loadCustomers()
            onSuccess?.()
        } catch (err) {
            setError(err.message || "Помилка при призначенні клієнтів.")
        } finally {
            setLoading(false)
        }
    }

    const handleRemove = async (customerId) => {
        setRemovingId(customerId)
        setError(null)
        try {
            await apiDelete(`/marketing/campaigns/${campaign.id}/customers`, { customerIds: [customerId] })
            await loadCustomers()
            onSuccess?.()
        } catch (err) {
            setError(err.message || "Не вдалося видалити клієнта.")
        } finally {
            setRemovingId(null)
        }
    }

    return {
        search, setSearch, selectedIds, setSelectedIds,
        fetching, loading, removingId, error, setError,
        available, assigned, totalAssignedCount,
        handleAssign, handleRemove
    }
}