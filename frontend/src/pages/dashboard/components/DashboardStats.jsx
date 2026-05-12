import { Activity, DollarSign, Users } from "lucide-react";
import { StatCard } from "@/components/StatCard";

const currencyFormatter = new Intl.NumberFormat("uk-UA", { style: "currency", currency: "UAH" });
const numberFormatter = new Intl.NumberFormat("uk-UA");

export function DashboardStats({ data, loading }) {
    if (loading || !data) {
        return (
            <div className="grid gap-4 md:grid-cols-3">
                {Array.from({ length: 3 }).map((_, i) => <StatCard key={i} title="—" value="—" index={i} loading />)}
            </div>
        );
    }

    const cards = [
        {
            title: "Загальний дохід",
            value: currencyFormatter.format(data.totalRevenue?.value ?? 0),
            change: data.totalRevenue?.changePercent ?? 0,
            icon: DollarSign,
        },
        {
            title: "Нові користувачі",
            value: `+${numberFormatter.format(data.newCustomers?.value ?? 0)}`,
            change: data.newCustomers?.changePercent ?? 0,
            icon: Users,
        },
        {
            title: "Активність",
            value: `+${numberFormatter.format(data.activity?.value ?? 0)}`,
            change: data.activity?.changePercent ?? 0,
            icon: Activity,
        },
    ];

    return (
        <div className="grid gap-4 md:grid-cols-3">
            {cards.map((stat, i) => (
                <StatCard key={stat.title} {...stat} index={i} />
            ))}
        </div>
    );
}