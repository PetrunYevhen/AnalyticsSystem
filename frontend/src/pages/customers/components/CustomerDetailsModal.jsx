import { useEffect } from "react"
import { useCustomerDetails } from "@/hooks/customers/useCustomerDetails"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { User, Mail, Phone, CalendarDays, ShoppingBag, TrendingUp, Link as LinkIcon } from "lucide-react"
import { StatusBadge } from "@/components/StatusBadge"

const moneyFormatter = new Intl.NumberFormat('uk-UA', { style: 'currency', currency: 'UAH', minimumFractionDigits: 2 })
const dateFormatter = new Intl.DateTimeFormat('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
})
const formatDate = (dateStr) => dateStr ? dateFormatter.format(new Date(dateStr)) : "—"

export function CustomerDetailsModal({ customer, onClose }) {
    const isOpen = !!customer
    const customerId = customer?.id || customer?.customerId

    const { customerInfo, loading, error, fetchCustomer } = useCustomerDetails(customerId)

    useEffect(() => {
        if (customerId) fetchCustomer()
    }, [customerId, fetchCustomer])

    if (!isOpen) return null

    const data = customerInfo || customer || {}
    const recentOrders = data.recentOrders || []

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="sm:max-w-[85vw] md:max-w-4xl max-h-[90vh] p-0 flex flex-col overflow-hidden bg-zinc-950 border-zinc-800 text-zinc-100">
                <DialogHeader className="p-6 pb-4 border-b border-zinc-800 shrink-0">
                    <DialogTitle className="text-2xl font-bold flex items-center gap-3">
                        <User className="h-6 w-6 text-zinc-500" />
                        {data.fullName || "Без імені"}
                        {data.status && <StatusBadge status={data.status} type="customer" />}
                    </DialogTitle>
                    <DialogDescription className="text-zinc-400 mt-1 flex items-center gap-4">
                        <span className="flex items-center gap-1"><Mail className="h-3 w-3" /> {data.email || "—"}</span>
                        <span className="flex items-center gap-1"><Phone className="h-3 w-3" /> {data.phoneNumber || "—"}</span>
                    </DialogDescription>
                </DialogHeader>

                <div className="flex-1 min-h-0 overflow-y-auto">
                    {loading ? (
                        <div className="p-10 text-center text-zinc-500">Завантаження профілю...</div>
                    ) : error ? (
                        <div className="p-10 text-center text-red-400">{error}</div>
                    ) : (
                        <div className="p-6 space-y-6">

                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <Card className="bg-zinc-900 border-zinc-800">
                                    <CardHeader className="pb-2">
                                        <CardTitle className="text-sm text-zinc-400 flex items-center gap-2">
                                            <TrendingUp className="h-4 w-4" /> Цінність клієнта (LTV)
                                        </CardTitle>
                                    </CardHeader>
                                    <CardContent className="flex items-end gap-8">
                                        <div>
                                            <p className="text-xs text-zinc-500 mb-1">Принесений дохід</p>
                                            <p className="text-3xl font-mono text-emerald-400">
                                                {moneyFormatter.format(data.predictedLtv || 0)}
                                            </p>
                                        </div>
                                        <div>
                                            <p className="text-xs text-zinc-500 mb-1">К-сть замовлень</p>
                                            <p className="text-3xl font-mono">{data.orderCount || 0}</p>
                                        </div>
                                    </CardContent>
                                </Card>

                                <Card className="bg-zinc-900 border-zinc-800">
                                    <CardHeader className="pb-2">
                                        <CardTitle className="text-sm text-zinc-400 flex items-center gap-2">
                                            <LinkIcon className="h-4 w-4" /> Джерело та активність
                                        </CardTitle>
                                    </CardHeader>
                                    <CardContent className="grid grid-cols-2 gap-4 text-sm">
                                        <div>
                                            <p className="text-xs text-zinc-500 mb-1">Канал залучення</p>
                                            <p className="font-medium text-zinc-200">{data.acquisitionChannel || "Не вказано"}</p>
                                            {data.campaignName && (
                                                <p className="text-xs text-zinc-400 mt-0.5">Кампанія: {data.campaignName}</p>
                                            )}
                                        </div>
                                        <div>
                                            <p className="text-xs text-zinc-500 mb-1">Зовнішній ID</p>
                                            <p className="font-mono text-xs text-zinc-400">{"—"}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs text-zinc-500 mb-1">Дата реєстрації</p>
                                            <p className="text-zinc-300">{formatDate(data.registrationDate)}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs text-zinc-500 mb-1">Останнє замовлення</p>
                                            <p className="text-zinc-300">{formatDate(data.lastOrderDate)}</p>
                                        </div>
                                    </CardContent>
                                </Card>
                            </div>

                            <Card className="bg-zinc-900 border-zinc-800">
                                <CardHeader>
                                    <CardTitle className="flex items-center gap-2 text-lg">
                                        <ShoppingBag className="h-5 w-5" /> Останні замовлення
                                    </CardTitle>
                                </CardHeader>
                                <CardContent className="p-0">
                                    <Table>
                                        <TableHeader className="bg-zinc-950/50">
                                            <TableRow className="border-zinc-800 hover:bg-transparent">
                                                <TableHead className="text-zinc-400">ID / Зовн. ID</TableHead>
                                                <TableHead className="text-zinc-400">Дата</TableHead>
                                                <TableHead className="text-zinc-400">Статус</TableHead>
                                                <TableHead className="text-zinc-400 text-right">Сума</TableHead>
                                            </TableRow>
                                        </TableHeader>
                                        <TableBody>
                                            {recentOrders.length === 0 ? (
                                                <TableRow className="border-zinc-800 hover:bg-transparent">
                                                    <TableCell colSpan={4} className="text-center text-zinc-500 py-6">
                                                        Замовлень не знайдено
                                                    </TableCell>
                                                </TableRow>
                                            ) : (
                                                recentOrders.map(order => (
                                                    <TableRow key={order.orderId} className="border-zinc-800 hover:bg-zinc-800/50 transition-none">
                                                        <TableCell>
                                                            <div className="font-mono text-xs text-zinc-400 truncate max-w-[150px]">
                                                                {order.orderId}
                                                            </div>
                                                            {order.externalOrderId && (
                                                                <div className="text-xs text-zinc-500">#{order.externalOrderId}</div>
                                                            )}
                                                        </TableCell>
                                                        <TableCell className="whitespace-nowrap">{formatDate(order.orderDate || order.createdAt)}</TableCell>
                                                        <TableCell>
                                                            <StatusBadge status={order.status} />
                                                        </TableCell>
                                                        <TableCell className="text-right font-mono text-zinc-200">
                                                            {moneyFormatter.format(order.totalAmount || order.amount || 0)}
                                                        </TableCell>
                                                    </TableRow>
                                                ))
                                            )}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                            </Card>

                        </div>
                    )}
                </div>
            </DialogContent>
        </Dialog>
    )
}