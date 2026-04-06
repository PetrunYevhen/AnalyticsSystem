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
import {AnalyticsIcon} from "@/components/AnalyticsIcon";

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
        <Sidebar>
            <SidebarContent>
                <SidebarGroup>
                    <SidebarGroupLabel className="text-lg font-bold text-primary mt-2 mb-4">
                        <div className="flex items-center gap-4 group cursor-default">
                            <div className="w-12 h-12
                                      text-black dark:text-white">
                                <AnalyticsIcon />
                            </div>
                        </div>
                        AnalyticsUI
                    </SidebarGroupLabel>
                    <SidebarGroupContent>
                        <SidebarMenu>
                            {menuItems.map((item) => (
                                <SidebarMenuItem key={item.title}>
                                    <SidebarMenuButton asChild>
                                        <Link to={item.url}>
                                            <item.icon />
                                            <span>{item.title}</span>
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