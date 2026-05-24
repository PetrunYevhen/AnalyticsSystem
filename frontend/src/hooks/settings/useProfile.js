import { useState, useCallback } from "react"
import { apiGet, apiPost, apiPut } from "@/lib/api-client"

export function useProfile() {
    const [profile, setProfile] = useState({ fullName: "", email: "", role: "" })
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [isSaving, setIsSaving] = useState(false)

    const fetchProfile = useCallback(async () => {
        setLoading(true)
        try {
            const data = await apiGet(`/users/profile`)
            setProfile(data || { fullName: "", email: "", role: "" })
        } catch (err) {
            setError(err.message || "Помилка завантаження профілю")
        } finally {
            setLoading(false)
        }
    }, [])

    const changePassword = async ({ currentPassword, newPassword }) => {
        setIsSaving(true)
        try {
            await apiPut(`/users/change-password`, { currentPassword, newPassword })
            return { success: true }
        } catch (err) {
            return { success: false, error: err.message || "Помилка зміни пароля" }
        } finally {
            setIsSaving(false)
        }
    }

    return { profile, loading, error, isSaving, fetchProfile, changePassword }
}