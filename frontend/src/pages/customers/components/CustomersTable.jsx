import { memo } from "react";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { UserCircle, ChevronUp, ChevronDown, ChevronsUpDown } from "lucide-react";
import { StatusBadge } from "@/components/StatusBadge";

const COLUMNS = [
    { label: "Імʼя",               sortKey: "FullName",           width: "w-[18%]" },
    { label: "Email",              sortKey: "Email",              width: "w-[21%]" },
    { label: "Останнє замовлення", sortKey: "LastOrderDate",      width: "w-[18%]" },
    { label: "Статус",             sortKey: "Status",             width: "w-[12%]" },
    { label: "Канал залучення",    sortKey: "AcquisitionChannel", width: "w-[16%]" },
    { label: "Загальний дохід",    sortKey: "TotalRevenue",      width: "w-[15%]" },
];

const dateFormatter = new Intl.DateTimeFormat("uk-UA");
const currencyFormatter = new Intl.NumberFormat("uk-UA", {
    style: "currency",
    currency: "UAH",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
});

const SortIcon = memo(function SortIcon({ active, direction }) {
    if (!active) return <ChevronsUpDown className="h-3 w-3 opacity-40 inline ml-1" />;
    return direction === "Asc"
        ? <ChevronUp className="h-3 w-3 inline ml-1" />
        : <ChevronDown className="h-3 w-3 inline ml-1" />;
});

const CustomerRow = memo(function CustomerRow({ customer, onClick }) {
    const lastOrder = customer.lastOrderDate
        ? dateFormatter.format(new Date(customer.lastOrderDate))
        : "—";

    const ltv = currencyFormatter.format(
        typeof customer.totalRevenue === "number" ? customer.totalRevenue : 0
    );

    return (
        <TableRow
            className="hover:bg-muted/30 transition-colors cursor-pointer"
            onClick={() => onClick?.(customer)}
        >
            <TableCell className="font-medium">
                <div className="flex items-center gap-2 min-w-0">
                    <UserCircle className="h-7 w-7 shrink-0 text-muted-foreground/60" />
                    <span className="truncate">{customer.fullName || "Без імені"}</span>
                </div>
            </TableCell>
            <TableCell className="text-muted-foreground truncate max-w-0">
                <span className="block truncate">{customer.email || "—"}</span>
            </TableCell>
            <TableCell>{lastOrder}</TableCell>
            <TableCell>
                <StatusBadge status={customer.status} type="customer" />
            </TableCell>
            <TableCell className="truncate max-w-0">
                <span className="block truncate font-medium text-primary">
                    {customer.acquisitionChannel || "—"}
                </span>
            </TableCell>
            <TableCell className="font-medium text-muted-foreground text-right truncate max-w-0">
                <span className="block truncate text-right">{currencyFormatter.format(customer.totalRevenue ?? 0)}</span>
            </TableCell>
        </TableRow>
    );
});

export function CustomersTable({ items, onCustomerClick, loading, error, sortBy, direction, onSort }) {
    if (error)          return <div className="p-10 text-center text-destructive">{error}</div>;
    if (loading)        return <div className="p-10 text-center text-muted-foreground">Завантаження даних...</div>;
    if (!items?.length) return <div className="p-10 text-center text-muted-foreground">Клієнтів не знайдено</div>;

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
                                className={`${col.width} cursor-pointer select-none hover:text-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring`}
                                onClick={() => onSort?.(col.sortKey)}
                                onKeyDown={(e) => {
                                    if (e.key !== "Enter" && e.key !== " ") return;
                                    e.preventDefault();
                                    onSort?.(col.sortKey);
                                }}
                            >
                                {col.label}
                                <SortIcon active={active} direction={direction} />
                            </TableHead>
                        );
                    })}
                </TableRow>
            </TableHeader>
            <TableBody>
                {items.map((customer) => (
                    <CustomerRow
                        key={customer.id}
                        customer={customer}
                        onClick={onCustomerClick}
                    />
                ))}
            </TableBody>
        </Table>
    );
}