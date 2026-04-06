import { useState, useEffect } from "react"
import { Card, CardHeader, CardTitle, CardContent, CardDescription } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Progress } from '@/components/ui/progress'
import { Badge } from "@/components/ui/badge"
import { Megaphone, TrendingUp, MousePointer2, Target, Globe } from "lucide-react"

export default function Marketing() {
    const [marketingData, setMarketingData] = useState([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const mockData = [
            { id: 1, source: "Google Ads", clicks: 1240, spend: 500, revenue: 1200, roas: "2.4x" },
            { id: 2, source: "Facebook", clicks: 850, spend: 300, revenue: 950, roas: "3.1x" },
            { id: 3, source: "Instagram", clicks: 2100, spend: 450, revenue: 800, roas: "1.7x" },
            { id: 4, source: "Email", clicks: 150, spend: 20, revenue: 400, roas: "20x" },
        ]
        setMarketingData(mockData)
        setLoading(false)
    }, [])

    return (
        <div className="p-6 md:p-10 space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Маркетингова аналітика</h2>
                    <p className="text-muted-foreground">Ефективність рекламних кампаній та джерел трафіку.</p>
                </div>
            </div>

            <div className="grid gap-6 md:grid-cols-4">
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium">ROMI (Окупність)</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold text-green-600">+245%</div>
                        <TrendingUp className="h-4 w-4 text-green-500 mt-1" />
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium">CTR (Клікабельність)</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">3.2%</div>
                        <p className="text-xs text-muted-foreground mt-1">Вище середнього на 0.4%</p>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium">Ціна за клік (CPC)</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">₴ 4.12</div>
                        <p className="text-xs text-muted-foreground mt-1">Знизилась на 12% за тиждень</p>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium">Конверсія</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">4.8%</div>
                        <Progress value={4.8 * 10} className="h-1.5 mt-3" />
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
                                {marketingData.map((item) => (
                                    <TableRow key={item.id}>
                                        <TableCell className="font-medium">{item.source}</TableCell>
                                        <TableCell>{item.clicks}</TableCell>
                                        <TableCell>₴{item.spend}</TableCell>
                                        <TableCell>₴{item.revenue}</TableCell>
                                        <TableCell className="text-right">
                                            <Badge variant="secondary" className="bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">
                                                {item.roas}
                                            </Badge>
                                        </TableCell>
                                    </TableRow>
                                ))}
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
                                <span>Перегляди (10k)</span>
                                <span className="font-bold">100%</span>
                            </div>
                            <Progress value={100} className="h-2 bg-muted" />
                        </div>
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span>Кліки (1.2k)</span>
                                <span className="font-bold text-blue-500">12%</span>
                            </div>
                            <Progress value={12} className="h-2 bg-muted shadow-sm" />
                        </div>
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span>Кошик (240)</span>
                                <span className="font-bold text-orange-500">2.4%</span>
                            </div>
                            <Progress value={2.4 * 5} className="h-2 bg-muted" />
                        </div>
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span>Оплачено (48)</span>
                                <span className="font-bold text-green-500">0.5%</span>
                            </div>
                            <Progress value={0.5 * 10} className="h-2 bg-muted" />
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}