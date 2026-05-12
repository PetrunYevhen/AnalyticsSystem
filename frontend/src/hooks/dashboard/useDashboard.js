import { useState, useCallback, useEffect, useRef } from "react";
import { apiGet } from "@/lib/api-client";

const toExclusiveEndUtc = (date) => {
    const next = new Date(date);
    next.setUTCHours(0, 0, 0, 0);
    next.setUTCDate(next.getUTCDate() + 1);
    return next.toISOString();
};

const toStartUtc = (date) => {
    const d = new Date(date);
    d.setUTCHours(0, 0, 0, 0);
    return d.toISOString();
};

export function useDashboard(range) {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const abortRef = useRef(null);

    const fetchDashboard = useCallback(async () => {
        if (!range?.from || !range?.to) return;

        abortRef.current?.abort();
        abortRef.current = new AbortController();

        setLoading(true);
        setError(null);

        try {
            const result = await apiGet("/dashboard", {
                from: toStartUtc(range.from),
                to:   toExclusiveEndUtc(range.to),
                granularity: "Daily",
            }, abortRef.current.signal);

            setData(result);
        } catch (err) {
            if (err.name === "AbortError") return;
            setError("Не вдалося завантажити дані дашборду.");
        } finally {
            setLoading(false);
        }
    }, [range]);

    useEffect(() => {
        fetchDashboard();
        return () => abortRef.current?.abort();
    }, [fetchDashboard]);

    return { data, loading, error, refetch: fetchDashboard };
}