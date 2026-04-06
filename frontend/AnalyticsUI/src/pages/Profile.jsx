import { useState, useEffect } from "react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  CardDescription,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Building, Mail, User, ShieldCheck } from "lucide-react";

export default function Profile() {
  // 1. Стан для даних профілю
  const [tenant, setTenant] = useState(null);
  const [loading, setLoading] = useState(true);

  // 2. Функція для завантаження даних з вашого .NET API
  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await fetch(
          "http://localhost:5044/tenants/366a4940-217f-44af-ba15-acf7852496e7",
        );

        console.log("Статус відповіді:", response.status); // Має бути 200

        const data = await response.json();
        console.log("Дані отримані успішно:", data);
        setTenant(data);
      } catch (error) {
        console.error("Критична помилка запиту:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, []);

  if (loading)
    return <div className="p-10 text-center">Завантаження профілю...</div>;

  return (
    <div className="p-6 md:p-10 max-w-4xl mx-auto space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-3xl font-bold tracking-tight">
          Профіль організації
        </h2>
        <Button variant="outline">Редагувати</Button>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Дані компанії</CardTitle>
            <CardDescription>
              Основна інформація про ваш робочий простір
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-1">
              <label className="text-xs font-medium text-muted-foreground">
                Назва тенанта
              </label>
              <div className="flex items-center gap-2 text-sm">
                <Building className="h-4 w-4 text-primary" />
                <span className="font-semibold">{tenant?.name}</span>
              </div>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-medium text-muted-foreground">
                ID Організації
              </label>
              <div className="text-sm font-mono bg-muted p-1 rounded text-xs">
                {tenant?.id || "6f823-abc-..."}
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Адміністратор</CardTitle>
            <CardDescription>Контактна особа власника тенанта</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-1">
              <label className="text-xs font-medium text-muted-foreground">
                Ім'я
              </label>
              <div className="flex items-center gap-2 text-sm">
                <User className="h-4 w-4 text-primary" />
                <span>{tenant?.adminName || "Адміністратор"}</span>
              </div>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-medium text-muted-foreground">
                Email
              </label>
              <div className="flex items-center gap-2 text-sm">
                <Mail className="h-4 w-4 text-primary" />
                <span>{tenant?.email || "admin@example.com"}</span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ShieldCheck className="h-5 w-5 text-green-600 dark:text-green-500" />
            Статус акаунту
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-between p-4 border rounded-lg
                    bg-green-50/50 border-green-200
                    dark:bg-green-950/20 dark:border-green-900">
            <div>
              <p className="font-semibold text-green-800 dark:text-green-400">
                Активний
              </p>
              <p className="text-sm text-green-700 dark:text-green-500/90">
                Ваша організація має повний доступ до функцій
              </p>
            </div>
            <Button
                size="sm"
                variant="outline"
                className="bg-white border-green-200 text-green-700 hover:bg-green-50
                   dark:bg-transparent dark:border-green-900 dark:text-green-400 dark:hover:bg-green-950/50"
            >
              Деталі підписки
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
