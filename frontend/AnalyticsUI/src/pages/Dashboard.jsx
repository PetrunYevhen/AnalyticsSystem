import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

// 1. Імпортуємо компоненти для бокового меню
import {
  SidebarProvider,
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarTrigger,
} from "@/components/ui/sidebar";

// 2. Імпортуємо іконки
import {
  Home,
  Megaphone,
  Users,
  Settings,
  CreditCard,
  Package,
} from "lucide-react";

// Створюємо список пунктів меню для зручності
const menuItems = [
  { title: "Головна", url: "#", icon: Home },
  { title: "Користувачі", url: "#", icon: Users },
  { title: "Замовлення", url: "#", icon: Package },
  { title: "Маркетинг", url: "#", icon: Megaphone },
  { title: "Транзакції", url: "#", icon: CreditCard },
  { title: "Налаштування", url: "#", icon: Settings },
];

export default function Dashboard() {
  return (
    // SidebarProvider має обгортати весь макет, щоб меню працювало правильно
    <SidebarProvider>
      {/* ЛІВА ЧАСТИНА: Бокове меню */}
      <Sidebar>
        <SidebarContent>
          <SidebarGroup>
            {/* Назва вашого додатку в меню */}
            <SidebarGroupLabel className="text-lg font-bold text-primary mt-2 mb-4">
              AnalyticsUI
            </SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {menuItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild>
                      <a href={item.url}>
                        <item.icon />
                        <span>{item.title}</span>
                      </a>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        </SidebarContent>
      </Sidebar>

      {/* ПРАВА ЧАСТИНА: Основний контент */}
      <main className="flex-1 bg-muted/40 min-h-screen">
        {/* Верхня панелька інструментів над контентом */}
        <header className="flex items-center h-16 px-6 border-b bg-background shadow-sm gap-4">
          {/* Ця кнопка ховає/показує бокове меню (особливо корисно на мобільних) */}
          <SidebarTrigger />

          <div className="flex-1">
            <Input
              type="search"
              placeholder="Пошук даних..."
              className="max-w-sm bg-background"
            />
          </div>
          <Button variant="outline">Профіль</Button>
        </header>

        {/* Самі картки з даними */}
        <div className="p-6 md:p-10 max-w-7xl mx-auto w-full space-y-6">
          <h2 className="text-3xl font-bold tracking-tight">Огляд панелі</h2>

          {/* Сітка з картками (Grid) */}
          <div className="grid gap-6 md:grid-cols-3">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Загальний дохід
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold">$45,231.89</div>
                <p className="text-xs text-muted-foreground mt-1">
                  +20.1% у порівнянні з минулим місяцем
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Нові користувачі
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold">+2350</div>
                <p className="text-xs text-muted-foreground mt-1">
                  +180.1% у порівнянні з минулим місяцем
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Активність
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-3xl font-bold">+12,234</div>
                <p className="text-xs text-muted-foreground mt-1">
                  +19% у порівнянні з минулим місяцем
                </p>
              </CardContent>
            </Card>
          </div>

          {/* Велика нижня картка */}
          <Card className="mt-6">
            <CardHeader>
              <CardTitle>Останні транзакції</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="h-[200px] flex items-center justify-center border-2 border-dashed rounded-md border-muted">
                <p className="text-muted-foreground">
                  Тут буде ваш графік або таблиця
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </SidebarProvider>
  );
}
