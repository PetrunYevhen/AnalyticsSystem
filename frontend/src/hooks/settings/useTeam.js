import { useState, useCallback } from "react"
import { apiGet, apiPost, apiDelete } from "@/lib/api-client"

export function useTeam() {
    const [members, setMembers] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    const fetchTeam = useCallback(async () => {
        setLoading(true)
        try {
            const data = await apiGet(`/users`)
            setMembers(data || [])
        } catch (err) {
            setError("Помилка завантаження команди")
            setMembers([])
        } finally {
            setLoading(false)
        }
    }, [])

    const inviteMember = async (payload) => {
        try {
            const newUser = await apiPost(`/users`, payload)
            setMembers(prev => [...prev, newUser])
            return { success: true }
        } catch (err) {
            return { success: false, error: err.message || "Не вдалося створити користувача" }
        }
    }

    const removeMember = async (id) => {
        try {
            await apiDelete(`/users/${id}`)
            setMembers(prev => prev.filter(m => m.id !== id))
            return { success: true }
        } catch (err) {
            return { success: false, error: err.message || "Не вдалося видалити" }
        }
    }

    return { members, loading, error, fetchTeam, inviteMember, removeMember }
}