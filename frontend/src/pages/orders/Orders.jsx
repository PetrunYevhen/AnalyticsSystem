import { useState, useEffect } from "react"
import { useOrders } from "@/hooks/orders/useOrders"
import { OrdersTable } from "@/pages/orders/components/OrdersTable"
import { OrderDetailsModal } from "@/pages/orders/components/OrderDetailsModal"
import { OrderStats } from "@/pages/orders/components/OrderStats"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Search } from "lucide-react"

export default function OrdersPage() {
    const {
        items, stats, loading, error,
        page, setPage, totalPages, hasNext, hasPrev,
        sortBy, direction, handleSort,
        setSearch, refetch
    } = useOrders()

    const [searchInput, setSearchInput] = useState("")
    const [selectedOrder, setSelectedOrder] = useState(null)

    useEffect(() => {
        const timer = setTimeout(() => {
            setSearch(searchInput)
        }, 300)
        return () => clearTimeout(timer)
    }, [searchInput, setSearch])

    return (
        <div className="px-0 md:px-2 py-6 space-y-6 w-full min-w-0 overflow-x-hidden">
            <h2 className="text-3xl font-bold">Замовлення</h2>

            <OrderStats stats={stats} />

            <Card className="flex flex-col min-h-[600px]">
                <CardHeader className="flex flex-row justify-between">
                    <CardTitle>Останні операції</CardTitle>
                    <div className="relative w-64">
                        <Search className="absolute left-2.5 top-2.5 h-4 w-4" />
                        <Input
                            placeholder="Пошук..."
                            className="pl-9"
                            value={searchInput}
                            onChange={e => setSearchInput(e.target.value)}
                        />
                    </div>
                </CardHeader>
                <CardContent className="flex-1 overflow-auto">
                    <OrdersTable
                        items={items} loading={loading} error={error}
                        sortBy={sortBy} direction={direction}
                        onSort={handleSort} onRowClick={setSelectedOrder}
                    />
                </CardContent>
            </Card>

            <OrderDetailsModal
                order={selectedOrder}
                onClose={() => setSelectedOrder(null)}
                onSuccess={refetch}
            />
        </div>
    )
}