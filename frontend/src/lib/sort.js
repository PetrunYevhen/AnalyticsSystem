const DESC_FIRST = new Set([
    "LastOrderDate", "TotalRevenue",
    "OrderDate", "TotalAmount",
    "TransactionDate",
])

export const defaultDirectionFor = (key) => DESC_FIRST.has(key) ? "Desc" : "Asc"
