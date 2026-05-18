import { useState } from "react"
import { Megaphone, CalendarDays, Banknote } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { useCreateCampaign } from "@/hooks/marketings/useCreateCampaign"

const initialForm = Object.freeze({
    name: "",
    channel: "",
    startDate: new Date().toISOString().split('T')[0],
    endDate: "",
    budget: ""
});

export function AddCampaignModal({ isOpen, onClose, onSuccess }) {
    const [form, setForm] = useState(initialForm);
    const [validationError, setValidationError] = useState(null);
    const { createCampaign, isPending, error: submitError } = useCreateCampaign();

    const handleChange = (field, value) => {
        setForm(prev => ({ ...prev, [field]: value }));
        if (validationError) setValidationError(null);
    };

    const handleModalClose = (open) => {
        if (!open) {
            setForm(initialForm);
            setValidationError(null);
            onClose();
        }
    };

    const validate = () => {
        if (!form.name.trim() || form.name.trim().length < 2) return "Назва має містити мінімум 2 символи.";
        if (!form.channel.trim() || form.channel.trim().length < 2) return "Джерело (Channel) обов'язкове.";
        if (!form.startDate) return "Дата початку обов'язкова.";
        if (form.endDate && new Date(form.endDate) < new Date(form.startDate)) return "Дата завершення не може бути раніше дати початку.";
        if (form.budget === "" || Number(form.budget) < 0) return "Бюджет не може бути від'ємним.";
        return null;
    };

    const handleSubmit = async () => {
        const err = validate();
        if (err) return setValidationError(err);

        try {
            const payload = {
                name: form.name.trim(),
                channel: form.channel.trim(),
                startDate: form.startDate,
                endDate: form.endDate || null,
                budget: Number(form.budget)
            };

            await createCampaign(payload);
            setForm(initialForm);
            if (onSuccess) onSuccess();
            onClose();
        } catch {
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={handleModalClose}>
            <DialogContent className="sm:max-w-[600px] p-0 overflow-hidden">
                <DialogHeader className="p-6 pb-4 border-b bg-card">
                    <DialogTitle>Нова рекламна кампанія</DialogTitle>
                </DialogHeader>

                <div className="p-6 space-y-8 bg-card">
                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase text-muted-foreground tracking-wider">
                            <Megaphone className="h-4 w-4" /> Основна інформація
                        </h3>
                        <div className="grid grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <Label htmlFor="name">Назва кампанії <span className="text-destructive">*</span></Label>
                                <Input id="name" value={form.name} onChange={(e) => handleChange('name', e.target.value)} placeholder="Напр. Black Friday" disabled={isPending} />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="channel">Джерело (Channel) <span className="text-destructive">*</span></Label>
                                <Input id="channel" value={form.channel} onChange={(e) => handleChange('channel', e.target.value)} placeholder="Напр. Facebook" disabled={isPending} />
                            </div>
                        </div>
                    </section>

                    <section className="space-y-4">
                        <h3 className="flex items-center gap-2 text-sm font-semibold uppercase text-muted-foreground tracking-wider">
                            <CalendarDays className="h-4 w-4" /> Дати та фінанси
                        </h3>
                        <div className="grid grid-cols-3 gap-4">
                            <div className="space-y-2">
                                <Label htmlFor="startDate">Початок <span className="text-destructive">*</span></Label>
                                <Input id="startDate" type="date" value={form.startDate} onChange={(e) => handleChange('startDate', e.target.value)} disabled={isPending} />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="endDate">Завершення</Label>
                                <Input id="endDate" type="date" value={form.endDate} onChange={(e) => handleChange('endDate', e.target.value)} disabled={isPending} />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="budget" className="flex items-center gap-1">
                                    <Banknote className="h-3 w-3" /> Бюджет (₴)
                                </Label>
                                <Input id="budget" type="number" step="0.01" value={form.budget} onChange={(e) => handleChange('budget', e.target.value)} disabled={isPending} />
                            </div>
                        </div>
                    </section>

                    {(validationError || submitError) && (
                        <Alert variant="destructive">
                            <AlertDescription>{validationError || submitError}</AlertDescription>
                        </Alert>
                    )}
                </div>

                <div className="flex justify-end gap-2 p-4 border-t bg-muted/20">
                    <Button type="button" variant="ghost" onClick={() => handleModalClose(false)} disabled={isPending}>
                        Скасувати
                    </Button>
                    <Button type="button" onClick={handleSubmit} disabled={isPending}>
                        {isPending ? "Збереження..." : "Створити кампанію"}
                    </Button>
                </div>
            </DialogContent>
        </Dialog>
    );
}