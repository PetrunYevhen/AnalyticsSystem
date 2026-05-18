import { useState, useMemo, useRef, useEffect, useCallback } from "react"
import { Trash2, Plus, ShoppingCart, User, Tag, CreditCard, CheckCircle, X } from "lucide-react"
import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { useCampaigns } from "@/hooks/marketings/useCampaigns"
import { apiGet, apiPost } from "@/lib/api-client"

const ORDER_STATUS = Object.freeze({
    Pending: 0, Processing: 1, Shipped: 2, Completed: 3, Cancelled: 4,
})

const ACQUISITION_CHANNELS = [
    { value: "", label: "— Не вказано —" },
    { value: "Direct", label: "Direct" },
    { value: "Facebook", label: "Facebook" },
    { value: "Twitter", label: "Twitter" },
    { value: "Google", label: "Google" },
    { value: "LinkedIn", label: "LinkedIn" },
    { value: "Organic", label: "Organic" },
]

const PRODUCT_CATEGORIES = [
    { value: "", label: "— Без категорії —" },
    { value: "Electronics", label: "Електроніка" },
    { value: "Clothing", label: "Одяг та взуття" },
    { value: "Food", label: "Продукти харчування" },
    { value: "Home", label: "Дім та сад" },
    { value: "Beauty", label: "Краса та здоров'я" },
    { value: "Sports", label: "Спорт та відпочинок" },
    { value: "Books", label: "Книги та канцтовари" },
    { value: "Toys", label: "Іграшки та дитячі товари" },
    { value: "Auto", label: "Авто" },
    { value: "Services", label: "Послуги" },
    { value: "Other", label: "Інше" },
]

const PAYMENT_METHODS = [
    { value: "", label: "— Без оплати —" },
    { value: "Cash", label: "Готівка" },
    { value: "Card", label: "Картка" },
    { value: "BankTransfer", label: "Банківський переказ" },
    { value: "Other", label: "Інше" },
]

const SELECT_CLS =
    "flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm " +
    "focus:outline-none focus:ring-2 focus:ring-ring disabled:opacity-50"

const initialNewCustomer = Object.freeze({
    fullName: "", phoneNumber: "", externalId: "",
    acquisitionChannel: "", campaignId: ""
})

const initialPayment = Object.freeze({ amount: "", method: "", note: "" })

const toLocalDatetimeInput = (d = new Date()) => {
    const pad = n => String(n).padStart(2, '0')
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}` +
        `T${pad(d.getHours())}:${pad(d.getMinutes())}`
}

const buildInitialOrderMeta = () => ({
    externalOrderId: "", currency: "UAH", status: "Pending",
    orderDate: toLocalDatetimeInput()
})

let _itemSeq = 0
const makeItem = () => ({
    _id: ++_itemSeq, productExternalId: "", productName: "",
    category: "", quantity: 1, unitPrice: 0, unitCost: 0
})

const num = v => { const n = Number(v); return Number.isFinite(n) ? n : 0 }

export function CreateOrderForm({ onSuccess }) {
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    const [customerQuery, setCustomerQuery] = useState("")
    const [suggestions, setSuggestions] = useState([])
    const [searching, setSearching] = useState(false)
    const [selectedCustomer, setSelectedCustomer] = useState(null)
    const [newCustomer, setNewCustomer] = useState(initialNewCustomer)
    const showNewForm = !selectedCustomer && customerQuery.length >= 2 && !searching && suggestions.length === 0

    const [orderMeta, setOrderMeta] = useState(buildInitialOrderMeta)
    const [items, setItems] = useState(() => [makeItem()])
    const [payment, setPayment] = useState(initialPayment)

    const abortRef = useRef(null)
    const searchAbortRef = useRef(null)
    const idempotencyRef = useRef(crypto.randomUUID())
    const { campaigns } = useCampaigns()

    useEffect(() => () => abortRef.current?.abort(), [])

    useEffect(() => {
        if (customerQuery.length < 2) {
            setSuggestions([])
            return
        }
        const timer = setTimeout(async () => {
            searchAbortRef.current?.abort()
            searchAbortRef.current = new AbortController()
            setSearching(true)
            try {
                const res = await apiGet('/customers', {
                    search: customerQuery, pageSize: 6, page: 1,
                    sortBy: "LastOrderDate", direction: "Desc"
                }, searchAbortRef.current.signal)
                setSuggestions(res?.items ?? [])
            } catch (e) {
                if (e.name === 'AbortError') return
                setSuggestions([])
            } finally {
                setSearching(false)
            }
        }, 300)
        return () => clearTimeout(timer)
    }, [customerQuery])

    const handleSelectCustomer = useCallback((c) => {
        setSelectedCustomer(c)
        setCustomerQuery(c.fullName || c.email || "")
        setSuggestions([])
        setNewCustomer(initialNewCustomer)
    }, [])

    const handleClearCustomer = useCallback(() => {
        setSelectedCustomer(null)
        setCustomerQuery("")
        setSuggestions([])
        setNewCustomer(initialNewCustomer)
    }, [])

    const totalAmount = useMemo(
        () => items.reduce((sum, it) => sum + num(it.quantity) * num(it.unitPrice), 0),
        [items]
    )

    const handleNewCustomerChange = (field, value) =>
        setNewCustomer(prev => ({ ...prev, [field]: value }))
    const handleOrderChange = (field, value) =>
        setOrderMeta(prev => ({ ...prev, [field]: value }))
    const handlePaymentChange = (field, value) =>
        setPayment(prev => ({ ...prev, [field]: value }))
    const updateItem = (id, field, value) =>
        setItems(prev => prev.map(it => it._id === id ? { ...it, [field]: value } : it))
    const addItem = () => setItems(prev => [...prev, makeItem()])
    const removeItem = (id) =>
        setItems(prev => prev.length === 1 ? prev : prev.filter(it => it._id !== id))

    const resetForm = () => {
        setSelectedCustomer(null)
        setCustomerQuery("")
        setSuggestions([])
        setNewCustomer(initialNewCustomer)
        setOrderMeta(buildInitialOrderMeta())
        setItems([makeItem()])
        setPayment(initialPayment)
        idempotencyRef.current = crypto.randomUUID()
    }

    const validate = () => {
        const errs = []

        if (!selectedCustomer && !showNewForm)
            errs.push("Оберіть або введіть клієнта.")

        if (showNewForm) {
            if (!newCustomer.fullName.trim()) errs.push("ПІБ обов'язковий.")
            if (!newCustomer.phoneNumber.trim()) errs.push("Телефон обов'язковий.")
        }

        if (customerQuery && !selectedCustomer && !/^\S+@\S+\.\S+$/.test(customerQuery))
            if (customerQuery.includes('@'))
                errs.push("Email має некоректний формат.")

        if (!orderMeta.externalOrderId.trim()) errs.push("External Order ID обов'язковий.")
        if (items.length === 0) errs.push("Потрібен хоча б один товар.")
        items.forEach((it, i) => {
            const tag = `Товар #${i + 1}`
            if (!it.productName.trim()) errs.push(`${tag}: назва обов'язкова.`)
            if (num(it.quantity) <= 0) errs.push(`${tag}: кількість > 0.`)
            if (num(it.unitPrice) < 0) errs.push(`${tag}: ціна не може бути від'ємною.`)
            if (num(it.unitCost) < 0) errs.push(`${tag}: собівартість не може бути від'ємною.`)
        })
        if (num(payment.amount) > totalAmount)
            errs.push("Сума оплати не може перевищувати суму замовлення.")
        if (num(payment.amount) > 0 && !payment.method)
            errs.push("Вкажіть метод оплати.")

        return errs.length ? errs.join('\n') : null
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        if (loading) return
        setError(null)
        const validationError = validate()
        if (validationError) { setError(validationError); return }

        const emailFromQuery = customerQuery.includes('@') ? customerQuery.trim() : null

        const payload = {
            customer: {
                email: selectedCustomer?.email ?? emailFromQuery,
                isNewCustomer: !selectedCustomer,
                ...(!selectedCustomer && {
                    externalId: newCustomer.externalId.trim() || null,
                    fullName: newCustomer.fullName.trim(),
                    phoneNumber: newCustomer.phoneNumber.trim(),
                    acquisitionChannel: newCustomer.acquisitionChannel || null,
                    campaignId: newCustomer.campaignId.trim() || null,
                    registrationDate: null,
                })
            },
            order: {
                externalOrderId: orderMeta.externalOrderId.trim(),
                orderDate: new Date(orderMeta.orderDate).toISOString(),
                status: orderMeta.status,
                currency: orderMeta.currency,
                initialPaymentAmount: num(payment.amount) || null,
                initialPaymentMethod: payment.method || null,
                initialPaymentNote: payment.note.trim() || null,
                items: items.map(i => ({
                    productExternalId: i.productExternalId.trim() || null,
                    productName: i.productName.trim(),
                    category: i.category || null,
                    quantity: num(i.quantity),
                    unitPrice: num(i.unitPrice),
                    unitCost: num(i.unitCost)
                }))
            }
        }

        abortRef.current?.abort()
        abortRef.current = new AbortController()
        setLoading(true)
        try {
            await apiPost('/orders', payload, {
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
                    <CardTitle className="text-2xl">Створення замовлення</CardTitle>
                </CardHeader>
                <CardContent className="space-y-10 px-8">

                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground border-b pb-2">
                            <User className="h-4 w-4" /> Клієнт
                        </h3>

                        {!selectedCustomer && (
                            <div className="relative">
                                <Input
                                    placeholder="Пошук за ім'ям або email..."
                                    value={customerQuery}
                                    onChange={e => {
                                        setCustomerQuery(e.target.value)
                                        setSelectedCustomer(null)
                                    }}
                                    disabled={loading}
                                    autoComplete="off"
                                />
                                {searching && (
                                    <span className="absolute right-3 top-3 text-xs text-muted-foreground">
                                        Пошук...
                                    </span>
                                )}

                                {suggestions.length > 0 && (
                                    <div className="absolute z-50 w-full mt-1 bg-popover border rounded-md shadow-md overflow-hidden">
                                        {suggestions.map(c => (
                                            <div
                                                key={c.id}
                                                className="flex items-center gap-3 px-4 py-2 hover:bg-muted cursor-pointer"
                                                onMouseDown={e => {
                                                    e.preventDefault()
                                                    handleSelectCustomer(c)
                                                }}
                                            >
                                                <div>
                                                    <p className="text-sm font-medium">{c.fullName || "Без імені"}</p>
                                                    <p className="text-xs text-muted-foreground">{c.email || "—"}</p>
                                                </div>
                                            </div>
                                        ))}
                                        <div
                                            className="px-4 py-2 text-xs text-primary hover:bg-muted cursor-pointer border-t"
                                            onMouseDown={e => {
                                                e.preventDefault()
                                                setSuggestions([])
                                            }}
                                        >
                                            + Створити нового клієнта з цим запитом
                                        </div>
                                    </div>
                                )}
                            </div>
                        )}

                        {selectedCustomer && (
                            <div className="flex items-center justify-between p-3 rounded-md border bg-muted/30">
                                <div className="flex items-center gap-2">
                                    <CheckCircle className="h-4 w-4 text-green-500" />
                                    <div>
                                        <p className="text-sm font-medium">{selectedCustomer.fullName}</p>
                                        <p className="text-xs text-muted-foreground">{selectedCustomer.email}</p>
                                    </div>
                                </div>
                                <Button
                                    type="button" variant="ghost" size="icon"
                                    onClick={handleClearCustomer} disabled={loading}
                                >
                                    <X className="h-4 w-4" />
                                </Button>
                            </div>
                        )}

                        {showNewForm && (
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-2">
                                <Input placeholder="ПІБ *" value={newCustomer.fullName}
                                       onChange={e => handleNewCustomerChange('fullName', e.target.value)}
                                       autoComplete="name" disabled={loading} />
                                <Input placeholder="Телефон *" value={newCustomer.phoneNumber}
                                       onChange={e => handleNewCustomerChange('phoneNumber', e.target.value)}
                                       autoComplete="tel" inputMode="tel" disabled={loading} />
                                <Input placeholder="External ID клієнта" value={newCustomer.externalId}
                                       onChange={e => handleNewCustomerChange('externalId', e.target.value)}
                                       disabled={loading} />
                                <select className={SELECT_CLS} value={newCustomer.acquisitionChannel}
                                        onChange={e => handleNewCustomerChange('acquisitionChannel', e.target.value)}
                                        disabled={loading} aria-label="Канал залучення">
                                    {ACQUISITION_CHANNELS.map(ch => (
                                        <option key={ch.value} value={ch.value}>{ch.label}</option>
                                    ))}
                                </select>
                                <select className={SELECT_CLS} value={newCustomer.campaignId}
                                        onChange={e => handleNewCustomerChange('campaignId', e.target.value)}
                                        disabled={loading} aria-label="Кампанія">
                                    <option value="">— Без кампанії —</option>
                                    {campaigns.filter(c => c.status === 'Active').map(c => (
                                        <option key={c.id} value={c.id}>{c.name} ({c.channel})</option>
                                    ))}
                                </select>
                            </div>
                        )}
                    </section>

                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground border-b pb-2">
                            <ShoppingCart className="h-4 w-4" /> Замовлення
                        </h3>
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                            <Input placeholder="External Order ID *" value={orderMeta.externalOrderId}
                                   onChange={e => handleOrderChange('externalOrderId', e.target.value)}
                                   required disabled={loading} />
                            <Input type="datetime-local" value={orderMeta.orderDate}
                                   onChange={e => handleOrderChange('orderDate', e.target.value)}
                                   required disabled={loading} />
                            <select className={SELECT_CLS} value={orderMeta.currency}
                                    onChange={e => handleOrderChange('currency', e.target.value)}
                                    disabled={loading} aria-label="Валюта">
                                <option value="UAH">UAH</option>
                            </select>
                            <select className={SELECT_CLS} value={orderMeta.status}
                                    onChange={e => handleOrderChange('status', e.target.value)}
                                    disabled={loading} aria-label="Статус">
                                {Object.keys(ORDER_STATUS).map(name => (
                                    <option key={name} value={name}>{name}</option>
                                ))}
                            </select>
                        </div>
                    </section>

                    <section className="space-y-4">
                        <div className="flex justify-between items-center border-b pb-2">
                            <h3 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
                                <Tag className="h-4 w-4" /> Товари
                            </h3>
                        </div>
                        <div className="space-y-3">
                            {items.map((item, index) => (
                                <div key={item._id} className="flex flex-col gap-3 p-4 rounded-md border bg-muted/20">
                                    <div className="grid grid-cols-1 md:grid-cols-12 gap-2">
                                        <Input
                                            className="md:col-span-2"
                                            placeholder="SKU"
                                            value={item.productExternalId}
                                            onChange={e => updateItem(item._id, 'productExternalId', e.target.value)}
                                            disabled={loading}
                                        />
                                        <Input
                                            className="md:col-span-6"
                                            placeholder={`Назва товару #${index + 1} *`}
                                            value={item.productName}
                                            onChange={e => updateItem(item._id, 'productName', e.target.value)}
                                            required disabled={loading}
                                        />
                                        <select
                                            className={`${SELECT_CLS} md:col-span-4`}
                                            value={item.category}
                                            onChange={e => updateItem(item._id, 'category', e.target.value)}
                                            disabled={loading}
                                            aria-label="Категорія"
                                        >
                                            {PRODUCT_CATEGORIES.map(c => (
                                                <option key={c.value} value={c.value}>{c.label}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <div className="grid grid-cols-2 md:grid-cols-12 gap-2 items-end">
                                        <div className="md:col-span-2">
                                            <label className="text-xs text-muted-foreground mb-1 block">Кількість *</label>
                                            <Input
                                                type="number" min="1" step="1" placeholder="1"
                                                value={item.quantity}
                                                onChange={e => updateItem(item._id, 'quantity', e.target.value)}
                                                required disabled={loading}
                                            />
                                        </div>
                                        <div className="md:col-span-3">
                                            <label className="text-xs text-muted-foreground mb-1 block">Ціна за од. *</label>
                                            <Input
                                                type="number" step="0.01" min="0" placeholder="0.00"
                                                value={item.unitPrice}
                                                onChange={e => updateItem(item._id, 'unitPrice', e.target.value)}
                                                required disabled={loading}
                                            />
                                        </div>
                                        <div className="md:col-span-3">
                                            <label className="text-xs text-muted-foreground mb-1 block">Собівартість</label>
                                            <Input
                                                type="number" step="0.01" min="0" placeholder="0.00"
                                                value={item.unitCost}
                                                onChange={e => updateItem(item._id, 'unitCost', e.target.value)}
                                                disabled={loading}
                                            />
                                        </div>
                                        <div className="md:col-span-3 flex flex-col">
                                            <label className="text-xs text-muted-foreground mb-1 block">Сума</label>
                                            <div className="h-10 flex items-center px-3 rounded-md border bg-muted/50 font-mono text-sm text-emerald-600">
                                                {(num(item.quantity) * num(item.unitPrice)).toFixed(2)} {orderMeta.currency}
                                            </div>
                                        </div>
                                        <div className="md:col-span-1 flex items-end justify-end">
                                            <Button
                                                type="button" variant="ghost" size="icon"
                                                onClick={() => removeItem(item._id)}
                                                disabled={loading || items.length === 1}
                                                className="text-red-500 h-10 w-10"
                                                aria-label={`Видалити товар ${index + 1}`}
                                            >
                                                <Trash2 className="h-4 w-4" />
                                            </Button>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                        <Button type="button" variant="outline" size="sm" onClick={addItem}
                                disabled={loading} className="w-full border-dashed">
                            <Plus className="h-4 w-4 mr-2" /> Додати товар
                        </Button>
                    </section>

                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground border-b pb-2">
                            <CreditCard className="h-4 w-4" /> Початкова оплата
                        </h3>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                            <div className="flex gap-2 items-center">
                                <Input
                                    className="[&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [appearance:textfield]"
                                    type="number" step="0.01" min="0"
                                    placeholder={`Сума (макс. ${totalAmount.toFixed(2)})`}
                                    value={payment.amount}
                                    onChange={e => handlePaymentChange('amount', e.target.value)}
                                    disabled={loading}
                                />
                                <Button
                                    type="button" variant="outline" size="sm"
                                    onClick={() => handlePaymentChange('amount', totalAmount.toFixed(2))}
                                    disabled={loading || totalAmount === 0}
                                    className="shrink-0"
                                >
                                    Макс
                                </Button>
                            </div>
                            <select className={SELECT_CLS} value={payment.method}
                                    onChange={e => handlePaymentChange('method', e.target.value)}
                                    disabled={loading} aria-label="Метод оплати">
                                {PAYMENT_METHODS.map(m => (
                                    <option key={m.value} value={m.value}>{m.label}</option>
                                ))}
                            </select>
                            <Input placeholder="Примітка" value={payment.note}
                                   onChange={e => handlePaymentChange('note', e.target.value)}
                                   disabled={loading} />
                        </div>
                    </section>

                    {error && (
                        <Alert variant="destructive">
                            <AlertDescription className="whitespace-pre-line">{error}</AlertDescription>
                        </Alert>
                    )}
                </CardContent>
                <CardFooter className="flex justify-end gap-2 border-t mt-4 pt-4 px-8 pb-8">
                    <Button type="button" variant="ghost" onClick={resetForm} disabled={loading}>
                        Скинути
                    </Button>
                    <Button type="submit" disabled={loading}>
                        {loading ? "Збереження..." : "Створити"}
                    </Button>
                </CardFooter>
            </form>
        </Card>
    )
}