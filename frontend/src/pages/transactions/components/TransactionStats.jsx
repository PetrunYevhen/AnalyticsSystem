import { memo } from "react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { ArrowDownLeft, ArrowUpRight, Wallet } from "lucide-react"

const moneyFormatter = new Intl.NumberFormat('uk-UA', { maximumFractionDigits: 0 })

export const TransactionStats = memo(function TransactionStats({ stats }) {
    const { availableBalance = 0, yesterdayRevenue = 0, refundedAmount = 0 } = stats || {}

    return (
        <div className="grid gap-4 md:grid-cols-3">
            <Card>
                <CardHeader className="pb-2">
                    <CardTitle className="text-sm font-medium text-muted-foreground uppercase">Доступний баланс</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-2xl font-bold flex items-center gap-2">
                        <Wallet className="h-5 w-5 text-primary" />
                        ₴ {moneyFormatter.format(availableBalance)}
                    </div>
                </CardContent>
            </Card>
            <Card>
                <CardHeader className="pb-2">
                    <CardTitle className="text-sm font-medium text-muted-foreground uppercase">Вчорашній виторг</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-2xl font-bold flex items-center gap-2 text-emerald-500">
                        <ArrowUpRight className="h-5 w-5" />
                        ₴ {moneyFormatter.format(yesterdayRevenue)}
                    </div>
                </CardContent>
            </Card>
            <Card>
                <CardHeader className="pb-2">
                    <CardTitle className="text-sm font-medium text-muted-foreground uppercase">Повернення коштів</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-2xl font-bold flex items-center gap-2 text-orange-500">
                        <ArrowDownLeft className="h-5 w-5" />
                        ₴ {moneyFormatter.format(refundedAmount)}
                    </div>
                </CardContent>
            </Card>
        </div>
    )
})