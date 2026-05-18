import { useState, useEffect } from "react"
import { useOrderPayment } from "@/hooks/orders/useOrderPayment"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Receipt, CreditCard, Ban, Undo2, Package } from "lucide-react"
import { StatusBadge } from "@/components/StatusBadge"
import { useOrderItems } from "@/hooks/orders/useOrderItems"


const formatMoney = (amount, currency = "UAH") =>
    new Intl.NumberFormat('uk-UA', { style: 'currency', currency }).format(amount || 0)

const formatDate = (dateStr) => {
    if (!dateStr) return "—"
    return new Intl.DateTimeFormat('uk-UA', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
    }).format(new Date(dateStr))
}

const SELECT_CLS =
    "flex h-8 w-full rounded-md border border-input bg-background px-3 text-sm " +
    "text-foreground focus:outline-none disabled:opacity-50"

export function OrderDetailsModal({ order, onClose, onSuccess }) {
    const isOpen = !!order
    const orderId = order?.orderId

    const {
        paymentInfo, transactions, loading, mutating, error: serverError,
        fetchPayment, recordPayment, cancelOrder, processRefund
    } = useOrderPayment(orderId)
    const { items, loading: itemsLoading, error: itemsError, fetchItems }
        = useOrderItems(orderId)


    const [showPaymentForm, setShowPaymentForm] = useState(false)
    const [paymentAmount, setPaymentAmount] = useState("")
    const [paymentMethod, setPaymentMethod] = useState("Cash")
    const [paymentNote, setPaymentNote] = useState("")
    const [paymentError, setPaymentError] = useState(null)

    const [showRefundForm, setShowRefundForm] = useState(false)
    const [refundAmount, setRefundAmount] = useState("")
    const [refundMethod, setRefundMethod] = useState("Cash")
    const [refundNote, setRefundNote] = useState("")
    const [refundError, setRefundError] = useState(null)

    useEffect(() => {
        if (orderId) {
            fetchPayment()
            fetchItems()
        }
    }, [orderId, fetchPayment, fetchItems])

    const totalAmount = paymentInfo?.totalAmount ?? order?.totalAmount ?? 0
    const paidAmount = paymentInfo?.paidAmount ?? 0
    const refundedAmount = paymentInfo?.refundedAmount ?? 0
    const netPaid = paidAmount - refundedAmount
    const leftToPay = Math.max(0, totalAmount - paidAmount)
    const isFullyPaid = paymentInfo?.status === 'Paid' || leftToPay === 0
    const currency = paymentInfo?.currency ?? "UAH"

    const handleRecordPaymentSubmit = async (e) => {
        e.preventDefault()
        if (Number(paymentAmount) <= 0) { setPaymentError("Вкажіть суму."); return }
        if (Number(paymentAmount) > leftToPay) { setPaymentError(`Макс: ${formatMoney(leftToPay, currency)}`); return }
        setPaymentError(null)
        const success = await recordPayment({ amount: paymentAmount, method: paymentMethod, note: paymentNote })
        if (success) {
            setShowPaymentForm(false)
            setPaymentAmount("")
            setPaymentNote("")
            onSuccess?.()
        }
    }

    const handleRefundSubmit = async (e) => {
        e.preventDefault()
        if (Number(refundAmount) <= 0) { setRefundError("Вкажіть суму."); return }
        if (Number(refundAmount) > paidAmount) { setRefundError(`Макс: ${formatMoney(paidAmount, currency)}`); return }
        setRefundError(null)
        const success = await processRefund({ amount: refundAmount, method: refundMethod, note: refundNote })
        if (success) {
            setShowRefundForm(false)
            setRefundAmount("")
            setRefundNote("")
            onSuccess?.()
        }
    }

    const handleCancelClick = async () => {
        if (!window.confirm("Ви впевнені, що хочете скасувати замовлення?")) return
        const success = await cancelOrder()
        if (success) { onSuccess?.(); onClose() }
    }

    const togglePaymentForm = () => {
        setShowPaymentForm(p => !p)
        setShowRefundForm(false)
        setPaymentError(null)
    }

    const toggleRefundForm = () => {
        setShowRefundForm(p => !p)
        setShowPaymentForm(false)
        setRefundError(null)
    }

    if (!isOpen) return null

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="sm:max-w-[90vw] max-h-[90vh] p-0 flex flex-col overflow-hidden bg-background border-border text-foreground">
                <DialogHeader className="p-6 pb-4 border-b border-border shrink-0">
                    <DialogTitle className="text-2xl font-bold flex items-center gap-3">
                        Замовлення #{order.externalOrderId}
                        <StatusBadge status={order.status} />
                    </DialogTitle>
                    <DialogDescription className="text-muted-foreground mt-1">
                        Від {formatDate(order.orderDate)} • Клієнт: {order.customerName}
                    </DialogDescription>
                </DialogHeader>

                <div className="flex-1 min-h-0 overflow-y-auto">
                    {loading ? (
                        <div className="p-10 text-center text-muted-foreground">Завантаження даних...</div>
                    ) : (
                        <div className="p-6 space-y-6">
                            {serverError && (
                                <div className="p-3 text-sm text-destructive bg-destructive/10 border border-destructive/30 rounded-md">
                                    {serverError}
                                </div>
                            )}

                            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                <Card className="md:col-span-2">
                                    <CardHeader className="pb-2">
                                        <CardTitle className="text-sm text-muted-foreground">Фінансовий статус</CardTitle>
                                    </CardHeader>
                                    <CardContent className="flex flex-wrap items-end gap-8">
                                        <div>
                                            <p className="text-xs text-muted-foreground mb-1">Загальна сума</p>
                                            <p className="text-2xl font-mono">{formatMoney(totalAmount, currency)}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs text-muted-foreground mb-1">Сплачено</p>
                                            <p className="text-2xl font-mono text-emerald-500">{formatMoney(netPaid, currency)}</p>
                                        </div>
                                        {refundedAmount > 0 && (
                                            <div>
                                                <p className="text-xs text-muted-foreground mb-1">Повернуто</p>
                                                <p className="text-2xl font-mono text-orange-500">-{formatMoney(refundedAmount, currency)}</p>
                                            </div>
                                        )}
                                        <div>
                                            <p className="text-xs text-muted-foreground mb-1">Залишок до сплати</p>
                                            <p className="text-2xl font-mono text-orange-500">{formatMoney(leftToPay, currency)}</p>
                                        </div>
                                    </CardContent>
                                </Card>

                                <Card>
                                    <CardHeader className="pb-2">
                                        <CardTitle className="text-sm text-muted-foreground">Дії</CardTitle>
                                    </CardHeader>
                                    <CardContent className="flex flex-col gap-2">
                                        <Button
                                            type="button" size="sm"
                                            className="w-full bg-emerald-600 hover:bg-emerald-700 text-white"
                                            disabled={isFullyPaid || order.status === 'Cancelled' || mutating}
                                            onClick={togglePaymentForm}
                                        >
                                            <CreditCard className="w-4 h-4 mr-2" />
                                            {showPaymentForm ? "Сховати форму" : "Записати оплату"}
                                        </Button>

                                        {showPaymentForm && (
                                            <form onSubmit={handleRecordPaymentSubmit} className="space-y-2 border border-border rounded-md p-3 bg-muted/30">
                                                <Input
                                                    type="number" step="0.01" min="0"
                                                    placeholder={`Сума (макс. ${leftToPay.toFixed(2)})`}
                                                    value={paymentAmount}
                                                    onChange={e => setPaymentAmount(e.target.value)}
                                                    disabled={mutating} className="h-8 text-sm"
                                                />
                                                <select className={SELECT_CLS} value={paymentMethod}
                                                        onChange={e => setPaymentMethod(e.target.value)} disabled={mutating}>
                                                    <option value="Cash">Готівка</option>
                                                    <option value="Card">Картка</option>
                                                    <option value="BankTransfer">Переказ</option>
                                                </select>
                                                <Input
                                                    placeholder="Примітка" value={paymentNote}
                                                    onChange={e => setPaymentNote(e.target.value)}
                                                    disabled={mutating} className="h-8 text-sm"
                                                />
                                                {paymentError && <p className="text-xs text-destructive">{paymentError}</p>}
                                                <Button type="submit" size="sm"
                                                        className="w-full bg-emerald-600 hover:bg-emerald-700 text-white"
                                                        disabled={mutating}>
                                                    {mutating ? "Збереження..." : "Підтвердити оплату"}
                                                </Button>
                                            </form>
                                        )}

                                        <Button
                                            type="button" size="sm" variant="outline"
                                            className="w-full"
                                            disabled={paidAmount === 0 || mutating}
                                            onClick={toggleRefundForm}
                                        >
                                            <Undo2 className="w-4 h-4 mr-2" />
                                            {showRefundForm ? "Сховати форму" : "Повернути кошти"}
                                        </Button>

                                        {showRefundForm && (
                                            <form onSubmit={handleRefundSubmit} className="space-y-2 border border-orange-500/30 rounded-md p-3 bg-orange-500/5">
                                                <Input
                                                    type="number" step="0.01" min="0"
                                                    placeholder={`Сума (макс. ${paidAmount.toFixed(2)})`}
                                                    value={refundAmount}
                                                    onChange={e => setRefundAmount(e.target.value)}
                                                    disabled={mutating} className="h-8 text-sm"
                                                />
                                                <Button
                                                    type="button" size="sm" variant="outline"
                                                    className="w-full h-7 text-xs"
                                                    disabled={mutating}
                                                    onClick={() => setRefundAmount(paidAmount.toFixed(2))}
                                                >
                                                    Макс
                                                </Button>
                                                <select className={SELECT_CLS} value={refundMethod}
                                                        onChange={e => setRefundMethod(e.target.value)} disabled={mutating}>
                                                    <option value="Cash">Готівка</option>
                                                    <option value="Card">Картка</option>
                                                    <option value="BankTransfer">Переказ</option>
                                                </select>
                                                <Input
                                                    placeholder="Причина повернення" value={refundNote}
                                                    onChange={e => setRefundNote(e.target.value)}
                                                    disabled={mutating} className="h-8 text-sm"
                                                />
                                                {refundError && <p className="text-xs text-destructive">{refundError}</p>}
                                                <Button type="submit" size="sm" variant="outline"
                                                        className="w-full border-orange-500/30 text-orange-500 hover:bg-orange-500/10"
                                                        disabled={mutating}>
                                                    {mutating ? "Обробка..." : "Підтвердити повернення"}
                                                </Button>
                                            </form>
                                        )}

                                        <Button
                                            type="button" size="sm" variant="destructive"
                                            className="w-full mt-4"
                                            disabled={order.status === 'Cancelled' || mutating}
                                            onClick={handleCancelClick}
                                        >
                                            <Ban className="w-4 h-4 mr-2" />
                                            {mutating ? "Обробка..." : "Скасувати замовлення"}
                                        </Button>
                                    </CardContent>
                                </Card>
                            </div>

                            <Card>
                                <CardHeader>
                                    <CardTitle className="flex items-center gap-2 text-lg">
                                        <Package className="h-5 w-5" /> Товари замовлення
                                    </CardTitle>
                                </CardHeader>
                                <CardContent className="p-0">
                                    {itemsLoading ? (
                                        <div className="p-6 text-center text-muted-foreground">Завантаження...</div>
                                    ) : itemsError ? (
                                        <div className="p-6 text-center text-destructive">{itemsError}</div>
                                    ) : (
                                        <Table>
                                            <TableHeader className="bg-muted/50">
                                                <TableRow className="border-border hover:bg-transparent">
                                                    <TableHead>Назва</TableHead>
                                                    <TableHead>Категорія</TableHead>
                                                    <TableHead className="text-right">Кількість</TableHead>
                                                    <TableHead className="text-right">Ціна</TableHead>
                                                    <TableHead className="text-right">Собівартість</TableHead>
                                                    <TableHead className="text-right">Сума</TableHead>
                                                </TableRow>
                                            </TableHeader>
                                            <TableBody>
                                                {items.length === 0 ? (
                                                    <TableRow className="border-border hover:bg-transparent">
                                                        <TableCell colSpan={6} className="text-center text-muted-foreground py-6">
                                                            Товарів не знайдено
                                                        </TableCell>
                                                    </TableRow>
                                                ) : (
                                                    items.map((item, index) => (
                                                        <TableRow key={item.productId ?? index} className="border-border hover:bg-muted/30 transition-colors">
                                                            <TableCell className="font-medium">{item.productName || "—"}</TableCell>
                                                            <TableCell className="text-muted-foreground">{item.productCategory || "—"}</TableCell>
                                                            <TableCell className="text-right">{item.quantity}</TableCell>
                                                            <TableCell className="text-right font-mono">{formatMoney(item.unitPrice, item.currency)}</TableCell>
                                                            <TableCell className="text-right font-mono text-muted-foreground">{formatMoney(item.unitCost, item.currency)}</TableCell>
                                                            <TableCell className="text-right font-mono">{formatMoney(item.quantity * item.unitPrice, item.currency)}</TableCell>
                                                        </TableRow>
                                                    ))
                                                )}
                                            </TableBody>
                                        </Table>
                                    )}
                                </CardContent>
                            </Card>

                            <Card>
                                <CardHeader>
                                    <CardTitle className="flex items-center gap-2 text-lg">
                                        <Receipt className="h-5 w-5" /> Історія транзакцій
                                    </CardTitle>
                                </CardHeader>
                                <CardContent className="p-0">
                                    <Table>
                                        <TableHeader className="bg-muted/50">
                                            <TableRow className="border-border hover:bg-transparent">
                                                <TableHead>ID транзакції</TableHead>
                                                <TableHead>Дата</TableHead>
                                                <TableHead>Метод</TableHead>
                                                <TableHead className="text-right">Сума</TableHead>
                                            </TableRow>
                                        </TableHeader>
                                        <TableBody>
                                            {transactions.length === 0 ? (
                                                <TableRow className="border-border hover:bg-transparent">
                                                    <TableCell colSpan={4} className="text-center text-muted-foreground py-6">
                                                        Транзакцій не знайдено
                                                    </TableCell>
                                                </TableRow>
                                            ) : (
                                                transactions.map(tx => (
                                                    <TableRow key={tx.id} className="border-border hover:bg-muted/30 transition-colors">
                                                        <TableCell className="font-mono text-xs text-muted-foreground">{tx.id}</TableCell>
                                                        <TableCell className="whitespace-nowrap">{formatDate(tx.paidAt || tx.createdAt)}</TableCell>
                                                        <TableCell>
                                                            <div className="flex gap-2 items-center">
                                                                <StatusBadge status={tx.method} type="default" />
                                                                {tx.type === 'Refund' && (
                                                                    <span className="text-xs text-orange-500 border border-orange-500/30 px-1.5 rounded bg-orange-500/10">
                                                                        Повернення
                                                                    </span>
                                                                )}
                                                            </div>
                                                        </TableCell>
                                                        <TableCell className={`text-right font-mono ${tx.type === 'Refund' || tx.amount < 0 ? 'text-orange-500' : 'text-emerald-500'}`}>
                                                            {tx.type === 'Refund' || tx.amount < 0 ? '-' : '+'}{formatMoney(Math.abs(tx.amount), currency)}
                                                        </TableCell>
                                                    </TableRow>
                                                ))
                                            )}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                            </Card>
                        </div>
                    )}
                </div>
            </DialogContent>
        </Dialog>
    )
}