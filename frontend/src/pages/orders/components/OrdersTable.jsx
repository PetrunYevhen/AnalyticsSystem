import { memo, useCallback } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { StatusBadge } from "@/components/StatusBadge"
import { ChevronUp, ChevronDown, ChevronsUpDown } from "lucide-react"

const COLUMNS = [
    { label: "Номер",  sortKey: "ExternalOrderId", align: "left",  width: "w-[18%]" },
    { label: "Клієнт", sortKey: "CustomerName",    align: "left",  width: "w-[32%]" },
    { label: "Дата",   sortKey: "OrderDate",       align: "left",  width: "w-[18%]" },
    { label: "Статус", sortKey: "Status",          align: "left",  width: "w-[16%]" },
    { label: "Сума",   sortKey: "TotalAmount",     align: "right", width: "w-[16%]" },
]

const dateFormatter = new Intl.DateTimeFormat("uk-UA")
const currencyFormatter = new Intl.NumberFormat("uk-UA", {
    style: "currency",
    currency: "UAH",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
})

const SortIcon = memo(function SortIcon({ active, direction }) {
    if (!active) return <ChevronsUpDown className="h-3 w-3 opacity-40 inline ml-1" />
    return direction === "Asc"
        ? <ChevronUp className="h-3 w-3 inline ml-1" />
        : <ChevronDown className="h-3 w-3 inline ml-1" />
})

const OrderRow = memo(function OrderRow({ order, onRowClick }) {
    const handleClick = useCallback(() => onRowClick?.(order), [order, onRowClick])
    const handleKeyDown = useCallback((e) => {
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault()
            onRowClick?.(order)
        }
    }, [order, onRowClick])

    const dateLabel = order.orderDate ? dateFormatter.format(new Date(order.orderDate)) : "—"
    const amountLabel = currencyFormatter.format(
        typeof order.totalAmount === "number" ? order.totalAmount : 0
    )

    return (
        <TableRow
            role="button"
            tabIndex={0}
            aria-label={`Відкрити замовлення ${order.externalOrderId}`}
            className="cursor-pointer hover:bg-muted/30 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
            onClick={handleClick}
            onKeyDown={handleKeyDown}
        >
            <TableCell>{order.externalOrderId}</TableCell>
            <TableCell className="truncate max-w-0">
                <span className="block truncate">{order.customerName || "—"}</span>
            </TableCell>
            <TableCell>{dateLabel}</TableCell>
            <TableCell><StatusBadge status={order.status} /></TableCell>
            <TableCell className="text-right font-mono">{amountLabel}</TableCell>
        </TableRow>
    )
})

export function OrdersTable({ items, loading, error, sortBy, direction, onSort, onRowClick }) {
    const handleHeaderClick = useCallback((e) => {
        const key = e.currentTarget.dataset.sortKey
        if (key) onSort?.(key)
    }, [onSort])

    const handleHeaderKeyDown = useCallback((e) => {
        if (e.key !== "Enter" && e.key !== " ") return
        e.preventDefault()
        const key = e.currentTarget.dataset.sortKey
        if (key) onSort?.(key)
    }, [onSort])

    if (loading)        return <div className="p-10 text-center text-muted-foreground">Завантаження...</div>
    if (error)          return <div className="p-10 text-center text-destructive">{error}</div>
    if (!items?.length) return <div className="p-10 text-center text-muted-foreground">Немає даних</div>

    return (
        <Table className="w-full table-fixed">
            <TableHeader>
                <TableRow className="bg-muted/50">
                    {COLUMNS.map((col) => {
                        const active = sortBy === col.sortKey;
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
                                        e.preventDefault();
                                        onSort?.(col.sortKey);
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
                {items.map(order => (
                    <OrderRow key={order.orderId} order={order} onRowClick={onRowClick} />
                ))}
            </TableBody>
        </Table>
    )
}