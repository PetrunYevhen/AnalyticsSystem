import { useState } from "react"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Button } from "@/components/ui/button"
import { Megaphone, Plus } from "lucide-react"
import { StatusBadge } from "@/components/StatusBadge"
import { AddCampaignModal } from "./modals/AddCampaignModal"
import { CampaignDetailsModal } from "@/pages/marketing/components/modals/CampaignDetailsModal"
import { useCampaigns } from "@/hooks/marketings/useCampaigns"

const dateFormatter = new Intl.DateTimeFormat('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric'
})
const formatDate = (dateStr) => dateStr ? dateFormatter.format(new Date(dateStr)) : "—"

const currencyFormatter = new Intl.NumberFormat('uk-UA', {
    style: 'currency', currency: 'UAH', minimumFractionDigits: 0
})

export function CampaignsSection({ onAddUsers }) {
    const [isAddModalOpen, setIsAddModalOpen]     = useState(false)
    const [selectedCampaign, setSelectedCampaign] = useState(null)

    const {
        campaigns, loading, refetch,
        updateCampaign, isUpdating, updateError,
        addSpend, isAddingSpend, addSpendError,
    } = useCampaigns()
    return (
        <Card className="shadow-sm">
            <CardHeader className="flex flex-row items-center justify-between pb-4">
                <CardTitle className="flex items-center gap-2 text-lg">
                    <Megaphone className="h-5 w-5" /> Рекламні кампанії
                </CardTitle>
                <Button onClick={() => setIsAddModalOpen(true)} size="sm">
                    <Plus className="h-4 w-4 mr-2" /> Додати кампанію
                </Button>
            </CardHeader>

            {loading ? (
                <CardContent className="text-sm text-muted-foreground pb-6">
                    Завантаження...
                </CardContent>
            ) : (!Array.isArray(campaigns) || campaigns.length === 0) ? (
                <CardContent className="text-sm text-muted-foreground pb-6">
                    Немає даних про активні кампанії за обраний період.
                </CardContent>
            ) : (
                <CardContent className="p-0">
                    <Table>
                        <TableHeader>
                            <TableRow className="bg-muted/50">
                                <TableHead className="w-[25%]">Назва</TableHead>
                                <TableHead className="w-[15%]">Канал</TableHead>
                                <TableHead className="w-[12%]">Статус</TableHead>
                                <TableHead className="w-[15%]">Початок</TableHead>
                                <TableHead className="w-[15%]">Кінець</TableHead>
                                <TableHead className="w-[10%] text-right">Бюджет</TableHead>
                                <TableHead className="w-[8%] text-right">Клієнти</TableHead>
                                <TableHead className="w-[10%]" />
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {campaigns.map((c) => (
                                <TableRow
                                    key={c.id}
                                    className="hover:bg-muted/30 transition-colors cursor-pointer"
                                    onClick={() => setSelectedCampaign(c)}
                                >
                                    <TableCell className="font-medium truncate max-w-0">
                                        <span className="block truncate">{c.name || "—"}</span>
                                    </TableCell>
                                    <TableCell>{c.channel || "—"}</TableCell>
                                    <TableCell>
                                        <StatusBadge status={c.status} type="campaign" />
                                    </TableCell>
                                    <TableCell>{formatDate(c.activePeriodStart)}</TableCell>
                                    <TableCell>{formatDate(c.activePeriodEnd)}</TableCell>
                                    <TableCell className="text-right tabular-nums">
                                        {currencyFormatter.format(c.budget ?? 0)}
                                    </TableCell>
                                    <TableCell className="text-right tabular-nums">
                                        {c.customersCount ?? 0}
                                    </TableCell>
                                    <TableCell>
                                        <Button
                                            variant="ghost"
                                            size="sm"
                                            onClick={(e) => { e.stopPropagation(); onAddUsers?.(c) }}
                                        >
                                            <Plus className="h-4 w-4 mr-1" /> Клієнти
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            )}

            <AddCampaignModal
                isOpen={isAddModalOpen}
                onClose={() => setIsAddModalOpen(false)}
                onSuccess={refetch}
            />

            <CampaignDetailsModal
                campaign={selectedCampaign}
                isOpen={!!selectedCampaign}
                onClose={() => setSelectedCampaign(null)}
                onSuccess={() => { setSelectedCampaign(null); refetch() }}
                updateCampaign={updateCampaign}
                isPending={isUpdating}
                submitError={updateError}
                addSpend={addSpend}
                isAddingSpend={isAddingSpend}
                addSpendError={addSpendError}
            />
        </Card>
    )
}