import { Repeat, UserMinus, Activity, Users } from "lucide-react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { StatCard } from "@/components/StatCard"
import {fmtPct, fmtNum, retentionColor, churnColor} from "@/lib/marketing-utils"

function NewVsReturningBar({ newCount, returningCount }) {
    const total = (newCount ?? 0) + (returningCount ?? 0)
    if (total === 0) return <div className="text-sm text-muted-foreground">Немає даних</div>

    const newPct = (newCount / total) * 100
    const retPct = 100 - newPct

    return (
        <div className="space-y-3">
            <div className="flex h-2 w-full overflow-hidden rounded-full bg-muted">
                <div className="bg-primary" style={{ width: `${newPct}%` }} />
                <div className="bg-emerald-500" style={{ width: `${retPct}%` }} />
            </div>

            <div className="flex flex-col gap-2 text-xs w-full mt-2">

                <div className="flex items-center justify-between whitespace-nowrap">
                    <div className="flex items-center gap-1.5">
                        <span className="h-2 w-2 rounded-full bg-primary shrink-0" />
                        <span className="text-muted-foreground">Нові:</span>
                    </div>
                    <span className="font-medium truncate">
            {fmtNum(newCount)} <span className="opacity-70">({newPct.toFixed(0)}%)</span>
        </span>
                </div>

                <div className="flex items-center justify-between whitespace-nowrap">
                    <div className="flex items-center gap-1.5">
                        <span className="h-2 w-2 rounded-full bg-emerald-500 shrink-0" />
                        <span className="text-muted-foreground">Постійні:</span>
                    </div>
                    <span className="font-medium truncate">
            {fmtNum(returningCount)} <span className="opacity-70">({retPct.toFixed(0)}%)</span>
        </span>
                </div>

            </div>
        </div>
    )
}

export function RetentionSection({ stats }) {
    const safeStats = stats || {}

    return (
        <div>
            <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-3">Утримання та активність</h3>
            <div className="grid gap-4 md:grid-cols-4">
                <StatCard icon={Repeat} title="Retention Rate" value={fmtPct(safeStats.retentionRate)} hint="Клієнти що повернулись" valueClassName={retentionColor(safeStats.retentionRate)} trend={safeStats.retentionTrend} />
                <StatCard icon={UserMinus} title="Churn Rate" value={fmtPct(safeStats.churnRate)} hint="% відтоку клієнтів" valueClassName={churnColor(safeStats.churnRate)} />
                <StatCard icon={Activity} title="Purchase Frequency" value={(safeStats.purchaseFrequency ?? 0).toFixed(2)} hint="Покупок на клієнта" />
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium flex items-center gap-2 text-muted-foreground">
                            <Users className="h-4 w-4" /> Нові vs Постійні
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <NewVsReturningBar newCount={safeStats.newCustomers} returningCount={safeStats.returningCustomers} />
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}