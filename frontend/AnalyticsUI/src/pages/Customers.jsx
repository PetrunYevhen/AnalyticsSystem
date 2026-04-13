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
import {StatusBadge} from "@/components/StatusBadge";

export default function Customers() {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    const fetchCustomers = async () => {
      try {
        const response = await fetch(
            "http://localhost:5044/customer",
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

        <Card className="min-h-[600px] flex flex-col">
          <CardContent className="p-0 flex-1">
            <div className="relative w-full overflow-auto">
              <Table className="table-fixed">
                <TableHeader>
                  <TableRow className="bg-muted/50">
                    <TableHead className="w-[20%]">Імʼя / ID</TableHead>
                    <TableHead className="w-[22%]">Email</TableHead>
                    <TableHead className="w-[19%]">Останнє замовлення</TableHead>
                    <TableHead className="w-[13%] text-center" >Статус</TableHead>
                    <TableHead className="w-[16%]">Канал залучення</TableHead>
                    <TableHead className="w-[10%]">LTV</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {loading ? (
                      <TableRow>
                        <TableCell colSpan={6} className="text-center h-[400px] text-muted-foreground">
                          Завантаження даних...
                        </TableCell>
                      </TableRow>
                  ) : filteredCustomers.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={6} className="text-center h-[400px] text-muted-foreground">
                          Клієнтів не знайдено
                        </TableCell>
                      </TableRow>
                  ) : (
                      filteredCustomers.map((customer) => (
                          <TableRow key={customer.id} className="hover:bg-muted/30 transition-colors h-16">
                            <TableCell className="font-medium truncate"> {/* truncate запобігає розтягуванню */}
                              <div className="flex items-center gap-3 overflow-hidden">
                                <UserCircle className="h-8 w-8 shrink-0 text-muted-foreground/60" />
                                <div className="flex flex-col overflow-hidden">
                                  <span className="truncate">{customer.fullName || "Без імені"}</span>
                                  <span className="text-xs text-muted-foreground font-mono truncate">
                        {customer.externalId}
                      </span>
                                </div>
                              </div>
                            </TableCell>
                            <TableCell className="text-muted-foreground truncate">
                              {customer.email || "—"}
                            </TableCell>
                            <TableCell className="whitespace-nowrap">
                              {customer.lastOrderDate ? new Date(customer.lastOrderDate).toLocaleDateString("uk-UA") : "—"}
                            </TableCell>
                            <TableCell className="!p-0 text-left">
                              <div className="flex items-center justify-start h-16 pl-4">
                                <StatusBadge status={customer.status} type="customer" />
                              </div>
                            </TableCell>
                            <TableCell className=" font-bold text-primary">
                               {customer.acquisitionChannel || "0.00"}
                            </TableCell>
                            <TableCell className="font-medium text-muted-foreground">
                              ₴ {customer.ltv || "0.00"}
                            </TableCell>
                          </TableRow>
                      ))
                  )}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      </div>
  );
}