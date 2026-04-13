import { useState, useEffect } from "react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { StatusBadge } from "@/components/StatusBadge"
import {
    CreditCard,
    Search,
    ArrowDownLeft,
    ArrowUpRight,
    Download,
    Wallet
} from "lucide-react"

export default function Transactions() {
    const [transactions, setTransactions] = useState([])
    const [loading, setLoading] = useState(true)

    // Стейт для верхніх карток (ініціалізуємо нулями)
    const [stats, setStats] = useState({
        balance: 0,
        yesterdayRevenue: 0,
        refunds: 0
    })

    useEffect(() => {
        const fetchTransactions = async () => {
            try {
                // Заміни URL на свій реальний ендпоінт транзакцій/платежів
                const response = await fetch("http://localhost:5044/transactions")
                const data = await response.json()

                console.log("Дані з бекенду (Транзакції):", data)

                if (data) {
                    // Якщо бекенд надсилає статистику в об'єкті stats (наприклад, data.stats.balance)
                    setStats(data.stats || { balance: 0, yesterdayRevenue: 0, refunds: 0 })
                    setTransactions(data.transactions || [])
                }
            } catch (error) {
                console.error("Помилка завантаження транзакцій:", error)
                setTransactions([])
            } finally {
                setLoading(false)
            }
        }

        fetchTransactions()
    }, [])

    return (
        <div className="p-6 md:p-10 space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Транзакції</h2>
                    <p className="text-muted-foreground">Історія всіх фінансових операцій вашої системи.</p>
                </div>
                <Button variant="outline" className="gap-2">
                    <Download className="h-4 w-4" /> Експорт CSV
                </Button>
            </div>

            <div className="grid gap-4 md:grid-cols-3">
                <Card>
                    <CardHeader className="pb-2">
                        <CardTitle className="text-sm font-medium text-muted-foreground uppercase">Доступний баланс</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold flex items-center gap-2">
                            <Wallet className="h-5 w-5 text-primary" />
                            ₴ {stats.availableBalance?.toLocaleString('uk-UA') || "0"}
                        </div>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className="pb-2">
                        <CardTitle className="text-sm font-medium text-muted-foreground uppercase">Вчорашній виторг</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold flex items-center gap-2 text-green-600">
                            <ArrowUpRight className="h-5 w-5" />
                            ₴ {stats.yesterdayRevenue?.toLocaleString('uk-UA') || "0"}
                        </div>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className="pb-2">
                        <CardTitle className="text-sm font-medium text-muted-foreground uppercase">Повернення коштів</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold flex items-center gap-2 text-orange-600">
                            <ArrowDownLeft className="h-5 w-5" />
                            ₴ {stats.refundedAmount?.toLocaleString('uk-UA') || "0"}
                        </div>
                    </CardContent>
                </Card>
            </div>

            <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                    <CardTitle>Список транзакцій</CardTitle>
                    <div className="relative w-64">
                        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input placeholder="Пошук за ID..." className="pl-9 h-9" />
                    </div>
                </CardHeader>
                <CardContent className="p-0">
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>ID Транзакції</TableHead>
                                <TableHead>Користувач</TableHead>
                                <TableHead>Метод</TableHead>
                                <TableHead>Дата та час</TableHead>
                                <TableHead>Статус</TableHead>
                                <TableHead className="text-right">Сума</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {loading ? (
                                <TableRow><TableCell colSpan={6} className="text-center py-10">Завантаження...</TableCell></TableRow>
                            ) : transactions.length === 0 ? (
                                <TableRow><TableCell colSpan={6} className="text-center py-10 text-muted-foreground">Транзакцій не знайдено</TableCell></TableRow>
                            ) : (
                                transactions.map((tx) => (
                                    <TableRow key={tx.id || tx.transactionId} className="hover:bg-muted/50">
                                        <TableCell className="font-mono text-xs truncate max-w-[120px]">
                                            {tx.id || tx.transactionId || "—"}
                                        </TableCell>
                                        <TableCell className="text-sm font-medium text-muted-foreground truncate max-w-[150px]">
                                            {tx.customerName || tx.customerId || "Гість"}
                                        </TableCell>
                                        <TableCell>
                                            <div className="flex items-center gap-2">
                                                <CreditCard className="h-4 w-4 text-muted-foreground" />
                                                <span className="text-sm truncate">{tx.method || tx.paymentMethod || "Card"}</span>
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-sm text-muted-foreground whitespace-nowrap">
                                            {/* Підтримка різних назв полів дати: date або createdAt */}
                                            {tx.date || tx.createdAt
                                                ? new Date(tx.date || tx.createdAt).toLocaleString('uk-UA', {
                                                    day: '2-digit', month: '2-digit', year: 'numeric',
                                                    hour: '2-digit', minute: '2-digit'
                                                })
                                                : "—"}
                                        </TableCell>
                                        <TableCell>
                                            <StatusBadge status={tx.status} />
                                        </TableCell>
                                        <TableCell className="text-right font-bold">
                                            ₴ {tx.amount ? Number(tx.amount).toLocaleString('uk-UA', { minimumFractionDigits: 2 }) : "0.00"}
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