import { useState, useCallback } from "react"
import { apiGet, apiPost } from "@/lib/api-client"

export function useOrderPayment(orderId) {
    const [paymentInfo, setPaymentInfo] = useState(null)
    const [transactions, setTransactions] = useState([])
    const [loading, setLoading] = useState(false)
    const [mutating, setMutating] = useState(false)
    const [error, setError] = useState(null)

    const fetchPayment = useCallback(async () => {
        if (!orderId) return
        setLoading(true)
        setError(null)
        try {
            const data = await apiGet(`/transactions/${orderId}/payment`)
            setPaymentInfo(data)
            setTransactions(data?.transactions ?? [])
        } catch(err) {
            if (err.status === 404) {
                setPaymentInfo(null)
                setTransactions([])
            } else {
                setError("Помилка завантаження платежу.")
            }
        } finally {
            setLoading(false)
        }
    }, [orderId])

    const recordPayment = async (payload) => {
        setMutating(true)
        setError(null)
        try {
            await apiPost(`/transactions/${orderId}/payment`, {
                amount: Number(payload.amount),
                method: payload.method,
                note: payload.note || null
            })
            await fetchPayment()
            return true
        } catch (err) {
            setError(err.message || "Помилка запису оплати.")
            return false
        } finally {
            setMutating(false)
        }
    }

    const cancelOrder = async (note = "") => {
        setMutating(true)
        setError(null)
        try {
            await apiPost(`orders/${orderId}/cancel`, {
                note:  null
            })
            return true
        } catch (err) {
            setError(err.message || "Помилка скасування замовлення.")
            return false
        } finally {
            setMutating(false)
        }
    }

    const processRefund = async (payload) => {
        setMutating(true)
        setError(null)
        try {
            await apiPost(`/transactions/${orderId}/payment/refund`, {
                amount: Number(payload.amount),
                method: payload.method,
                note: payload.note || null })
            await fetchPayment()
            return true
        } catch (err) {
            setError(err.message || "Помилка повернення коштів.")
            return false
        } finally {
            setMutating(false)
        }
    }

    return {
        paymentInfo,
        transactions,
        loading,
        mutating,
        error,
        fetchPayment,
        recordPayment,
        cancelOrder,
        processRefund
    }
}