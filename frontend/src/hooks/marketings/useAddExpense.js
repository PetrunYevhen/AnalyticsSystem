import { useState, useRef, useEffect } from "react"
import { apiPost } from "@/lib/api-client"

export function useAddExpense(onSuccess) {
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    const abortRef = useRef(null)
    const idempotencyRef = useRef(crypto.randomUUID())

    useEffect(() => () => abortRef.current?.abort(), [])

    const submitExpense = async (payload, onResetForm) => {
        if (loading) return

        abortRef.current?.abort()
        abortRef.current = new AbortController()
        setLoading(true)
        setError(null)

        try {
            await apiPost('/marketing', payload, {
                signal: abortRef.current.signal,
                headers: { 'Idempotency-Key': idempotencyRef.current }
            })
            onResetForm()
            idempotencyRef.current = crypto.randomUUID()
            if (onSuccess) onSuccess()
        } catch (err) {
            if (err.name !== 'AbortError') setError(err.message || "Помилка збереження.")
        } finally {
            setLoading(false)
        }
    }

    return { loading, error, setError, submitExpense }
}