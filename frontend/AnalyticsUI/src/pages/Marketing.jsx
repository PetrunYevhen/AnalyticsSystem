import { useState, useEffect } from "react"
import { Card, CardHeader, CardTitle, CardContent, CardDescription } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Progress } from '@/components/ui/progress'
import { Badge } from "@/components/ui/badge"
import { Megaphone, TrendingUp, Target, Globe, Wallet, UserPlus } from "lucide-react"

export default function Marketing() {
    const [marketingData, setMarketingData] = useState([])
    const [loading, setLoading] = useState(true)

    // 1. Оновили назви полів відповідно до твого JSON
    const [stats, setStats] = useState({
        totalAdSpend: 0,
        cac: 0,
        romi: 0,
        conversionRate: 0
    })

    useEffect(() => {
        const fetchMarketingData = async () => {
            try {
                const response = await fetch("http://localhost:5044/marketing")
                const data = await response.json()

                if (data) {
                    // 2. Використовуємо правильні ключі з JSON
                    setStats(data.marketingStats || { totalAdSpend: 0, cac: 0, romi: 0, conversionRate: 0 })
                    setMarketingData(data.marketingListItems || [])
                }

                // ПРИБРАНО: setMarketingData([]) - воно затирало дані!
            } catch (error) {
                console.error("Помилка завантаження маркетингових даних:", error)
            } finally {
                setLoading(false)
            }
        }

        fetchMarketingData()
    }, [])

    return (
        <div className="p-6 md:p-10 space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Маркетингова аналітика</h2>
                    <p className="text-muted-foreground">Ефективність рекламних кампаній, CAC та джерела трафіку.</p>
                </div>
            </div>

            <div className="grid gap-6 md:grid-cols-4">
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium flex items-center gap-2">
                            <Wallet className="h-4 w-4 text-muted-foreground" />
                            Витрати на рекламу
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        {/* 3. Додано форматування суми з пробілами */}
                        <div className="text-2xl font-bold">₴ {stats.totalAdSpend?.toLocaleString('uk-UA', { minimumFractionDigits: 2 })}</div>
                        <p className="text-xs text-muted-foreground mt-1">За поточний місяць</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium flex items-center gap-2">
                            <UserPlus className="h-4 w-4 text-muted-foreground" />
                            CAC (Вартість клієнта)
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">₴ {stats.cac?.toFixed(2)}</div>
                        <div className="flex items-center text-xs mt-1 text-muted-foreground">
                            <span>Немає даних для порівняння</span>
                        </div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium flex items-center gap-2">
                            <Megaphone className="h-4 w-4 text-muted-foreground" />
                            ROMI (Окупність)
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        {/* 4. Обрізаємо ROMI до 2 знаків після коми */}
                        <div className="text-2xl font-bold text-foreground">
                            {stats.romi > 0 ? "+" : ""}{stats.romi?.toFixed(2)}%
                        </div>
                        <div className="flex items-center text-xs mt-1 text-muted-foreground">
                            <span>Очікування даних</span>
                        </div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium flex items-center gap-2">
                            <Target className="h-4 w-4 text-muted-foreground" />
                            Конверсія в покупку
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">
                            {stats.conversionRate ? stats.conversionRate.toFixed(2) : "0"}%
                        </div>
                        <Progress value={stats.conversionRate} className="h-1.5 mt-3" />
                    </CardContent>
                </Card>
            </div>

            <div className="grid gap-6 md:grid-cols-3">
                <Card className="md:col-span-2">
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2 text-lg">
                            <Globe className="h-5 w-5" /> Джерела трафіку
                        </CardTitle>
                    </CardHeader>
                    <CardContent className="p-0">
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Джерело</TableHead>
                                    <TableHead>Кліки</TableHead>
                                    <TableHead>Витрати</TableHead>
                                    <TableHead>Дохід</TableHead>
                                    <TableHead className="text-right">ROAS</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {loading ? (
                                    <TableRow>
                                        <TableCell colSpan={5} className="text-center py-10 text-muted-foreground">
                                            Завантаження...
                                        </TableCell>
                                    </TableRow>
                                ) : marketingData.length === 0 ? (
                                    <TableRow>
                                        <TableCell colSpan={5} className="text-center py-10 text-muted-foreground">
                                            Дані про джерела трафіку відсутні
                                        </TableCell>
                                    </TableRow>
                                ) : (
                                    // 5. Використовуємо item.source як key, оскільки id в JSON немає
                                    marketingData.map((item) => (
                                        <TableRow key={item.source}>
                                            <TableCell className="font-medium">{item.source}</TableCell>
                                            <TableCell>{item.clicks}</TableCell>
                                            <TableCell>₴{item.spend?.toLocaleString('uk-UA')}</TableCell>
                                            <TableCell>₴{item.revenue?.toLocaleString('uk-UA')}</TableCell>
                                            <TableCell className="text-right">
                                                <Badge variant="secondary" className="bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">
                                                    {item.roas?.toFixed(2)}x
                                                </Badge>
                                            </TableCell>
                                        </TableRow>
                                    ))
                                )}
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2 text-lg">
                            <Target className="h-5 w-5" /> Воронка продажів
                        </CardTitle>
                        <CardDescription>Від перегляду до покупки</CardDescription>
                    </CardHeader>
                    <CardContent className="space-y-6">
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">Перегляди</span>
                                <span className="font-bold">0%</span>
                            </div>
                            <Progress value={0} className="h-2 bg-muted" />
                        </div>
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">Кліки</span>
                                <span className="font-bold">0%</span>
                            </div>
                            <Progress value={0} className="h-2 bg-muted" />
                        </div>
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">Кошик</span>
                                <span className="font-bold">0%</span>
                            </div>
                            <Progress value={0} className="h-2 bg-muted" />
                        </div>
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">Оплачено</span>
                                <span className="font-bold">0%</span>
                            </div>
                            <Progress value={0} className="h-2 bg-muted" />
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}