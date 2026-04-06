import { useState, useEffect } from "react";
import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { UserCircle, Search, Filter } from "lucide-react";

export default function Customers() {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    const fetchCustomers = async () => {
      try {
        const response = await fetch(
            "http://localhost:5044/customers?tenantId=366a4940-217f-44af-ba15-acf7852496e7",
        );
        const data = await response.json();
        setCustomers(data);
      } catch (error) {
        console.error("Помилка завантаження клієнтів:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchCustomers();
  }, []);

  const filteredCustomers = customers.filter(
      (c) =>
          c.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          c.externalId?.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  return (
      <div className="p-6 md:p-10 space-y-6">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h2 className="text-3xl font-bold tracking-tight">Клієнти</h2>
            <p className="text-muted-foreground">
              Список користувачів, які здійснили хоча б одне замовлення.
            </p>
          </div>

          <div className="flex items-center gap-2">
            <div className="relative w-64">
              <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                  placeholder="Пошук клієнта..."
                  className="pl-9"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
            <Badge
                variant="secondary"
                className="h-10 px-4 gap-2 border shadow-sm cursor-pointer hover:bg-muted"
            >
              <Filter className="h-4 w-4" /> Фільтри
            </Badge>
          </div>
        </div>

        <Card>
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/50">
                  <TableHead className="w-[250px]">Клієнт (Email / ID)</TableHead>
                  <TableHead>Телефон</TableHead>
                  <TableHead>Дата реєстрації</TableHead>
                  <TableHead>Статус</TableHead>
                  <TableHead className="text-right">
                    Загальна сума (LTV)
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {loading ? (
                    <TableRow>
                      <TableCell
                          colSpan={5}
                          className="text-center py-10 text-muted-foreground"
                      >
                        Завантаження даних...
                      </TableCell>
                    </TableRow>
                ) : filteredCustomers.length === 0 ? (
                    <TableRow>
                      <TableCell
                          colSpan={5}
                          className="text-center py-10 text-muted-foreground"
                      >
                        Клієнтів не знайдено
                      </TableCell>
                    </TableRow>
                ) : (
                    filteredCustomers.map((customer) => (
                        <TableRow
                            key={customer.id}
                            className="hover:bg-muted/30 transition-colors"
                        >
                          <TableCell className="font-medium">
                            <div className="flex items-center gap-3">
                              <UserCircle className="h-8 w-8 text-muted-foreground/60" />
                              <div className="flex flex-col">
                                <span>{customer.email || "Без email"}</span>
                                <span className="text-xs text-muted-foreground font-mono">
                            {customer.externalId}
                          </span>
                              </div>
                            </div>
                          </TableCell>
                          <TableCell className="text-muted-foreground">
                            {customer.phoneNumber || "—"}
                          </TableCell>
                          <TableCell>
                            {new Date(customer.registrationDate).toLocaleDateString(
                                "uk-UA",
                            )}
                          </TableCell>
                          <TableCell>
                            <Badge
                                variant="outline"
                                className="bg-green-50 text-green-700 border-green-200 dark:bg-green-900/20 dark:text-green-400 dark:border-green-900"
                            >
                              Активний
                            </Badge>
                          </TableCell>
                          <TableCell className="text-right font-bold text-primary">
                            ₴ {customer.totalSpent || "0.00"}
                          </TableCell>
                        </TableRow>
                    ))
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </div>
  );
}