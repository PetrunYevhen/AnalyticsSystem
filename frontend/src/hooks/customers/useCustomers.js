import { useState, useCallback, useEffect, useRef } from "react";
import { apiGet } from "@/lib/api-client";

export function useCustomers(pageSize = 50) {
    const [data, setData] = useState({
        items: [], totalCount: 0, totalPages: 1,
        hasNextPage: false, hasPreviousPage: false, page: 1
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const [sort, setSort] = useState({ sortBy: "LastOrderDate", direction: "Desc" });

    const abortRef = useRef(null);

    const fetchCustomers = useCallback(async () => {
        abortRef.current?.abort();
        abortRef.current = new AbortController();

        setLoading(true);
        setError(null);
        try {
            const res = await apiGet('/customers', {
                page, pageSize,
                sortBy: sort.sortBy,
                direction: sort.direction,
                search: search || undefined,
            }, abortRef.current.signal);

            setData(res ?? {
                items: [], totalCount: 0, totalPages: 1,
                hasNextPage: false, hasPreviousPage: false, page: 1
            });
        } catch (e) {
            if (e.name === 'AbortError') return;
            setError(e.message || 'Помилка завантаження клієнтів');
            setData(prev => ({ ...prev, items: [] }));
        } finally {
            setLoading(false);
        }
    }, [page, pageSize, sort.sortBy, sort.direction, search]);

    useEffect(() => {
        fetchCustomers();
    }, [fetchCustomers]);

    const handleSort = useCallback((key) => {
        setSort(prev => ({
            sortBy: key,
            direction: prev.sortBy === key && prev.direction === 'Asc' ? 'Desc' : 'Asc'
        }));
        setPage(1);
    }, []);

    return {
        ...data, loading, error,
        search, setSearch,
        page, setPage,
        sortBy: sort.sortBy,
        direction: sort.direction,
        handleSort,
    };
}