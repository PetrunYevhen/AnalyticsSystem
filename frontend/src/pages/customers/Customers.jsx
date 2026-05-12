import { useState, useEffect, useCallback } from "react";
import { useCustomers } from "@/hooks/customers/useCustomers";
import { CustomersTable } from "./components/CustomersTable";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search, ChevronLeft, ChevronRight } from "lucide-react";
import { CustomerDetailsModal } from "@/pages/customers/components/CustomerDetailsModal";

export default function CustomersPage() {
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const {
    items, totalCount, totalPages, hasNextPage, hasPreviousPage,
    loading, error, page, setPage,
    sortBy, direction, handleSort,
    setSearch,
  } = useCustomers();

  const [searchInput, setSearchInput] = useState("");

  useEffect(() => {
    const timer = setTimeout(() => {
      setSearch(searchInput);
      setPage(1);
    }, 300);
    return () => clearTimeout(timer);
  }, [searchInput, setSearch, setPage]);

  const handleCustomerClick = useCallback((customer) => {
    setSelectedCustomer(customer);
  }, []);

  const handleCloseModal = useCallback(() => {
    setSelectedCustomer(null);
  }, []);

  return (
      <div className="px-0 md:px-2 py-6 space-y-6 w-full min-w-0 overflow-x-hidden">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h2 className="text-3xl font-bold tracking-tight">Клієнти</h2>
            <p className="text-muted-foreground">
              {loading ? "Завантаження..." : `${totalCount} клієнтів`}
            </p>
          </div>
          <div className="flex items-center gap-2">
            <div className="relative w-64">
              <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                  placeholder="Пошук клієнта..."
                  className="pl-9"
                  value={searchInput}
                  onChange={(e) => setSearchInput(e.target.value)}
              />
            </div>
          </div>
        </div>

        <Card className="min-h-[600px] flex flex-col">
          <CardContent className="p-0 flex-1 flex flex-col">
            <div className="relative w-full overflow-auto flex-1">
              {selectedCustomer && (
                  <CustomerDetailsModal
                      customer={selectedCustomer}
                      onClose={handleCloseModal}
                  />
              )}
              <CustomersTable
                  items={items} loading={loading} error={error}
                  sortBy={sortBy} direction={direction} onSort={handleSort}
                  onCustomerClick={handleCustomerClick}
              />
            </div>

            {totalPages > 1 && (
                <div className="flex items-center justify-between px-4 py-3 border-t">
                            <span className="text-sm text-muted-foreground">
                                Сторінка {page} з {totalPages}
                            </span>
                  <div className="flex gap-2">
                    <Button
                        variant="outline" size="sm"
                        disabled={!hasPreviousPage || loading}
                        onClick={() => setPage(p => p - 1)}
                    >
                      <ChevronLeft className="h-4 w-4" />
                    </Button>
                    <Button
                        variant="outline" size="sm"
                        disabled={!hasNextPage || loading}
                        onClick={() => setPage(p => p + 1)}
                    >
                      <ChevronRight className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
            )}
          </CardContent>
        </Card>
      </div>
  );
}