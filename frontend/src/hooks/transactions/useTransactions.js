import { useState, useCallback, useEffect, useRef } from "react"
import { apiGet } from "@/lib/api-client"

export function useTransactions(pageSize = 50) {
    const [data, setData] = useState({
        items: [], totalCount: 0, totalPages: 1,
        hasNextPage: false, hasPreviousPage: false,
        stats: { availableBalance: 0, yesterdayRevenue: 0, refundedAmount: 0 }
    });
    const [initialLoad, setInitialLoad] = useState(true);
    const [isFetching, setIsFetching] = useState(false);
    const [error, setError] = useState(null);
    const [page, setPage] = useState(1);
    const [sort, setSort] = useState({ sortBy: "TransactionDate", direction: "Desc" });
    const [search, setSearch] = useState("");

    const abortRef = useRef(null);

    const fetchTransactions = useCallback(async () => {
        abortRef.current?.abort();
        abortRef.current = new AbortController();

        setIsFetching(true);
        setError(null);
        try {
            const res = await apiGet("/transactions", {
                page, pageSize,
                sortBy: sort.sortBy,
                direction: sort.direction,
                search: search || undefined,
            }, abortRef.current.signal);

            setData({
                items:           res.transactions?.items           ?? [],
                totalCount:      res.transactions?.totalCount      ?? 0,
                totalPages:      res.transactions?.totalPages      ?? 1,
                hasNextPage:     res.transactions?.hasNextPage     ?? false,
                hasPreviousPage: res.transactions?.hasPreviousPage ?? false,
                stats: res.stats ?? { availableBalance: 0, yesterdayRevenue: 0, refundedAmount: 0 }
            });
        } catch (e) {
            if (e.name === 'AbortError') return;
            setError("Не вдалося завантажити транзакції.");
            setData(prev => ({ ...prev, items: [] }));
        } finally {
            setIsFetching(false);
            setInitialLoad(false);
        }
    }, [page, pageSize, sort.sortBy, sort.direction, search]);

    useEffect(() => {
        fetchTransactions();
    }, [fetchTransactions]);

    const handleSort = useCallback((key) => {
        setSort(prev => ({
            sortBy: key,
            direction: prev.sortBy === key && prev.direction === 'Asc' ? 'Desc' : 'Asc'
        }));
        setPage(1);
    }, []);

    return {
        ...data,
        loading: initialLoad,
        isFetching,
        error,
        page, setPage,
        sortBy: sort.sortBy,
        direction: sort.direction,
        handleSort,
        search, setSearch,
    };
}