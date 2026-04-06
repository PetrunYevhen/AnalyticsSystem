import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";

export default function Dashboard() {
  return (
      <div className="p-6 md:p-10 max-w-7xl mx-auto w-full space-y-6">
        <h2 className="text-3xl font-bold tracking-tight">Огляд панелі</h2>

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
  );
}