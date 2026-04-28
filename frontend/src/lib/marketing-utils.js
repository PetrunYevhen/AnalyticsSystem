export const STUB_CHART = [
    { label: "Бер", spend: 12400, revenue: 38200 },
    { label: "Кві", spend: 15800, revenue: 44500 },
    { label: "Тра", spend: 11200, revenue: 31000 },
    { label: "Чер", spend: 18900, revenue: 57300 },
    { label: "Лип", spend: 21000, revenue: 63800 },
    { label: "Сер", spend: 17500, revenue: 52400 },
    { label: "Вер", spend: 23100, revenue: 71200 },
    { label: "Жов", spend: 19800, revenue: 60100 },
    { label: "Лис", spend: 25400, revenue: 78900 },
    { label: "Гру", spend: 28700, revenue: 89500 },
    { label: "Січ", spend: 22300, revenue: 68400 },
    { label: "Лют", spend: 26900, revenue: 84200 },
]

export const fmtUAH = (v) => `₴ ${(v ?? 0).toLocaleString('uk-UA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
export const fmtNum = (v) => (v ?? 0).toLocaleString('uk-UA')
export const fmtPct = (v) => `${(v ?? 0).toFixed(2)}%`

export const ltvCacColor = (ratio) => {
    if (ratio >= 3) return "text-emerald-600 dark:text-emerald-400"
    if (ratio >= 1) return "text-amber-600 dark:text-amber-400"
    return "text-red-500"
}

export const ltvCacLabel = (ratio) => {
    if (ratio >= 3) return "Здорова економіка"
    if (ratio >= 1) return "Маржинально"
    return "Збитково"
}

export const retentionColor = (rate) => {
    if (rate >= 70) return "text-emerald-600 dark:text-emerald-400"
    if (rate >= 40) return "text-amber-600 dark:text-amber-400"
    return "text-red-500"
}

export const romiColor = (romi) => {
    if (romi > 0) return "text-emerald-600 dark:text-emerald-400"
    if (romi < 0) return "text-red-500"
    return "text-foreground"
}

export const churnColor = (rate) => {
    if (rate <= 5)  return "text-emerald-600 dark:text-emerald-400"
    if (rate <= 15) return "text-amber-600 dark:text-amber-400"
    return "text-red-500"
}