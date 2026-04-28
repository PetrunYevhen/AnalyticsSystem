import { useState } from "react"
import { format, isSameDay } from "date-fns"
import { uk } from "date-fns/locale"
import { Calendar as CalendarIcon } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Calendar } from "@/components/ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"

export function DateRangePicker({ value, onChange }) {
    const [open, setOpen] = useState(false)

    const label = value?.from
        ? value.to
            ? `${format(value.from, "dd.MM.yyyy")} — ${format(value.to, "dd.MM.yyyy")}`
            : format(value.from, "dd.MM.yyyy")
        : "Оберіть період"

    const handleSelect = (range, triggerDate) => {
        if (value?.from && value?.to) {
            onChange({ from: triggerDate, to: undefined })
            return
        }
        if (value?.from && !value?.to && isSameDay(value.from, triggerDate)) {
            onChange({ from: value.from, to: triggerDate })
            setOpen(false)
            return
        }
        onChange(range)
        if (range?.from && range?.to) setOpen(false)
    }

    return (
        <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
                <Button variant="outline" className="w-fit gap-2">
                    <CalendarIcon className="h-4 w-4" />
                    {label}
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="end">
                <Calendar
                    mode="range"
                    selected={value}
                    onSelect={handleSelect}
                    defaultMonth={value?.from}
                    numberOfMonths={2}
                    locale={uk}
                />
            </PopoverContent>
        </Popover>
    )
}