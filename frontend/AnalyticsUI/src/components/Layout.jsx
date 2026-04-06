import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"
import { AppSidebar } from "./AppSidebar"
import { Outlet, Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { User, Sun, Moon, Search } from "lucide-react"
import { useState, useEffect } from "react"
import {Input} from "@/components/ui/input"

export default function Layout() {
    const [isDark, setIsDark] = useState(() => {
        const savedTheme = localStorage.getItem("theme")
        return savedTheme === "dark"
    })

    // 2. Слідкуємо за змінами теми і записуємо їх у сховище + додаємо клас до HTML
    useEffect(() => {
        const root = window.document.documentElement
        if (isDark) {
            root.classList.add("dark")
            localStorage.setItem("theme", "dark")
        } else {
            root.classList.remove("dark")
            localStorage.setItem("theme", "light")
        }
    }, [isDark])

    return (
        <SidebarProvider>
            <AppSidebar />
            <main className="flex-1 bg-muted/40 dark:bg-background min-h-screen">
                {/* ВЕРХНЯ ПАНЕЛЬ */}
                <header className="flex items-center h-16 px-6 border-b bg-background shadow-sm gap-4">
                    <SidebarTrigger />
                    <div className="flex-1 flex justify-center">
                        <div className="relative w-full max-w-sm hidden md:block">
                            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                            <Input
                                type="search"
                                placeholder="Глобальний пошук..."
                                className="pl-9 bg-muted/50 focus-visible:bg-background transition-all border-none shadow-none"
                            />
                        </div>
                    </div>
                    <div className="flex-1">
                    </div>

                    <div className="flex items-center gap-2">
                        {/* КНОПКА ТЕМНОЇ ТЕМИ */}
                        <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => setIsDark(!isDark)}
                            className="rounded-full"
                        >
                            {isDark ? <Sun className="h-5 w-5" /> : <Moon className="h-5 w-5" />}
                        </Button>

                        {/* КНОПКА ПРОФІЛЮ */}
                        <Button variant="outline" asChild className="gap-2">
                            <Link to="/profile">
                                <User className="h-4 w-4" />
                                <span className="hidden sm:inline">Профіль</span>
                            </Link>
                        </Button>
                    </div>
                </header>
                <Outlet />
            </main>
        </SidebarProvider>
    )
}