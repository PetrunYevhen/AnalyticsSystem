import { useState } from "react"
import { Plus } from "lucide-react"
import { Dialog, DialogContent, DialogTrigger, DialogTitle } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { CreateOrderForm } from "./CreateOrderForm"
import { VisuallyHidden } from "@radix-ui/react-visually-hidden"

export function AddOrderModal({ onImportSuccess }) {
    const [open, setOpen] = useState(false)

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
                <Button variant="outline">
                    <Plus className="h-4 w-4 mr-2" />
                    Створити замовлення
                </Button>
            </DialogTrigger>

            <DialogContent className="w-[95vw] max-w-[95vw] sm:max-w-2xl md:max-w-3xl lg:max-w-4xl max-h-[90vh] overflow-y-auto p-0 bg-background">                <VisuallyHidden>
                    <DialogTitle>Створити замовлення</DialogTitle>
                </VisuallyHidden>

                <CreateOrderForm
                    onSuccess={() => {
                        setOpen(false);
                        if (onImportSuccess) onImportSuccess();
                    }}
                />
            </DialogContent>
        </Dialog>
    )
}