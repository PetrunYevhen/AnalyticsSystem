import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"
import { AppSidebar } from "./AppSidebar"
import { Outlet, Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { Sun, Moon, Search, Download, Upload, AlertCircle } from "lucide-react"
import { useState, useEffect, useRef } from "react"
import { Input } from "@/components/ui/input"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
    DropdownMenuSeparator,
    DropdownMenuLabel,
} from "@/components/ui/dropdown-menu"
import { isAuthenticated } from "@/api/auth"

function downloadCsv(url, filename) {
    const token = localStorage.getItem("token")
    fetch(url, {
        headers: { Authorization: `Bearer ${token}` }
    })
        .then(res => {
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            return res.blob()
        })
        .then(blob => {
            const a = document.createElement("a")
            a.href = URL.createObjectURL(blob)
            a.download = filename
            a.click()
            URL.revokeObjectURL(a.href)
        })
        .catch(err => console.error("Export failed:", err))
}

async function uploadCsv(url, file) {
    const token = localStorage.getItem("token")
    const formData = new FormData()
    formData.append("file", file)

    const res = await fetch(url, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
        body: formData,
    })

    const data = await res.json()

    if (!res.ok) throw new Error(JSON.stringify(data))
    return data
}

export default function Layout() {
    const [isDark, setIsDark] = useState(() => localStorage.getItem("theme") === "dark")
    const [importStatus, setImportStatus] = useState(null)
    const fileInputRef = useRef(null)
    const pendingImportUrl = useRef(null)

    useEffect(() => {
        const root = window.document.documentElement
        isDark ? root.classList.add("dark") : root.classList.remove("dark")
        localStorage.setItem("theme", isDark ? "dark" : "light")
    }, [isDark])

    const formatDate = (date) => {
        const y = date.getFullYear()
        const m = String(date.getMonth() + 1).padStart(2, "0")
        const d = String(date.getDate()).padStart(2, "0")
        return `${y}-${m}-${d}`
    }

    const today = formatDate(new Date())
    const yearAgo = formatDate(new Date(Date.now() - 365 * 24 * 60 * 60 * 1000))

    const exports = [
        { label: "Замовлення",           url: `/api/orders/export?from=${yearAgo}&to=${today}`,    file: "orders.csv" },
        { label: "Клієнти",              url: `/api/customers/export?from=${yearAgo}&to=${today}`, file: "customers.csv" },
        { label: "Маркетингові витрати", url: `/api/marketing/export?from=${yearAgo}&to=${today}`, file: "marketing-expenses.csv" },
        { label: "Транзакції",           url: `/api/transactions/export?from=${yearAgo}&to=${today}`, file: "transactions.csv" },
    ]

    const imports = [
        { label: "Замовлення",           url: "/api/orders/import" },
        { label: "Клієнти",              url: "/api/customers/import" },
        { label: "Маркетингові витрати", url: "/api/marketing/import" },
        { label: "Транзакції",           url: "/api/transactions/import" },]

    const handleImportClick = (url) => {
        pendingImportUrl.current = url
        fileInputRef.current.click()
    }

    const handleFileChange = async (e) => {
        const file = e.target.files?.[0]
        if (!file) return
        e.target.value = "" 

        try {
            const result = await uploadCsv(pendingImportUrl.current, file)
            const msg = `Імпортовано: ${result.imported}, пропущено: ${result.skipped}`
            const errors = result.errors?.length > 0 ? result.errors : null
            setImportStatus({ message: msg, errors })
        } catch (err) {
            setImportStatus({ message: "Помилка імпорту", errors: [err.message] })
        }

        setTimeout(() => setImportStatus(null), 8000)
    }

    return (
        <SidebarProvider>
            <AppSidebar />
            <main className="flex-1 flex flex-col min-w-0 w-full bg-muted/40 dark:bg-background min-h-screen overflow-x-hidden">
                <header className="flex items-center justify-between h-16 px-6 border-b bg-background shadow-sm gap-4 shrink-0">
                    <div className="flex items-center">
                        <SidebarTrigger />
                    </div>

                    <div className="flex-1 flex justify-center max-w-sm hidden md:block relative">
                        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input
                            type="search"
                            placeholder="Глобальний пошук..."
                            className="pl-9 bg-muted/50 focus-visible:bg-background transition-all border-none shadow-none"
                        />
                    </div>

                    <div className="flex items-center justify-end gap-3 shrink-0">
                        <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => setIsDark(!isDark)}
                            className="rounded-full shrink-0"
                        >
                            {isDark ? <Sun className="h-5 w-5" /> : <Moon className="h-5 w-5" />}
                        </Button>

                        <input
                            ref={fileInputRef}
                            type="file"
                            accept=".csv"
                            className="hidden"
                            onChange={handleFileChange}
                        />

                        <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                                <Button
                                    variant="secondary"
                                    className="gap-2 bg-emerald-600/10 text-emerald-600 hover:bg-emerald-600/20 dark:text-emerald-400"
                                >
                                    <Download className="h-4 w-4" />
                                    <span className="hidden sm:inline">CSV</span>
                                </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end" className="w-52">
                                <DropdownMenuLabel className="flex items-center gap-2">
                                    <Download className="h-3.5 w-3.5" /> Експорт
                                </DropdownMenuLabel>
                                {exports.map(e => (
                                    <DropdownMenuItem
                                        key={e.file}
                                        onClick={() => downloadCsv(e.url, e.file)}
                                    >
                                        {e.label}
                                    </DropdownMenuItem>
                                ))}

                                <DropdownMenuSeparator />

                                <DropdownMenuLabel className="flex items-center gap-2">
                                    <Upload className="h-3.5 w-3.5" /> Імпорт
                                </DropdownMenuLabel>
                                {imports.map(i => (
                                    <DropdownMenuItem
                                        key={i.url}
                                        onClick={() => handleImportClick(i.url)}
                                    >
                                        {i.label}
                                    </DropdownMenuItem>
                                ))}
                            </DropdownMenuContent>
                        </DropdownMenu>
                    </div>
                </header>

                {importStatus && (
                    <div className="fixed bottom-4 right-4 z-50 max-w-sm bg-background border rounded-lg shadow-lg p-4">
                        <p className="text-sm font-medium">{importStatus.message}</p>
                        {importStatus.errors && (
                            <ul className="mt-2 text-xs text-destructive space-y-1 max-h-32 overflow-y-auto">
                                {importStatus.errors.map((e, i) => (
                                    <li key={i}>{e}</li>
                                ))}
                            </ul>
                        )}
                    </div>
                )}

                {!isAuthenticated() && (
                    <div className="mx-4 mt-4 p-4 bg-yellow-950/30 text-yellow-400 text-sm border border-yellow-900/50 rounded-md flex items-center gap-2">
                        <AlertCircle className="h-4 w-4 shrink-0" />
                        Ви не авторизовані. <Link to="/login" className="underline font-medium hover:text-yellow-300">Увійдіть в систему</Link>, щоб бачити дані.
                    </div>
                )}

                <Outlet />
            </main>
        </SidebarProvider>
    )
}