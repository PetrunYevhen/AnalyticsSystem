import { useState, useEffect } from "react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Package, Search, Clock, CheckCircle2 } from "lucide-react"
import {StatusBadge} from "@/components/StatusBadge";

export default function Orders() {
    // 1. Розділяємо стан: окремо статистика, окремо список замовлень
    const [orders, setOrders] = useState([])
    const [stats, setStats] = useState({ totalOrders: 0, successfulOrders: 0, processingOrders: 0 })
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const response = await fetch("http://localhost:5044/orders")
                const data = await response.json()

                // 2. Зберігаємо дані в правильні стейти
                if (data) {
                    setStats(data.orderStats || { totalOrders: 0, successfulOrders: 0, processingOrders: 0 })
                    setOrders(data.orderItems || [])
                }
            } catch (error) {
                console.error("Помилка завантаження замовлень:", error)
                setOrders([])
            } finally {
                setLoading(false)
            }
        }
        fetchOrders()
    }, [])


    return (
        <div className="p-6 md:p-10 space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight text-foreground">Замовлення</h2>
                    <p className="text-muted-foreground">Керування та аналітика транзакцій вашого тенанта.</p>
                </div>
            </div>

            {/* 4. Виводимо готову статистику з об'єкта stats */}
            <div className="grid gap-4 md:grid-cols-3">
                <Card>
                    <CardContent className="pt-6 flex items-center gap-4">
                        <div className="p-2 bg-blue-100 dark:bg-blue-900/20 rounded-lg">
                            <Package className="h-6 w-6 text-blue-600" />
                        </div>
                        <div>
                            <p className="text-sm font-medium text-muted-foreground">Всього замовлень</p>
                            <h3 className="text-2xl font-bold">{stats.totalOrders}</h3>
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
                            <h3 className="text-2xl font-bold">{stats.successfulOrders}</h3>
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
                            <h3 className="text-2xl font-bold">{stats.processingOrders}</h3>
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
                                <TableHead>Користувач</TableHead>
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
                                    <TableRow key={order.orderNumber} className="cursor-pointer hover:bg-muted/50 transition-colors">
                                        {/* 5. Використовуємо правильні назви полів з вашого JSON */}
                                        <TableCell className="font-mono text-xs font-medium">
                                            #{order.orderNumber}
                                        </TableCell>
                                        <TableCell className="text-sm text-muted-foreground truncate max-w-[150px]">
                                            {order.customerName}
                                        </TableCell>
                                        <TableCell className="text-sm whitespace-nowrap">
                                            {new Date(order.orderDate).toLocaleDateString('uk-UA', {
                                                day: '2-digit', month: '2-digit', year: 'numeric',
                                                hour: '2-digit', minute: '2-digit'
                                            })}
                                        </TableCell>
                                        <TableCell>
                                            <StatusBadge status={order.status} />
                                        </TableCell>
                                        <TableCell className="text-right font-bold text-primary">
                                            ₴ {order.totalAmount?.toLocaleString('uk-UA', { minimumFractionDigits: 2 }) || "0.00"}
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