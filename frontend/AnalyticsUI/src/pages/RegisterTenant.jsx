import { Button } from "@/components/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Building, Mail, Lock } from "lucide-react";

export default function RegisterTenant() {
  return (
    // Відцентруємо форму по середині екрану
    <div className="min-h-screen flex items-center justify-center bg-muted/40 p-4">
      <Card className="w-full max-w-md shadow-lg">
        <CardHeader className="space-y-1 text-center">
          <CardTitle className="text-2xl font-bold tracking-tight">
            Реєстрація організації
          </CardTitle>
          <CardDescription>
            Створіть новий робочий простір (tenant) для вашої команди
          </CardDescription>
        </CardHeader>

        <CardContent className="space-y-4">
          {/* Поле: Назва компанії */}
          <div className="space-y-2">
            <label
              className="text-sm font-medium leading-none"
              htmlFor="tenantName"
            >
              Назва компанії
            </label>
            <div className="relative">
              <Building className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                id="tenantName"
                placeholder="Введіть назву..."
                className="pl-9"
              />
            </div>
          </div>

          {/* Поле: Електронна пошта */}
          <div className="space-y-2">
            <label className="text-sm font-medium leading-none" htmlFor="email">
              Електронна пошта адміністратора
            </label>
            <div className="relative">
              <Mail className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                id="email"
                type="email"
                placeholder="name@example.com"
                className="pl-9"
              />
            </div>
          </div>

          {/* Поле: Пароль */}
          <div className="space-y-2">
            <label
              className="text-sm font-medium leading-none"
              htmlFor="password"
            >
              Пароль
            </label>
            <div className="relative">
              <Lock className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                id="password"
                type="password"
                placeholder="••••••••"
                className="pl-9"
              />
            </div>
          </div>
        </CardContent>

        <CardFooter>
          <Button className="w-full text-md h-10">
            Створити робочий простір
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
}
