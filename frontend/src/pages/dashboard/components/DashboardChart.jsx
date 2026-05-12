import { useMemo } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { BarChart3 } from "lucide-react";
import { Area, AreaChart, ResponsiveContainer, Tooltip, XAxis, YAxis, CartesianGrid } from "recharts";

const currencyFormatter = new Intl.NumberFormat("uk-UA", {
    style: "currency",
    currency: "UAH",
    maximumFractionDigits: 0
});

const dateFormatter = new Intl.DateTimeFormat("uk-UA", {
    day: "numeric",
    month: "short"
});

export function DashboardChart({ data, loading }) {
    const chartData = useMemo(() => {
        if (!data?.revenueChart || !Array.isArray(data.revenueChart)) return [];

        return data.revenueChart.map(item => {
            const dateObj = new Date(item.date);
            return {
                ...item,
                displayDate: isNaN(dateObj.getTime()) ? item.date : dateFormatter.format(dateObj)
            };
        });
    }, [data?.revenueChart]);

    const CustomTooltip = ({ active, payload, label }) => {
        if (active && payload && payload.length) {
            return (
                <div className="bg-zinc-950 border border-zinc-800 p-3 rounded-lg shadow-lg">
                    <p className="text-sm text-zinc-400 mb-1">{label}</p>
                    <p className="text-lg font-bold text-emerald-500">
                        {currencyFormatter.format(payload[0].value)}
                    </p>
                </div>
            );
        }
        return null;
    };

    return (
        <Card className="w-full shadow-sm flex flex-col">
            <CardHeader>
                <CardTitle className="text-lg flex items-center gap-2">
                    <BarChart3 className="h-5 w-5 text-muted-foreground" />
                    Аналітика доходів
                </CardTitle>
                <CardDescription>Динаміка за обраний період</CardDescription>
            </CardHeader>
            <CardContent className="flex-1 min-h-[400px] p-0 pb-4">
                {loading ? (
                    <div className="h-full w-full flex items-center justify-center">
                        <span className="text-muted-foreground animate-pulse">Завантаження графіка...</span>
                    </div>
                ) : chartData.length === 0 ? (
                    <div className="h-full w-full flex items-center justify-center">
                        <span className="text-muted-foreground text-sm">Немає даних за обраний період</span>
                    </div>
                ) : (
                    <ResponsiveContainer width="100%" height={400}>
                        <AreaChart data={chartData} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
                            <defs>
                                <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
                                    <stop offset="5%" stopColor="#10b981" stopOpacity={0.3} />
                                    <stop offset="95%" stopColor="#10b981" stopOpacity={0} />
                                </linearGradient>
                            </defs>
                            <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#27272a" />
                            <XAxis
                                dataKey="displayDate"
                                stroke="#71717a"
                                fontSize={12}
                                tickLine={false}
                                axisLine={false}
                                dy={10}
                                minTickGap={20}
                            />
                            <YAxis
                                stroke="#71717a"
                                fontSize={12}
                                tickLine={false}
                                axisLine={false}
                                tickFormatter={(value) => currencyFormatter.format(value)}
                                width={80}
                            />
                            <Tooltip content={<CustomTooltip />} cursor={{ stroke: '#3f3f46', strokeWidth: 1, strokeDasharray: '4 4' }} />
                            <Area
                                type="monotone"
                                dataKey="revenue"
                                stroke="#10b981"
                                strokeWidth={2}
                                fillOpacity={1}
                                fill="url(#colorRevenue)"
                                activeDot={{ r: 6, fill: "#10b981", stroke: "#18181b", strokeWidth: 2 }}
                            />
                        </AreaChart>
                    </ResponsiveContainer>
                )}
            </CardContent>
        </Card>
    );
}