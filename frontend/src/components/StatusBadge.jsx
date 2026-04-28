import { memo } from "react";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

const BASE = "border-transparent font-medium shadow-sm";

const COLORS = {
    blue:   "!bg-blue-600   !text-white hover:!bg-blue-700   dark:!bg-blue-500   dark:hover:!bg-blue-600",
    green:  "!bg-green-600  !text-white hover:!bg-green-700  dark:!bg-green-500  dark:hover:!bg-green-600",
    orange: "!bg-orange-500 !text-white hover:!bg-orange-600 dark:!bg-orange-500 dark:hover:!bg-orange-600",
    red:    "!bg-red-600    !text-white hover:!bg-red-700    dark:!bg-red-500    dark:hover:!bg-red-600",
    purple: "!bg-purple-600 !text-white hover:!bg-purple-700 dark:!bg-purple-500 dark:hover:!bg-purple-600",
    muted:  "!bg-muted !text-muted-foreground",
};

const CUSTOMER_STATUSES = {
    "0":     { label: "Новий",        color: "blue"   },
    new:     { label: "Новий",        color: "blue"   },
    "1":     { label: "Активний",     color: "green"  },
    active:  { label: "Активний",     color: "green"  },
    "2":     { label: "Під загрозою", color: "orange" },
    atrisk:  { label: "Під загрозою", color: "orange" },
    "3":     { label: "Відтік",       color: "red"    },
    churned: { label: "Відтік",       color: "red"    },
};

const ORDER_STATUSES = {
    "0":          { label: "Очікує",       color: "blue"   },
    pending:      { label: "Очікує",       color: "blue"   },
    "1":          { label: "В обробці",    color: "orange" },
    processing:   { label: "В обробці",    color: "orange" },
    "2":          { label: "Відправлено",  color: "purple" },
    shipped:      { label: "Відправлено",  color: "purple" },
    "3":          { label: "Завершено",    color: "green"  },
    completed:    { label: "Завершено",    color: "green"  },
    "4":          { label: "Скасовано",    color: "red"    },
    cancelled:    { label: "Скасовано",    color: "red"    },
    "5":          { label: "Повернення",   color: "red"    },
    refunded:     { label: "Повернення",   color: "red"    },

    paid:         { label: "Оплачено",     color: "green"  },
    partiallypaid:{ label: "Частково",     color: "orange" },
};

const CAMPAIGN_STATUSES = {
    "0":          { label: "Чернетка",     color: "muted"  },
    draft:        { label: "Чернетка",     color: "muted"  },
    "1":          { label: "Активна",      color: "green"  },
    active:       { label: "Активна",      color: "green"  },
    "2":          { label: "На паузі",     color: "orange" },
    paused:       { label: "На паузі",     color: "orange" },
    "3":          { label: "Завершена",    color: "purple" },
    completed:    { label: "Завершена",    color: "purple" },
};

const ROLE_STATUSES = {
    admin:   { label: "Admin",   color: "purple" },
    analyst: { label: "Analyst", color: "blue"   },
    viewer:  { label: "Viewer",  color: "muted"  },
}

const REGISTRIES = {
    customer: CUSTOMER_STATUSES,
    campaign: CAMPAIGN_STATUSES,
    role:     ROLE_STATUSES,
    default:  ORDER_STATUSES,
}

export const StatusBadge = memo(function StatusBadge({
                                                         status,
                                                         type = "default",
                                                         className,
                                                     }) {
    if (status === null || status === undefined || status === "") {
        return <Badge className={cn(BASE, COLORS.muted, className)}>—</Badge>;
    }

    const registry = REGISTRIES[type] ?? REGISTRIES.default;
    const key = String(status).trim().toLowerCase();
    const config = registry[key];

    if (!config) {
        return <Badge className={cn(BASE, COLORS.muted, className)}>{String(status)}</Badge>;
    }

    const variant = config.color === "muted" ? "secondary" : "outline";

    return (
        <Badge variant={variant} className={cn(BASE, COLORS[config.color], className)}>
            {config.label}
        </Badge>
    );
});