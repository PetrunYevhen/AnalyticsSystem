import { useState, useEffect } from "react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import {
    CreditCard,
    Search,
    ArrowDownLeft,
    ArrowUpRight,
    Download,
    DollarSign,
    Wallet
} from "lucide-react"

export default function Transactions() {
    const [transactions, setTransactions] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const mockTransactions = [
            { id: "TX-9021", customer: "user_771", amount: 1200.50, method: "Visa", status: "Success", date: "2026-04-06T14:30:00" },
            { id: "TX-9022", customer: "user_882", amount: 450.00, method: "Apple Pay", status: "Pending", date: "2026-04-06T15:10:00" },
            { id: "TX-9023", customer: "user_112", amount: 3200.00, method: "Mastercard", status: "Success", date: "2026-04-05T09:20:00" },
            { id: "TX-9024", customer: "user_451", amount: 150.00, method: "Google Pay", status: "Failed", date: "2026-04-05T18:45:00" },
        ]
        setTransactions(mockTransactions)
        setLoading(false)
    }, [])

    const getStatusStyle = (status) => {
        switch (status) {
            case "Success": return "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 border-green-200"
            case "Pending": return "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 border-blue-200"
            case "Failed": return "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400 border-red-200"
            default: return ""
        }
    }

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
                            ₴ 142,500.00
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
                            ₴ 12,400.00
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
                            ₴ 1,200.00
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
                            ) : (
                                transactions.map((tx) => (
                                    <TableRow key={tx.id} className="hover:bg-muted/50">
                                        <TableCell className="font-mono text-xs">{tx.id}</TableCell>
                                        <TableCell className="text-sm font-medium text-muted-foreground">{tx.customer}</TableCell>
                                        <TableCell>
                                            <div className="flex items-center gap-2">
                                                <CreditCard className="h-4 w-4 text-muted-foreground" />
                                                <span className="text-sm">{tx.method}</span>
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-sm text-muted-foreground">
                                            {new Date(tx.date).toLocaleString('uk-UA')}
                                        </TableCell>
                                        <TableCell>
                                            <Badge variant="outline" className={getStatusStyle(tx.status)}>
                                                {tx.status}
                                            </Badge>
                                        </TableCell>
                                        <TableCell className="text-right font-bold">
                                            ₴ {tx.amount.toLocaleString()}
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