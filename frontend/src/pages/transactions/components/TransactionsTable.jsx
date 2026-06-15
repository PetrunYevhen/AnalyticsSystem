import { memo } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { StatusBadge } from "@/components/StatusBadge"
import { CreditCard, Landmark, Banknote, HelpCircle, ChevronUp, ChevronDown, ChevronsUpDown } from "lucide-react"

const dateFormatter = new Intl.DateTimeFormat('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
})
const moneyFormatter = new Intl.NumberFormat('uk-UA', { minimumFractionDigits: 2 })

const METHOD_ICONS = {
    Card: CreditCard,
    BankTransfer: Landmark,
    Cash: Banknote,
    Other: HelpCircle
}

const safeFormatDate = (dateString) => {
    if (!dateString) return "—"
    const date = new Date(dateString)
    return isNaN(date.getTime()) ? "Невалідна дата" : dateFormatter.format(date)
}

const COLUMNS = [
    { label: "ID Замовлення", sortKey: "ExternalOrderId", align: "left",  width: "w-[15%]" },
    { label: "Користувач",    sortKey: "CustomerName",    align: "left",  width: "w-[20%]" },
    { label: "Метод",         sortKey: "PaymentMethod",   align: "left",  width: "w-[15%]" },
    { label: "Дата та час",   sortKey: "TransactionDate", align: "left",  width: "w-[20%]" },
    { label: "Статус",        sortKey: "Status",          align: "left",  width: "w-[15%]" },
    { label: "Сума",          sortKey: "TotalAmount",     align: "right", width: "w-[15%]" },
]

const SortIcon = memo(function SortIcon({ active, direction }) {
    if (!active) return <ChevronsUpDown className="h-3 w-3 opacity-40 inline ml-1" />
    return direction === 'Asc'
        ? <ChevronUp className="h-3 w-3 inline ml-1" />
        : <ChevronDown className="h-3 w-3 inline ml-1" />
})

const TransactionRow = memo(function TransactionRow({ tx }) {
    const Icon = METHOD_ICONS[tx.method] || HelpCircle

    return (
        <TableRow className="hover:bg-muted/30 transition-colors">
            <TableCell className="text-xs truncate max-w-[120px]" title={tx.externalOrderId}>
                {tx.externalOrderId || "—"}
            </TableCell>
            <TableCell className="text-sm font-medium text-muted-foreground truncate max-w-[150px]" title={tx.customerName}>
                {tx.customerName}
            </TableCell>
            <TableCell>
                <div className="flex items-center gap-2">
                    <Icon className="h-4 w-4 text-muted-foreground" />
                    <span className="text-sm truncate">{tx.method || "Невідомо"}</span>
                </div>
            </TableCell>
            <TableCell className="text-sm text-muted-foreground whitespace-nowrap">
                {safeFormatDate(tx.date)}
            </TableCell>
            <TableCell>
                <StatusBadge status={tx.status} />
            </TableCell>
            <TableCell className="text-right font-bold tabular-nums">
                <span className={tx.transactionType === "Refund" ? "text-destructive" : ""}>
                    {tx.transactionType === "Refund" ? "-" : ""}₴ {moneyFormatter.format(tx.amount ?? 0)}
                </span>
            </TableCell>
        </TableRow>
    )
})

export const TransactionsTable = memo(function TransactionsTable({
                                                                     items, loading, error, sortBy, direction, onSort
                                                                 }) {
    if (loading) return <div className="p-10 text-center text-muted-foreground">Завантаження...</div>
    if (error)   return <div className="p-10 text-center text-destructive">{error}</div>
    if (!items?.length) return <div className="p-10 text-center text-muted-foreground">Транзакцій не знайдено</div>

    return (
        <div className="overflow-x-auto">
            <Table className="table-fixed w-full min-w-[800px]">
                <TableHeader className="bg-muted/50">
                    <TableRow>
                        {COLUMNS.map((col) => {
                            const active = sortBy === col.sortKey
                            return (
                                <TableHead
                                    key={col.sortKey}
                                    role="button"
                                    tabIndex={0}
                                    aria-sort={active ? (direction === "Asc" ? "ascending" : "descending") : "none"}
                                    className={`${col.width} cursor-pointer select-none hover:text-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring ${col.align === "right" ? "text-right" : ""}`}
                                    onClick={() => onSort?.(col.sortKey)}
                                    onKeyDown={(e) => {
                                        if (e.key === "Enter" || e.key === " ") {
                                            e.preventDefault()
                                            onSort?.(col.sortKey)
                                        }
                                    }}
                                >
                                    {col.label}
                                    <SortIcon active={active} direction={direction} />
                                </TableHead>
                            )
                        })}
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {items.map((tx, index) => (
                        <TransactionRow key={`${tx.externalOrderId}-${tx.date}-${index}`} tx={tx} />
                    ))}
                </TableBody>
        </Table>
</div>
)
})