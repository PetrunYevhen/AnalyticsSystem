import { cn } from "@/lib/utils";

const PERIODS = [
    { value: 7,  label: "7 днів"  },
    { value: 30, label: "30 днів" },
    { value: 90, label: "90 днів" },
];

export function PeriodSelector({ value, onChange, disabled }) {
    return (
        <div
            role="tablist"
            aria-label="Період статистики"
            className="inline-flex items-center gap-1 rounded-md border bg-muted/30 p-1"
        >
            {PERIODS.map((period) => {
                const isActive = value === period.value;
                return (
                    <button
                        key={period.value}
                        role="tab"
                        aria-selected={isActive}
                        disabled={disabled}
                        onClick={() => onChange(period.value)}
                        className={cn(
                            "px-3 py-1.5 text-sm font-medium rounded-sm transition-colors",
                            "disabled:opacity-50 disabled:cursor-not-allowed",
                            isActive
                                ? "bg-background text-foreground shadow-sm"
                                : "text-muted-foreground hover:text-foreground"
                        )}
                    >
                        {period.label}
                    </button>
                );
            })}
        </div>
    );
}