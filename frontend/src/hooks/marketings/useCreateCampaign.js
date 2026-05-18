import { useState } from "react"
import { apiPost } from "@/lib/api-client"

export function useCreateCampaign() {
    const [isPending, setIsPending] = useState(false)
    const [error, setError] = useState(null)

    const createCampaign = async (payload) => {
        setIsPending(true)
        setError(null)

        try {
            const result = await apiPost("/marketing/campaigns", payload)
            return result
        } catch (err) {
            setError(err.message)
            throw err
        } finally {
            setIsPending(false)
        }
    }

    return { createCampaign, isPending, error }
}