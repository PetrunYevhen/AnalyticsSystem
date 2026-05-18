import { Wallet, Users, ShoppingCart, Target } from "lucide-react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Progress } from "@/components/ui/progress"
import { StatCard } from "@/components/StatCard"
import { fmtUAH, fmtPct } from "@/lib/marketing-utils"

export function EfficiencySection({ stats }) {
    const safeStats = stats || {}
    return (
        <div>
            <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-3">Ефективність</h3>
            <div className="grid gap-4 md:grid-cols-4">
                <StatCard icon={Wallet} title="Витрати на рекламу" value={fmtUAH(safeStats.totalAdSpend)} hint="За поточний місяць" />
                <StatCard icon={Users} title="ARPU" value={fmtUAH(safeStats.arpu)} hint="Середній дохід на юзера" />
                <StatCard icon={ShoppingCart} title="AOV" value={fmtUAH(safeStats.aov)} hint="Середній чек" />
                <Card>
                    <CardHeader className="pb-2 space-y-0">
                        <CardTitle className="text-sm font-medium flex items-center gap-2 text-muted-foreground">
                            <Target className="h-4 w-4" /> Повторні покупки
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{fmtPct(safeStats.repeatPurchaseRate)}</div>
                        <Progress value={Math.min(safeStats.repeatPurchaseRate ?? 0, 100)} className="h-1.5 mt-3" />
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}