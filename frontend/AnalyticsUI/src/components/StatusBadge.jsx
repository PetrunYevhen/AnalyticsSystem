import { Badge } from "@/components/ui/badge"

export const StatusBadge = ({ status, type = "default" }) => {
    const s = String(status).trim().toLowerCase();

    const baseStyles = "bg-transparent shadow-none border";
    if (type === "customer") {
    switch (s) {
        case "0": case "potential":
            return <Badge className={`${baseStyles} text-blue-700 dark:text-blue-400 dark:border-blue-900/50`}>Потенційни</Badge>
        case "1": case "new":
            return <Badge className={`${baseStyles} text-blue-700 dark:text-blue-400 dark:border-blue-900/50`}>Новий</Badge>
        case "2": case "active":
            return <Badge className={`${baseStyles} text-green-700  dark:text-green-400 dark:border-green-900/50`}>Активний</Badge>
        case "3": case "atrisk":
            return <Badge className={`${baseStyles} text-orange-600 dark:text-orange-400 dark:border-orange-900/50`}>Під загрозою</Badge>
        case "4": case "churned":
            return <Badge variant="outline" className={`${baseStyles} text-red-600 border-red-200 dark:text-red-400 dark:border-red-900/50`}>Відтік</Badge>;
        case "5": case "reactivated":
            return <Badge variant="outline" className={`${baseStyles} text-purple-600 border-purple-200 dark:text-purple-400 dark:border-purple-900/50`}>Reactivated</Badge>;
        default:
            return <Badge variant="outline" className={`${baseStyles} text-muted-foreground`}>{status}</Badge>;
    }
    }
    switch (s) {
        case "completed": case "paid": case "success": case "1":
            return <Badge className={`${baseStyles} text-green-700  dark:text-green-400 dark:border-green-900/50`}>Виконано</Badge>
        case "pending": case "0":
            return <Badge className={`${baseStyles} text-blue-700 dark:text-blue-400 dark:border-blue-900/50`}>Очікує</Badge>
        case "cancelled": case "failed": case "4":
            return <Badge variant="outline" className={`${baseStyles} text-red-600 border-red-200 dark:text-red-400 dark:border-red-900/50`}>Churned</Badge>;
        default:
            return <Badge variant="outline" className={`${baseStyles} text-muted-foreground`}>{status}</Badge>;
    }
}