import { format } from "date-fns"

export function toLocalDayStartIso(date) {
    if (!date) return undefined
    return format(date, "yyyy-MM-dd'T'00:00:00XXX")
}

export function toLocalDayEndExclusiveIso(date) {
    if (!date) return undefined
    const next = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1)
    return format(next, "yyyy-MM-dd'T'00:00:00XXX")
}
