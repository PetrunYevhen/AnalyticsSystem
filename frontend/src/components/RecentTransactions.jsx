import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
} from "@/components/ui/card";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { StatusBadge } from "@/components/StatusBadge";


const currencyFormatter = new Intl.NumberFormat("uk-UA", {
    style: "currency",
    currency: "USD",
});

const dateFormatter = new Intl.DateTimeFormat("uk-UA", {
    day: "2-digit",
    month: "short",
    year: "numeric",
});

function formatDate(value) {
    if (!value) return "—";
    const d = new Date(value);
    if (Number.isNaN(d.getTime())) return "—";
    return dateFormatter.format(d);
}

function formatAmount(value) {
    const n = Number(value);
    if (!Number.isFinite(n)) return "—";
    return currencyFormatter.format(n);
}

export function RecentTransactions({ transactions = [] }) {
    return (
        <Card>
            <CardHeader>
                <CardTitle>Останні транзакції</CardTitle>
                <CardDescription>
                    Всього {transactions.length} транзакцій за тиждень
                </CardDescription>
            </CardHeader>
            <CardContent>
                {transactions.length === 0 ? (
                    <div className="h-[200px] flex flex-col items-center justify-center text-center border-2 border-dashed rounded-md border-muted">
                        <p className="text-muted-foreground">Ще немає транзакцій</p>
                        <p className="text-xs text-muted-foreground/60 mt-1">
                            Тут з'являться ваші перші продажі
                        </p>
                    </div>
                ) : (
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Клієнт</TableHead>
                                <TableHead>Статус</TableHead>
                                <TableHead>Дата</TableHead>
                                <TableHead className="text-right">Сума</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {transactions.map((t) => (
                                <TableRow key={t.id}>
                                    <TableCell>
                                        <div className="font-medium">
                                            {t.customer ?? t.customerName ?? "—"}
                                        </div>
                                        <div className="text-xs text-muted-foreground">
                                            {t.email ?? t.customerEmail ?? "—"}
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        <StatusBadge status={t.status} />
                                    </TableCell>
                                    <TableCell className="text-muted-foreground">
                                        {formatDate(t.date ?? t.createdAt)}
                                    </TableCell>
                                    <TableCell className="text-right font-medium tabular-nums">
                                        {formatAmount(t.amount)}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                )}
            </CardContent>
        </Card>
    );
}