import { useState, useEffect } from "react"
import { Megaphone, CalendarDays, Banknote, TrendingUp, Users, Pencil } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Badge } from "@/components/ui/badge"

const STATUS_CONFIG = {
    Draft:     { label: "Чернетка",  className: "bg-secondary text-secondary-foreground" },
    Active:    { label: "Активна",   className: "bg-green-500 text-white hover:bg-green-600" },
    Paused:    { label: "На паузі",  className: "border border-amber-500 text-amber-600 bg-transparent" },
    Completed: { label: "Завершена", className: "border border-blue-500 text-blue-600 bg-transparent" },
}

const STATUSES = Object.keys(STATUS_CONFIG)

const toDateInput = (iso) => iso?.split("T")[0] ?? ""

const buildFormFromCampaign = (c) => ({
    name:    c.name,
    channel: c.channel,
    status:  c.status,
    endDate: toDateInput(c.activePeriodEnd),
    budget:  c.budget,
})

const validate = (form) => {
    if (!form.name.trim() || form.name.trim().length < 2)
        return "Назва має містити мінімум 2 символи."
    if (!form.channel.trim() || form.channel.trim().length < 2)
        return "Джерело обов'язкове."
    if (Number(form.budget) < 0)
        return "Бюджет не може бути від'ємним."
    return null
}

const checkDirty = (form, campaign) => {
    if (!form || !campaign) return false
    return (
        form.name    !== campaign.name                         ||
        form.channel !== campaign.channel                      ||
        form.status  !== campaign.status                       ||
        form.endDate !== toDateInput(campaign.activePeriodEnd) ||
        Number(form.budget) !== campaign.budget
    )
}

function InlineField({ label, value, field, type = "text", isEditing, onEdit, onBlur, onChange, disabled, placeholder = "—" }) {
    return (
        <div className="space-y-2">
            <Label htmlFor={field}>{label}</Label>
            {isEditing ? (
                <Input
                    id={field}
                    type={type}
                    step={type === "number" ? "0.01" : undefined}
                    value={value}
                    onChange={(e) => onChange(field, e.target.value)}
                    onBlur={onBlur}
                    onKeyDown={(e) => { if (e.key === "Enter") onBlur() }}
                    disabled={disabled}
                    autoFocus
                    className="h-9"
                />
            ) : (
                <button
                    type="button"
                    onClick={() => onEdit(field)}
                    disabled={disabled}
                    className="w-full h-9 px-3 flex items-center justify-between rounded-md border border-transparent hover:border-input hover:bg-muted/40 transition-all group"
                >
                    <span className={value !== "" && value !== null && value !== undefined ? "text-sm" : "text-sm text-muted-foreground"}>
                        {(value !== "" && value !== null && value !== undefined) ? value : placeholder}
                    </span>
                    <Pencil className="h-3 w-3 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity shrink-0" />
                </button>
            )}
        </div>
    )
}

function BudgetProgress({ actualSpend, budget, currency }) {
    const pct       = budget > 0 ? Math.min((actualSpend / budget) * 100, 100) : 0
    const barColor  = pct > 90 ? "bg-red-500" : pct > 70 ? "bg-amber-500" : "bg-green-500"
    const remaining = Math.max(budget - actualSpend, 0)

    return (
        <div className="space-y-2 pt-1">
            <div className="flex justify-between text-xs text-muted-foreground">
                <span>Витрачено {pct.toFixed(1)}%</span>
                <span>Залишок: {remaining.toLocaleString("uk-UA")} {currency}</span>
            </div>
            <div className="h-2.5 rounded-full bg-muted overflow-hidden">
                <div
                    className={`h-full rounded-full transition-all duration-500 ${barColor}`}
                    style={{ width: `${pct}%` }}
                />
            </div>
            <div className="flex justify-between text-xs text-muted-foreground">
                <span>₴0</span>
                <span>{Number(budget).toLocaleString("uk-UA")} {currency}</span>
            </div>
        </div>
    )
}

function SectionTitle({ icon, label }) {
    return (
        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase text-muted-foreground tracking-wider">
            {icon} {label}
        </h3>
    )
}

export function CampaignDetailsModal({
                                         isOpen, onClose, campaign, onSuccess,
                                         updateCampaign, isPending, submitError,
                                         addSpend, isAddingSpend, addSpendError,
                                     }) {
    const [form, setForm]                       = useState(null)
    const [editingField, setEditingField]       = useState(null)
    const [validationError, setValidationError] = useState(null)
    const [spendInput, setSpendInput]           = useState("")

    useEffect(() => {
        if (campaign) {
            setForm(buildFormFromCampaign(campaign))
            setEditingField(null)
            setValidationError(null)
            setSpendInput("")
        }
    }, [campaign])

    if (!campaign || !form) return null

    const isDirty   = checkDirty(form, campaign)
    const statusCfg = STATUS_CONFIG[form.status] ?? STATUS_CONFIG.Draft
    const remaining = Math.max(Number(campaign.budget) - Number(campaign.actualSpend), 0)

    const handleChange = (field, value) => {
        setForm((prev) => ({ ...prev, [field]: value }))
        if (validationError) setValidationError(null)
    }

    const handleModalClose = (open) => {
        if (!open) {
            setEditingField(null)
            setValidationError(null)
            onClose()
        }
    }

    const handleCancel = () => {
        setForm(buildFormFromCampaign(campaign))
        setEditingField(null)
        setValidationError(null)
    }

    const handleSubmit = async () => {
        const err = validate(form)
        if (err) return setValidationError(err)
        try {
            await updateCampaign(campaign.id, {
                name:    form.name.trim(),
                channel: form.channel.trim(),
                status:  form.status,
                endDate: form.endDate || null,
                budget:  Number(form.budget),
            })
            if (onSuccess) onSuccess()
            onClose()
        } catch {
        }
    }

    const handleAddSpend = async () => {
        const amount = Number(spendInput)
        if (!amount || amount <= 0) return setValidationError("Сума витрати має бути більше нуля.")
        try {
            await addSpend(campaign.id, amount, campaign.currency)
            setSpendInput("")
        } catch {
        }
    }

    return (
        <Dialog open={isOpen} onOpenChange={handleModalClose}>
            <DialogContent className="sm:max-w-[860px] p-0 overflow-hidden">

                <DialogHeader className="p-6 pb-4 border-b bg-card">
                    <div className="flex items-start justify-between gap-4">
                        <div>
                            <DialogTitle className="text-lg">{campaign.name}</DialogTitle>
                            <p className="text-sm text-muted-foreground mt-0.5">{campaign.channel}</p>
                        </div>
                        <Badge className={`${statusCfg.className} shrink-0`}>
                            {statusCfg.label}
                        </Badge>
                    </div>
                </DialogHeader>

                <div className="p-6 bg-card max-h-[70vh] overflow-y-auto">
                    <div className="grid grid-cols-2 gap-x-8 gap-y-8">

                        <div className="space-y-8">

                            <section className="space-y-4">
                                <SectionTitle icon={<Megaphone className="h-4 w-4" />} label="Основна інформація" />
                                <div className="space-y-4">
                                    <InlineField label="Назва кампанії" value={form.name} field="name"
                                                 isEditing={editingField === "name"} onEdit={setEditingField}
                                                 onBlur={() => setEditingField(null)} onChange={handleChange} disabled={isPending} />
                                    <InlineField label="Джерело (Channel)" value={form.channel} field="channel"
                                                 isEditing={editingField === "channel"} onEdit={setEditingField}
                                                 onBlur={() => setEditingField(null)} onChange={handleChange} disabled={isPending} />
                                </div>
                            </section>

                            <section className="space-y-4">
                                <SectionTitle icon={<TrendingUp className="h-4 w-4" />} label="Статус" />
                                <div className="space-y-2">
                                    <Label>Статус кампанії</Label>
                                    <div className="flex gap-2 flex-wrap">
                                        {STATUSES.map((s) => (
                                            <button key={s} type="button" onClick={() => handleChange("status", s)}
                                                    disabled={isPending}
                                                    className={`px-3 py-1.5 rounded-md text-sm font-medium border transition-all ${
                                                        form.status === s
                                                            ? "border-primary bg-primary text-primary-foreground"
                                                            : "border-border bg-background hover:bg-muted"
                                                    }`}>
                                                {STATUS_CONFIG[s].label}
                                            </button>
                                        ))}
                                    </div>
                                </div>
                            </section>

                            <section className="space-y-4">
                                <SectionTitle icon={<CalendarDays className="h-4 w-4" />} label="Дати" />
                                <div className="space-y-4">
                                    <div className="space-y-2">
                                        <Label className="text-muted-foreground">Початок (незмінний)</Label>
                                        <div className="h-9 px-3 flex items-center text-sm rounded-md border border-dashed border-border bg-muted/30 text-muted-foreground">
                                            {toDateInput(campaign.activePeriodStart) || "—"}
                                        </div>
                                    </div>
                                    <InlineField label="Завершення" value={form.endDate} field="endDate" type="date"
                                                 isEditing={editingField === "endDate"} onEdit={setEditingField}
                                                 onBlur={() => setEditingField(null)} onChange={handleChange}
                                                 disabled={isPending} placeholder="Не задано" />
                                </div>
                            </section>

                        </div>

                        <div className="space-y-8">

                            <section className="space-y-4">
                                <SectionTitle icon={<Banknote className="h-4 w-4" />} label="Бюджет та витрати" />
                                <div className="space-y-4">
                                    <InlineField label={`Бюджет (${campaign.currency})`} value={form.budget}
                                                 field="budget" type="number" isEditing={editingField === "budget"}
                                                 onEdit={setEditingField} onBlur={() => setEditingField(null)}
                                                 onChange={handleChange} disabled={isPending} />

                                    <div className="space-y-2">
                                        <Label className="text-muted-foreground">
                                            Фактичні витрати ({campaign.currency})
                                        </Label>
                                        <div className="h-9 px-3 flex items-center text-sm rounded-md border border-dashed border-border bg-muted/30 tabular-nums">
                                            {Number(campaign.actualSpend).toLocaleString("uk-UA")}
                                        </div>
                                    </div>
                                </div>

                                <BudgetProgress
                                    actualSpend={Number(campaign.actualSpend)}
                                    budget={Number(form.budget)}
                                    currency={campaign.currency}
                                />

                                <div className="space-y-2 pt-1">
                                    <Label>Додати витрату</Label>
                                    <div className="flex gap-2">
                                        <Input
                                            type="number"
                                            step="0.01"
                                            min="0"
                                            placeholder="Сума..."
                                            value={spendInput}
                                            onChange={(e) => setSpendInput(e.target.value)}
                                            onKeyDown={(e) => { if (e.key === "Enter") handleAddSpend() }}
                                            disabled={isAddingSpend}
                                            className="h-9"
                                        />
                                        <Button
                                            type="button"
                                            variant="outline"
                                            size="sm"
                                            onClick={() => setSpendInput(String(remaining))}
                                            disabled={isAddingSpend || remaining <= 0}
                                            className="shrink-0 h-9 px-3 text-xs"
                                        >
                                            Макс
                                        </Button>
                                        <Button
                                            type="button"
                                            variant="secondary"
                                            size="sm"
                                            onClick={handleAddSpend}
                                            disabled={isAddingSpend || !spendInput}
                                            className="shrink-0 h-9"
                                        >
                                            {isAddingSpend ? "..." : "+ Додати"}
                                        </Button>
                                    </div>
                                    {remaining > 0 && (
                                        <p className="text-xs text-muted-foreground">
                                            Доступно: {remaining.toLocaleString("uk-UA")} {campaign.currency}
                                        </p>
                                    )}
                                </div>

                                {addSpendError && (
                                    <Alert variant="destructive">
                                        <AlertDescription>{addSpendError}</AlertDescription>
                                    </Alert>
                                )}
                            </section>

                            <section className="space-y-4">
                                <SectionTitle icon={<Users className="h-4 w-4" />} label="Охоплення" />
                                <div className="h-9 px-3 flex items-center gap-2 text-sm rounded-md border border-dashed border-border bg-muted/30">
                                    <Users className="h-4 w-4 text-muted-foreground" />
                                    <span className="font-medium">{campaign.customersCount}</span>
                                    <span className="text-muted-foreground">клієнтів у кампанії</span>
                                </div>
                            </section>

                        </div>
                    </div>

                    {validationError && (
                        <Alert variant="destructive" className="mt-6">
                            <AlertDescription>{validationError}</AlertDescription>
                        </Alert>
                    )}
                    {submitError && (
                        <Alert variant="destructive" className="mt-6">
                            <AlertDescription>{submitError}</AlertDescription>
                        </Alert>
                    )}
                </div>

                <div className="flex justify-end gap-2 p-4 border-t bg-muted/20">
                    {isDirty ? (
                        <>
                            <Button type="button" variant="ghost" onClick={handleCancel} disabled={isPending}>
                                Скасувати зміни
                            </Button>
                            <Button type="button" onClick={handleSubmit} disabled={isPending}>
                                {isPending ? "Збереження..." : "Зберегти зміни"}
                            </Button>
                        </>
                    ) : (
                        <Button type="button" variant="ghost" onClick={() => handleModalClose(false)}>
                            Закрити
                        </Button>
                    )}
                </div>

            </DialogContent>
        </Dialog>
    )
}