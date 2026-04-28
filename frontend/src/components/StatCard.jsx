import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card";
import { TrendingUp, TrendingDown, Minus } from "lucide-react";


export function StatCard({ title, value, change, hint, icon: Icon, loading, valueClassName, invertTrendColors }) {
    if (loading) {
        return (
            <Card className="animate-pulse">
                <CardHeader className="pb-2"><div className="h-4 w-1/2 bg-muted rounded" /></CardHeader>
                <CardContent>
                    <div className="h-8 w-3/4 bg-muted rounded mb-2" />
                    <div className="h-3 w-1/3 bg-muted rounded" />
                </CardContent>
            </Card>
        );
    }

    const safeChange = Number(change) || 0;
    const isPositive = invertTrendColors ? safeChange < 0 : safeChange > 0;
    const isNegative = invertTrendColors ? safeChange > 0 : safeChange < 0;
    const hasChange = change !== undefined && change !== null;

    return (
        <Card className="shadow-sm">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                    {title}
                </CardTitle>
                {Icon && <Icon className="h-4 w-4 text-muted-foreground" />}
            </CardHeader>
            <CardContent>
                <div className={`text-2xl font-bold ${valueClassName ?? ""}`}>{value}</div>

                {hint && !hasChange && (
                    <p className="text-xs text-muted-foreground mt-2">{hint}</p>
                )}

                {hasChange && (
                    <div className="text-xs flex items-center gap-1.5 mt-2">
                        {isPositive && <TrendingUp className="h-3.5 w-3.5 text-emerald-500" />}
                        {isNegative && <TrendingDown className="h-3.5 w-3.5 text-red-500" />}
                        {!isPositive && !isNegative && <Minus className="h-3.5 w-3.5 text-muted-foreground" />}
                        <span className={`font-medium ${
                            isPositive ? "text-emerald-500" :
                                isNegative ? "text-red-500" :
                                    "text-muted-foreground"
                        }`}>
                            {safeChange > 0 ? "+" : ""}{Math.abs(safeChange).toFixed(1)}%
                        </span>
                        <span className="text-muted-foreground">відносно минулого періоду</span>
                    </div>
                )}
            </CardContent>
        </Card>
    );
}