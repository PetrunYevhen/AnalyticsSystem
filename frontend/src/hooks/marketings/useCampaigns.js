import { useState, useEffect, useCallback } from "react";
import { apiGet, apiPatch, apiPost } from "@/lib/api-client";

export function useCampaigns() {
    const [campaigns, setCampaigns]     = useState([]);
    const [loading, setLoading]         = useState(true);
    const [isUpdating, setIsUpdating]   = useState(false);
    const [updateError, setUpdateError] = useState(null);
    const [isAddingSpend, setIsAddingSpend]   = useState(false);
    const [addSpendError, setAddSpendError]   = useState(null);

    const fetchCampaigns = useCallback(async () => {
        setLoading(true);
        try {
            const data = await apiGet('/marketing/campaigns');
            setCampaigns(data?.items ?? data ?? []);
        } catch {
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => { fetchCampaigns(); }, [fetchCampaigns]);

    const updateCampaign = useCallback(async (campaignId, payload) => {
        setIsUpdating(true);
        setUpdateError(null);
        try {
            await apiPatch(`/marketing/campaigns/${campaignId}`, payload);
            await fetchCampaigns();
        } catch (error) {
            const message = error?.message ?? "Не вдалося оновити кампанію";
            setUpdateError(message);
            throw error;
        } finally {
            setIsUpdating(false);
        }
    }, [fetchCampaigns]);

    const addSpend = useCallback(async (campaignId, amount, currency = "UAH") => {
        setIsAddingSpend(true);
        setAddSpendError(null);
        try {
            await apiPost(`/marketing/${campaignId}/spend`, { amount, currency });
            await fetchCampaigns();
        } catch (error) {
            const message = error?.message ?? "Не вдалося додати витрату";
            setAddSpendError(message);
            throw error;
        } finally {
            setIsAddingSpend(false);
        }
    }, [fetchCampaigns]);

    return {
        campaigns, loading, refetch: fetchCampaigns,
        updateCampaign, isUpdating, updateError,
        addSpend, isAddingSpend, addSpendError,
    };
}