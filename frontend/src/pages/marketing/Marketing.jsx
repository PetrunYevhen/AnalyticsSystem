import { useState } from "react"
import { format, subDays } from "date-fns"
import { DateRangePicker } from "@/components/DataRangePicker"

import { useMarketingDashboard } from "@/hooks/marketings/useMarketings"
import { UnitEconomicsSection } from "./components/UnitEconomicsSection"
import { EfficiencySection } from "./components/EfficiencySection"
import { RetentionSection } from "./components/RetentionSection"
import { CampaignsSection } from "./components/CampaignsSection"

import { AddExpenseModal } from "./components/modals/AddExpenseModal"
import { AddCustomersToCampaignModal } from "./components/modals/AddCustomersToCampaignModal"

const toISO = (d) => format(d, "yyyy-MM-dd")

export default function MarketingPage() {
    const [range, setRange] = useState({
        from: subDays(new Date(), 30),
        to: new Date(),
    })

    const fromDate = range?.from ? toISO(range.from) : undefined
    const toDate = range?.to ? toISO(range.to) : undefined

    const { data, loading, error, refetch } = useMarketingDashboard(fromDate, toDate)

    const [selectedCampaign, setSelectedCampaign] = useState(null)

    if (loading && !data) return <div className="p-10 text-center text-muted-foreground animate-pulse">Завантаження аналітики...</div>
    if (error && !data) return <div className="p-10 text-center text-destructive">Помилка: {error}</div>
    if (!data) return null;

    return (
        <div className="px-0 md:px-2 py-6 space-y-6 w-full min-w-0 overflow-x-hidden">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Маркетингова аналітика</h2>
                    <p className="text-muted-foreground text-sm">
                        Період: {fromDate} — {toDate}
                    </p>
                </div>
                <div className="flex items-center gap-3 flex-wrap">
                    <DateRangePicker value={range} onChange={setRange} />
                    <AddExpenseModal onSuccess={() => refetch()} />
                </div>
            </div>

            <UnitEconomicsSection
                stats={data.unitEconomics}
                historicalData={data.ltvCacHistory}/>
            <EfficiencySection stats={data.efficiency} />
            <RetentionSection stats={data.retention} />

            <CampaignsSection
                campaigns={data.campaigns}
                onAddUsers={(campaign) => setSelectedCampaign(campaign)}
            />

            <AddCustomersToCampaignModal
                campaign={selectedCampaign}
                onClose={() => setSelectedCampaign(null)}
                onSuccess={() => refetch()}
            />
        </div>
    )
}