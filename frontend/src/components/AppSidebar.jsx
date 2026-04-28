import { Home, Users, Package, Megaphone, CreditCard, Settings, UserCircle } from "lucide-react"
import {
    Sidebar,
    SidebarContent,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
} from "@/components/ui/sidebar"
import { Link } from "react-router-dom"
import { AnalyticsIcon } from "@/components/AnalyticsIcon"

const menuItems = [
    { title: "Головна", url: "/", icon: Home },
    { title: "Клієнти", url: "/customers", icon: UserCircle },
    { title: "Замовлення", url: "/orders", icon: Package },
    { title: "Маркетинг", url: "/marketing", icon: Megaphone },
    { title: "Транзакції", url: "/transactions", icon: CreditCard },
    { title: "Налаштування", url: "/settings", icon: Settings },
]

export function AppSidebar() {
    return (
        <Sidebar style={{ "--sidebar-width": "14rem" }}>
            <SidebarContent>
                <SidebarGroup>
                    <SidebarGroupLabel className="text-lg font-bold text-primary mt-2 mb-4 h-auto py-2">
                        <div className="flex items-center gap-3 group cursor-default overflow-hidden">
                            <div className="w-8 h-8 text-black dark:text-white shrink-0">
                                <AnalyticsIcon />
                            </div>
                            <span className="truncate">Metrify</span>
                        </div>
                    </SidebarGroupLabel>
                    <SidebarGroupContent>
                        <SidebarMenu>
                            {menuItems.map((item) => (
                                <SidebarMenuItem key={item.title}>
                                    <SidebarMenuButton asChild>
                                        <Link to={item.url} className="flex items-center gap-3">
                                            <item.icon className="shrink-0" />
                                            <span className="truncate">{item.title}</span>
                                        </Link>
                                    </SidebarMenuButton>
                                </SidebarMenuItem>
                            ))}
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
            </SidebarContent>
        </Sidebar>
    )
}