import { useState } from "react"
import { Megaphone, BarChart2, Plus } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { useAddExpense } from "@/hooks/marketings/useAddExpense"

const AD_SOURCES = ["Facebook", "Google", "LinkedIn", "TikTok", "Organic", "Other"]

const initialForm = Object.freeze({
    adSource: "Facebook",
    expenseDate: new Date().toISOString().split('T')[0],
    amount: "", currency: "USD", campaignId: "",
    impressions: "", clicks: "", leads: ""
})

export function AddExpenseModal({ onSuccess }) {
    const [open, setOpen] = useState(false)
    const [form, setForm] = useState(initialForm)
    const { loading, error, setError, submitExpense } = useAddExpense(() => {
        setOpen(false)
        if (onSuccess) onSuccess()
    })

    const handleChange = (f, v) => setForm(p => ({ ...p, [f]: v }))

    const validate = () => {
        if (!form.adSource || !form.expenseDate) return "Джерело та дата обов'язкові."
        if (!form.amount || Number(form.amount) <= 0) return "Сума витрат має бути більшою за 0."
        if ([form.impressions, form.clicks, form.leads].some(v => v !== "" && Number(v) < 0)) return "Метрики не можуть бути від'ємними."
        return null
    }

    const handleSubmit = (e) => {
        e.preventDefault()
        const err = validate()
        if (err) return setError(err)

        const parseNum = (v) => v === "" ? null : Number(v)
        submitExpense({
            ...form,
            amount: Number(form.amount),
            campaignId: form.campaignId.trim() || null,
            impressions: parseNum(form.impressions),
            clicks: parseNum(form.clicks),
            leads: parseNum(form.leads)
        }, () => setForm(initialForm))
    }

    return (
        <Dialog open={open} onOpenChange={(val) => { setOpen(val); if (!val) { setForm(initialForm); setError(null); } }}>
            <DialogTrigger asChild>
                <Button><Plus className="h-4 w-4 mr-2" /> Додати витрати</Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[600px] p-0">
                <form onSubmit={handleSubmit}>
                    <DialogHeader className="p-6 pb-4 border-b">
                        <DialogTitle>Фіксація витрат</DialogTitle>
                    </DialogHeader>

                    <div className="p-6 space-y-6">
                        <section className="space-y-4">
                            <h3 className="flex items-center gap-2 text-sm font-semibold uppercase text-muted-foreground"><Megaphone className="h-4 w-4" /> Джерело та бюджет</h3>
                            <div className="grid grid-cols-2 gap-4">
                                <select className="flex h-10 w-full rounded-md border px-3" value={form.adSource} onChange={e => handleChange('adSource', e.target.value)} disabled={loading}>
                                    {AD_SOURCES.map(s => <option key={s} value={s}>{s} Ads</option>)}
                                </select>
                                <Input type="date" value={form.expenseDate} onChange={e => handleChange('expenseDate', e.target.value)} disabled={loading} />
                                <Input type="number" step="0.01" min="0.01" placeholder="Сума *" value={form.amount} onChange={e => handleChange('amount', e.target.value)} disabled={loading} />
                                <select className="flex h-10 w-full rounded-md border px-3" value={form.currency} onChange={e => handleChange('currency', e.target.value)} disabled={loading}>
                                    <option value="USD">USD</option><option value="EUR">EUR</option><option value="UAH">UAH</option>
                                </select>
                                <Input className="col-span-2" placeholder="Campaign ID (необов'язково)" value={form.campaignId} onChange={e => handleChange('campaignId', e.target.value)} disabled={loading} />
                            </div>
                        </section>

                        <section className="space-y-4">
                            <h3 className="flex items-center gap-2 text-sm font-semibold uppercase text-muted-foreground"><BarChart2 className="h-4 w-4" /> Метрики з кабінету</h3>
                            <div className="grid grid-cols-3 gap-4">
                                <Input type="number" min="0" placeholder="Покази" value={form.impressions} onChange={e => handleChange('impressions', e.target.value)} disabled={loading} />
                                <Input type="number" min="0" placeholder="Кліки" value={form.clicks} onChange={e => handleChange('clicks', e.target.value)} disabled={loading} />
                                <Input type="number" min="0" placeholder="Ліди" value={form.leads} onChange={e => handleChange('leads', e.target.value)} disabled={loading} />
                            </div>
                        </section>

                        {error && <Alert variant="destructive"><AlertDescription>{error}</AlertDescription></Alert>}
                    </div>

                    <div className="flex justify-end gap-2 p-4 border-t bg-muted/20">
                        <Button type="button" variant="ghost" onClick={() => setOpen(false)} disabled={loading}>Скасувати</Button>
                        <Button type="submit" disabled={loading}>{loading ? "Збереження..." : "Зберегти витрати"}</Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    )
}