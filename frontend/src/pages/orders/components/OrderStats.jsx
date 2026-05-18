import { memo } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Package, CheckCircle2, Clock } from "lucide-react"

const STATS_CONFIG = [
    { key: "totalOrders",      label: "Всього замовлень", icon: Package,      tone: "blue"   },
    { key: "successfulOrders", label: "Успішні",          icon: CheckCircle2, tone: "green"  },
    { key: "processingOrders", label: "В обробці",        icon: Clock,        tone: "yellow" },
]

const TONE_MAP = {
    blue:   "bg-blue-100 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400",
    green:  "bg-green-100 dark:bg-green-900/20 text-green-600 dark:text-green-400",
    yellow: "bg-yellow-100 dark:bg-yellow-900/20 text-yellow-600 dark:text-yellow-400",
}

const numberFormatter = new Intl.NumberFormat("uk-UA")

const StatCard = memo(function StatCard({ label, icon: Icon, tone, value }) {
    return (
        <Card>
            <CardContent className="p-1 flex flex-col items-center justify-center text-center gap-3">
                <div className={`p-2.5 rounded-full ${TONE_MAP[tone]}`}>
                    <Icon className="h-5 w-5" aria-hidden="true" />
                </div>
                <div className="space-y-1">
                    <h3 className="text-3xl font-bold tabular-nums tracking-tight text-foreground leading-none">
                        {numberFormatter.format(value)}
                    </h3>
                    <p className="text-sm font-medium text-muted-foreground">
                        {label}
                    </p>
                </div>
            </CardContent>
        </Card>
    )
})

export const OrderStats = memo(function OrderStats({ stats }) {
    return (
        <div className="grid gap-4 md:grid-cols-3">
            {STATS_CONFIG.map(({ key, label, icon, tone }) => (
                <StatCard
                    key={key}
                    label={label}
                    icon={icon}
                    tone={tone}
                    value={Number(stats?.[key]) || 0}
                />
            ))}
        </div>
    )
})