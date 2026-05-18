import { Search, X } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Checkbox } from "@/components/ui/checkbox"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { ScrollArea } from "@/components/ui/scroll-area"
import { useCampaignCustomers } from "@/hooks/marketings/useCampaignCustomers"

const getInitials = (name) => {
    if (!name) return "??"
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase()
}

export function AddCustomersToCampaignModal({ campaign, onClose, onSuccess }) {
    const isOpen = !!campaign
    const {
        search, setSearch, selectedIds, setSelectedIds,
        fetching, loading, removingId, error,
        available: availableCustomers,
        assigned: assignedCustomers,
        totalAssignedCount,
        handleAssign, handleRemove
    } = useCampaignCustomers(campaign, isOpen, onSuccess)

    const isAllSelected = availableCustomers.length > 0 && selectedIds.size === availableCustomers.length

    const handleSelectAll = (checked) => {
        if (checked) setSelectedIds(new Set(availableCustomers.map(c => c.id)))
        else setSelectedIds(new Set())
    }

    const toggleCustomer = (id) => {
        const next = new Set(selectedIds)
        if (next.has(id)) next.delete(id)
        else next.add(id)
        setSelectedIds(next)
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="sm:max-w-xl p-0 gap-0 overflow-hidden bg-zinc-950 border-zinc-800 text-zinc-100">
                <DialogHeader className="p-6 pb-4 border-b border-zinc-800">
                    <DialogTitle className="text-xl font-semibold">
                        Призначити клієнтів — <span className="text-blue-400">{campaign?.name}</span>
                    </DialogTitle>
                    <DialogDescription className="text-zinc-400 text-sm mt-1">
                        Керуйте клієнтами поточної рекламної кампанії.
                    </DialogDescription>
                </DialogHeader>

                <div className="px-6 py-4 border-b border-zinc-800 bg-zinc-900/30">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-zinc-500" />
                        <Input
                            placeholder="Пошук за ім'ям або email..."
                            className="pl-9 bg-zinc-900 border-zinc-800 text-zinc-100 placeholder:text-zinc-500"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                        />
                    </div>
                </div>

                <Tabs defaultValue="add" className="w-full flex flex-col h-[400px]">
                    <div className="px-6 pt-2 border-b border-zinc-800">
                        <TabsList className="bg-transparent gap-4 p-0 h-auto">
                            <TabsTrigger
                                value="add"
                                className="data-[state=active]:bg-transparent data-[state=active]:border-b-2 data-[state=active]:border-blue-500 data-[state=active]:shadow-none rounded-none px-2 pb-2 pt-0 text-zinc-400 data-[state=active]:text-zinc-100"
                            >
                                Додати клієнтів
                            </TabsTrigger>
                            <TabsTrigger
                                value="assigned"
                                className="data-[state=active]:bg-transparent data-[state=active]:border-b-2 data-[state=active]:border-blue-500 data-[state=active]:shadow-none rounded-none px-2 pb-2 pt-0 text-zinc-400 data-[state=active]:text-zinc-100 flex items-center gap-2"
                            >
                                В кампанії <span className="bg-zinc-800 text-xs px-2 py-0.5 rounded-full">{totalAssignedCount}</span>
                            </TabsTrigger>
                        </TabsList>
                    </div>

                    <TabsContent value="add" className="flex-1 overflow-hidden m-0 flex flex-col data-[state=inactive]:hidden min-h-0">
                        <div className="px-6 py-2 border-b border-zinc-800 flex justify-between items-center bg-zinc-900/50">
                            <div className="flex items-center gap-3">
                                <Checkbox
                                    id="select-all"
                                    checked={isAllSelected}
                                    onCheckedChange={handleSelectAll}
                                    disabled={availableCustomers.length === 0}
                                    className="border-zinc-600 data-[state=checked]:bg-blue-600"
                                />
                                <label htmlFor="select-all" className="text-sm font-medium cursor-pointer text-zinc-300">
                                    Вибрати всіх ({availableCustomers.length})
                                </label>
                            </div>
                        </div>

                        <ScrollArea className="flex-1 min-h-0">
                            {fetching ? (
                                <div className="p-6 text-center text-zinc-500 text-sm">Завантаження...</div>
                            ) : availableCustomers.length === 0 ? (
                                <div className="p-6 text-center text-zinc-500 text-sm">Всі клієнти призначені або не знайдено</div>
                            ) : (
                                <div className="flex flex-col pb-2">
                                    {availableCustomers.map((c) => (
                                        <label key={c.id} className="flex items-center gap-4 p-4 hover:bg-zinc-900/80 transition-colors cursor-pointer border-b border-zinc-800/50 last:border-0">
                                            <Checkbox
                                                checked={selectedIds.has(c.id)}
                                                onCheckedChange={() => toggleCustomer(c.id)}
                                                className="border-zinc-600 data-[state=checked]:bg-blue-600"
                                            />
                                            <Avatar className="h-8 w-8 border border-zinc-800">
                                                <AvatarFallback className="bg-blue-900 text-blue-200 text-xs">{getInitials(c.fullName)}</AvatarFallback>
                                            </Avatar>
                                            <div className="flex-1 overflow-hidden">
                                                <p className="text-sm font-medium text-zinc-100 truncate">{c.fullName}</p>
                                                <p className="text-xs text-zinc-400 truncate">{c.email}</p>
                                            </div>
                                        </label>
                                    ))}
                                </div>
                            )}
                        </ScrollArea>

                        <div className="p-4 border-t border-zinc-800 flex justify-between items-center bg-zinc-950 mt-auto">
                            <span className="text-sm text-zinc-400">Обрано: <strong className="text-zinc-100">{selectedIds.size}</strong></span>
                            <div className="flex gap-2">
                                <Button
                                    type="button"
                                    variant="outline"
                                    onClick={(e) => { e.preventDefault(); onClose(); }}
                                    disabled={loading}
                                    className="border-zinc-700 text-zinc-300"
                                >
                                    Скасувати
                                </Button>
                                <Button
                                    type="button"
                                    onClick={(e) => { e.preventDefault(); handleAssign(); }}
                                    disabled={loading || selectedIds.size === 0}
                                    className="bg-zinc-800 hover:bg-zinc-700 text-zinc-100 border border-zinc-700"
                                >
                                    {loading ? "Обробка..." : "Призначити"}
                                </Button>
                            </div>
                        </div>
                    </TabsContent>

                    <TabsContent value="assigned" className="flex-1 overflow-hidden m-0 flex flex-col data-[state=inactive]:hidden min-h-0">
                        <ScrollArea className="flex-1 min-h-0">
                            {fetching ? (
                                <div className="p-6 text-center text-zinc-500 text-sm">Завантаження...</div>
                            ) : assignedCustomers.length === 0 ? (
                                <div className="p-6 text-center text-zinc-500 text-sm">У кампанії ще немає клієнтів</div>
                            ) : (
                                <div className="flex flex-col pb-2">
                                    {assignedCustomers.map((c) => (
                                        <div key={c.id} className="flex items-center gap-4 p-4 hover:bg-zinc-900/80 transition-colors border-b border-zinc-800/50 last:border-0 group">
                                            <Avatar className="h-8 w-8 border border-zinc-800">
                                                <AvatarFallback className="bg-zinc-800 text-zinc-400 text-xs">{getInitials(c.fullName)}</AvatarFallback>
                                            </Avatar>
                                            <div className="flex-1 overflow-hidden">
                                                <p className="text-sm font-medium text-zinc-100 truncate">{c.fullName}</p>
                                                <p className="text-xs text-zinc-400 truncate">{c.email}</p>
                                            </div>
                                            <Button
                                                type="button"
                                                variant="ghost"
                                                size="icon"
                                                className="h-8 w-8 text-zinc-500 hover:text-red-400 hover:bg-red-400/10 opacity-0 group-hover:opacity-100 transition-all"
                                                disabled={removingId === c.id}
                                                onClick={(e) => {
                                                    e.preventDefault()
                                                    e.stopPropagation()
                                                    handleRemove(c.id);
                                                }}
                                                title="Видалити з кампанії"
                                            >
                                                {removingId === c.id ? <span className="animate-spin text-xs">...</span> : <X className="h-4 w-4" />}
                                            </Button>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </ScrollArea>
                    </TabsContent>
                </Tabs>

                {error && (
                    <div className="px-6 pb-4 bg-zinc-950">
                        <div className="text-sm text-red-400 bg-red-950/30 p-3 rounded border border-red-900/50">
                            {error}
                        </div>
                    </div>
                )}
            </DialogContent>
        </Dialog>
    )
}