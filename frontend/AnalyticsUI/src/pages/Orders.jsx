import { useState, useEffect } from "react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Package, Search, ArrowUpRight, Clock, CheckCircle2, XCircle } from "lucide-react"

export default function Orders() {
    const [orders, setOrders] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const response = await fetch("http://localhost:5044/orders?tenantId=366a4940-217f-44af-ba15-acf7852496e7")
                const data = await response.json()
                setOrders(data)
            } catch (error) {
                console.error("Помилка завантаження замовлень:", error)
            } finally {
                setLoading(false)
            }
        }
        fetchOrders()
    }, [])

    const getStatusBadge = (status) => {
        switch (status?.toLowerCase()) {
            case "completed":
            case "paid":
                return <Badge className="bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 border-green-200">Виконано</Badge>
            case "pending":
                return <Badge className="bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400 border-yellow-200">Очікує</Badge>
            case "cancelled":
                return <Badge className="bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400 border-red-200">Скасовано</Badge>
            default:
                return <Badge variant="outline">{status}</Badge>
        }
    }

    return (
        <div className="p-6 md:p-10 space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight text-foreground">Замовлення</h2>
                    <p className="text-muted-foreground">Керування та аналітика транзакцій вашого тенанта.</p>
                </div>
            </div>

            <div className="grid gap-4 md:grid-cols-3">
                <Card>
                    <CardContent className="pt-6 flex items-center gap-4">
                        <div className="p-2 bg-blue-100 dark:bg-blue-900/20 rounded-lg">
                            <Package className="h-6 w-6 text-blue-600" />
                        </div>
                        <div>
                            <p className="text-sm font-medium text-muted-foreground">Всього замовлень</p>
                            <h3 className="text-2xl font-bold">{orders.length}</h3>
                        </div>
                    </CardContent>
                </Card>
                <Card>
                    <CardContent className="pt-6 flex items-center gap-4">
                        <div className="p-2 bg-green-100 dark:bg-green-900/20 rounded-lg">
                            <CheckCircle2 className="h-6 w-6 text-green-600" />
                        </div>
                        <div>
                            <p className="text-sm font-medium text-muted-foreground">Успішні</p>
                            <h3 className="text-2xl font-bold">
                                {orders.filter(o => o.status === 'Paid' || o.status === 'Completed').length}
                            </h3>
                        </div>
                    </CardContent>
                </Card>
                <Card>
                    <CardContent className="pt-6 flex items-center gap-4">
                        <div className="p-2 bg-yellow-100 dark:bg-yellow-900/20 rounded-lg">
                            <Clock className="h-6 w-6 text-yellow-600" />
                        </div>
                        <div>
                            <p className="text-sm font-medium text-muted-foreground">В обробці</p>
                            <h3 className="text-2xl font-bold">
                                {orders.filter(o => o.status === 'Pending').length}
                            </h3>
                        </div>
                    </CardContent>
                </Card>
            </div>

            <Card className="dark:bg-card/50">
                <CardHeader className="flex flex-row items-center justify-between">
                    <CardTitle>Останні операції</CardTitle>
                    <div className="relative w-64">
                        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input placeholder="Пошук за ID..." className="pl-9 h-9" />
                    </div>
                </CardHeader>
                <CardContent className="p-0">
                    <Table>
                        <TableHeader>
                            <TableRow className="hover:bg-transparent">
                                <TableHead>Номер замовлення</TableHead>
                                <TableHead>Клієнт (ID)</TableHead>
                                <TableHead>Дата</TableHead>
                                <TableHead>Статус</TableHead>
                                <TableHead className="text-right">Сума</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {loading ? (
                                <TableRow><TableCell colSpan={5} className="text-center py-10">Завантаження...</TableCell></TableRow>
                            ) : orders.length === 0 ? (
                                <TableRow><TableCell colSpan={5} className="text-center py-10 text-muted-foreground">Замовлень поки немає</TableCell></TableRow>
                            ) : (
                                orders.map((order) => (
                                    <TableRow key={order.id} className="cursor-pointer hover:bg-muted/50 transition-colors">
                                        <TableCell className="font-mono text-xs font-medium">
                                            #{order.externalId || order.id.slice(0, 8)}
                                        </TableCell>
                                        <TableCell className="text-sm text-muted-foreground">
                                            {order.customerId || "Гість"}
                                        </TableCell>
                                        <TableCell className="text-sm">
                                            {new Date(order.createdAt).toLocaleDateString('uk-UA')}
                                        </TableCell>
                                        <TableCell>
                                            {getStatusBadge(order.status)}
                                        </TableCell>
                                        <TableCell className="text-right font-bold">
                                            ₴ {order.amount?.toLocaleString() || "0.00"}
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    )
}