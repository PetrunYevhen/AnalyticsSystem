import { useState } from "react";
import { subDays } from "date-fns";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { DateRangePicker } from "@/components/DataRangePicker";
import { RecentTransactions } from "@/components/RecentTransactions";
import { AddOrderModal } from "@/pages/orders/components/modals/AddOrderModal";

import { useDashboard } from "@/hooks/dashboard/useDashboard";
import { DashboardStats } from "./components/DashboardStats";
import { DashboardChart } from "./components/DashboardChart";

export default function DashboardPage() {
    const [range, setRange] = useState({
        from: subDays(new Date(), 30),
        to: new Date(),
    });

    const { data, loading, error, refetch } = useDashboard(range);

    return (
        <div className="px-0 md:px-2 py-6 space-y-6 w-full min-w-0 overflow-x-hidden">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b pb-6">
                <div className="space-y-1">
                    <h2 className="text-3xl font-bold tracking-tight">Огляд панелі</h2>
                    <p className="text-muted-foreground text-sm">
                        Операційна аналітика та фінансові метрики
                    </p>
                </div>
                <div className="flex items-center gap-3">
                    <DateRangePicker value={range} onChange={setRange} disabled={loading} />
                    <AddOrderModal onImportSuccess={refetch} />
                </div>
            </div>

            {error && (
                <div className="p-4 bg-red-950/30 text-red-400 text-sm border border-red-900/50 rounded-md flex items-center gap-2">
                    <AlertCircle className="h-4 w-4" /> {error}
                </div>
            )}

            <DashboardStats data={data} loading={loading} />

            <div className="space-y-6">
                <div className="w-full">
                    <DashboardChart data={data} loading={loading} />
                </div>

                <div className="w-full">
                    <RecentTransactions
                        transactions={data?.recentTransactions ?? []}
                        loading={loading}
                    />
                </div>
            </div>
        </div>
    );
}