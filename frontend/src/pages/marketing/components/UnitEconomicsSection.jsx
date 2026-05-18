import { Scale, TrendingUp, UserPlus, Megaphone } from "lucide-react"
import { StatCard } from "@/components/StatCard"
import { LtvCacChart } from "@/pages/marketing/components/LtvCacChart"
import {fmtUAH, ltvCacColor, ltvCacLabel, romiColor} from "@/lib/marketing-utils"

export function UnitEconomicsSection({ stats, historicalData }) {
    const safeStats = stats ?? {};
    const romi = safeStats.romi ?? 0;
    const ltvToCac = safeStats.ltvToCac ?? 0;

    return (
        <div className="space-y-6">
            <div>
                <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-3">
                    Unit-економіка
                </h3>
                <div className="grid gap-4 md:grid-cols-4">
                    <StatCard icon={Scale} title="LTV : CAC" value={`${ltvToCac.toFixed(2)}x`} hint={ltvCacLabel(ltvToCac)} valueClassName={ltvCacColor(ltvToCac)} />
                    <StatCard icon={TrendingUp} title="LTV (середній)" value={fmtUAH(safeStats.ltv)} hint="Дохід з клієнта" />
                    <StatCard icon={UserPlus} title="CAC" value={fmtUAH(safeStats.cac)} hint="Вартість залучення" invertTrendColors />
                    <StatCard icon={Megaphone} title="ROMI" value={`${romi > 0 ? "+" : ""}${romi.toFixed(2)}%`} hint="Окупність інвестицій" valueClassName={romiColor(romi)} />
                </div>
            </div>

            <LtvCacChart data={historicalData} />
        </div>
    )
}