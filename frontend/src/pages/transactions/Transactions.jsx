import {useTransactions} from "@/hooks/transactions/useTransactions";
import {useEffect, useState} from "react";
import {TransactionStats} from "@/pages/transactions/components/TransactionStats";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card";
import {Input} from "@/components/ui/input";
import {TransactionsTable} from "@/pages/transactions/components/TransactionsTable";
import {Button} from "@/components/ui/button";
import { Search, Download, ChevronLeft, ChevronRight } from "lucide-react"

export default function Transactions() {
    const {
        items, stats, loading, isFetching, error,
        totalPages, hasNextPage, hasPreviousPage, page, setPage,
        sortBy, direction, handleSort,
        setSearch,
    } = useTransactions()

    const [searchInput, setSearchInput] = useState("")


    useEffect(() => {
        const timer = setTimeout(() => setSearch(searchInput), 300)
        return () => clearTimeout(timer)
    }, [searchInput, setSearch])

    return (
        <div className="px-0 md:px-2 py-6 space-y-6 w-full min-w-0 overflow-x-hidden">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Транзакції</h2>
                    <p className="text-muted-foreground">Історія всіх фінансових операцій вашої системи.</p>
                </div>
                <Button variant="outline" className="gap-2">
                    <Download className="h-4 w-4" /> Експорт CSV
                </Button>
            </div>

            <TransactionStats stats={stats} />

            <Card className="flex flex-col min-h-[600px]">
                <CardHeader className="flex flex-row items-center justify-between">
                    <CardTitle>Список транзакцій</CardTitle>
                    <div className="relative w-64">
                        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input
                            placeholder="Пошук за ID..."
                            className="pl-9 h-9"
                            value={searchInput}
                            onChange={(e) => setSearchInput(e.target.value)}
                        />
                    </div>
                </CardHeader>
                <CardContent className="p-0 flex-1 overflow-auto relative">
                    {isFetching && (
                        <div className="absolute inset-0 bg-background/40 z-10 flex items-center justify-center backdrop-blur-[1px]">
                            <span className="text-sm text-muted-foreground">Оновлення...</span>
                        </div>
                    )}
                    <TransactionsTable
                        items={items}
                        loading={loading}
                        error={error}
                        sortBy={sortBy}
                        direction={direction}
                        onSort={handleSort}
                    />
                </CardContent>

                {totalPages > 1 && (
                    <div className="flex items-center justify-between px-4 py-3 border-t">
                        <span className="text-sm text-muted-foreground">
                            Сторінка {page} з {totalPages}
                        </span>
                        <div className="flex gap-2">
                            <Button
                                variant="outline" size="sm"
                                disabled={!hasPreviousPage || loading}
                                onClick={() => setPage(p => p - 1)}
                            >
                                <ChevronLeft className="h-4 w-4" />
                            </Button>
                            <Button
                                variant="outline" size="sm"
                                disabled={!hasNextPage || loading}
                                onClick={() => setPage(p => p + 1)}
                            >
                                <ChevronRight className="h-4 w-4" />
                            </Button>
                        </div>
                    </div>
                )}
            </Card>
        </div>
    )
}