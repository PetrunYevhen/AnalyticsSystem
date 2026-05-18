import { useState, useRef, useEffect } from "react"
import { Megaphone, BarChart2 } from "lucide-react"
import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { apiPost } from "@/lib/api-client"

const AD_SOURCES = [
    { value: "Facebook", label: "Facebook Ads" },
    { value: "Google", label: "Google Ads" },
    { value: "LinkedIn", label: "LinkedIn Ads" },
    { value: "TikTok", label: "TikTok Ads" },
    { value: "Organic", label: "Organic / SEO" },
    { value: "Other", label: "Інше" }
]

const SELECT_CLS =
    "flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm " +
    "focus:outline-none focus:ring-2 focus:ring-ring disabled:opacity-50"

const getTodayDateOnly = () => {
    const d = new Date()
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

const initialForm = Object.freeze({
    adSource: "Facebook",
    expenseDate: getTodayDateOnly(),
    amount: "",
    currency: "USD",
    campaignId: "",
    impressions: "",
    clicks: "",
    leads: ""
})

export function CreateExpenseForm({ onSuccess }) {
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)
    const [form, setForm] = useState(initialForm)

    const abortRef = useRef(null)
    const idempotencyRef = useRef(crypto.randomUUID())

    useEffect(() => () => abortRef.current?.abort(), [])

    const handleChange = (field, value) => {
        setForm(prev => ({ ...prev, [field]: value }))
    }

    const resetForm = () => {
        setForm(initialForm)
        idempotencyRef.current = crypto.randomUUID()
    }

    const validate = () => {
        const errs = []
        if (!form.adSource) errs.push("Джерело реклами обов'язкове.")
        if (!form.expenseDate) errs.push("Дата обов'язкова.")

        const amount = Number(form.amount)
        if (!form.amount || isNaN(amount) || amount <= 0) {
            errs.push("Сума витрат має бути більшою за 0.")
        }

        const checkMetric = (val, name) => {
            if (val !== "" && (isNaN(Number(val)) || Number(val) < 0)) {
                errs.push(`${name} не може бути від'ємним.`)
            }
        }

        checkMetric(form.impressions, "Покази")
        checkMetric(form.clicks, "Кліки")
        checkMetric(form.leads, "Ліди")

        return errs.length ? errs.join('\n') : null
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        if (loading) return
        setError(null)

        const validationError = validate()
        if (validationError) {
            setError(validationError)
            return
        }

        const parseOptNum = (val) => val === "" ? null : Number(val)

        const payload = {
            adSource: form.adSource,
            expenseDate: form.expenseDate,
            amount: Number(form.amount),
            currency: form.currency,
            campaignId: form.campaignId.trim() || null,
            impressions: parseOptNum(form.impressions),
            clicks: parseOptNum(form.clicks),
            leads: parseOptNum(form.leads)
        }

        abortRef.current?.abort()
        abortRef.current = new AbortController()
        setLoading(true)

        try {
            await apiPost('/marketing', payload, {
                signal: abortRef.current.signal,
                headers: { 'Idempotency-Key': idempotencyRef.current }
            })

            resetForm()
            onSuccess?.()
        } catch (err) {
            if (err.name === 'AbortError') return
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    return (
        <Card className="w-full h-full border-none shadow-none">
            <form onSubmit={handleSubmit} noValidate>
                <CardHeader className="px-8 pt-8">
                    <CardTitle className="text-2xl">Фіксація витрат</CardTitle>
                </CardHeader>

                <CardContent className="space-y-8 px-8">
                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground border-b pb-2">
                            <Megaphone className="h-4 w-4" /> Джерело та бюджет
                        </h3>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <select className={SELECT_CLS} value={form.adSource} onChange={e => handleChange('adSource', e.target.value)} disabled={loading} required>
                                {AD_SOURCES.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
                            </select>
                            <Input type="date" value={form.expenseDate} onChange={e => handleChange('expenseDate', e.target.value)} required disabled={loading} />
                            <Input type="number" step="0.01" min="0.01" placeholder="Сума витрат *" value={form.amount} onChange={e => handleChange('amount', e.target.value)} required disabled={loading} />
                            <select className={SELECT_CLS} value={form.currency} onChange={e => handleChange('currency', e.target.value)} disabled={loading}>
                                <option value="USD">USD</option>
                                <option value="EUR">EUR</option>
                                <option value="UAH">UAH</option>
                            </select>
                            <Input className="md:col-span-2" placeholder="Campaign ID (необов'язково)" value={form.campaignId} onChange={e => handleChange('campaignId', e.target.value)} disabled={loading} />
                        </div>
                    </section>

                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground border-b pb-2">
                            <BarChart2 className="h-4 w-4" /> Метрики з кабінету <span className="text-xs font-normal lowercase">(необов'язково)</span>
                        </h3>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                            <Input type="number" min="0" step="1" placeholder="Покази (Impressions)" value={form.impressions} onChange={e => handleChange('impressions', e.target.value)} disabled={loading} />
                            <Input type="number" min="0" step="1" placeholder="Кліки (Clicks)" value={form.clicks} onChange={e => handleChange('clicks', e.target.value)} disabled={loading} />
                            <Input type="number" min="0" step="1" placeholder="Ліди (Leads)" value={form.leads} onChange={e => handleChange('leads', e.target.value)} disabled={loading} />
                        </div>
                    </section>

                    {error && <Alert variant="destructive"><AlertDescription className="whitespace-pre-line">{error}</AlertDescription></Alert>}
                </CardContent>

                <CardFooter className="flex justify-end gap-2 border-t mt-4 pt-4 px-8 pb-8">
                    <Button type="button" variant="ghost" onClick={resetForm} disabled={loading}>Скинути</Button>
                    <Button type="submit" disabled={loading}>{loading ? "Збереження..." : "Зберегти витрати"}</Button>
                </CardFooter>
            </form>
        </Card>
    )
}