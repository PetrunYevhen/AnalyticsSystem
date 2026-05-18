import { ResponsiveContainer, ComposedChart, Line, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from "recharts"

export function LtvCacChart({ data }) {
    if (!data || data.length === 0) {
        return <div className="text-sm text-muted-foreground p-4 border rounded-xl bg-card">Немає даних для відображення</div>;
    }

    const CustomTooltip = ({ active, payload, label }) => {
        if (active && payload && payload.length) {
            return (
                <div className="bg-zinc-950 border border-zinc-800 p-3 rounded-lg shadow-lg">
                    <p className="text-sm text-zinc-400 mb-1">{label}</p>
                    {payload.map((p, i) => (
                        <p key={i} className="text-sm font-bold" style={{ color: p.color }}>
                            {p.name}: ₴{Number(p.value).toLocaleString("uk-UA")}
                        </p>
                    ))}
                </div>
            )
        }
        return null
    }

    return (
        <div className="p-4 border rounded-xl bg-card text-card-foreground shadow-sm w-full">
            <h4 className="text-sm font-medium mb-4">Динаміка LTV та CAC по місяцях (₴)</h4>
            <div className="h-[300px] w-full">
                <ResponsiveContainer width="100%" height="100%">
                    <ComposedChart data={data} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
                        <defs>
                            <linearGradient id="colorLtv" x1="0" y1="0" x2="0" y2="1">
                                <stop offset="5%" stopColor="#10b981" stopOpacity={0.3} />
                                <stop offset="95%" stopColor="#10b981" stopOpacity={0} />
                            </linearGradient>
                        </defs>

                        <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#27272a" />

                        <XAxis
                            dataKey="date"
                            stroke="#71717a"
                            fontSize={12}
                            tickLine={false}
                            axisLine={false}
                            dy={10}
                        />

                        <YAxis
                            stroke="#71717a"
                            fontSize={12}
                            tickLine={false}
                            axisLine={false}
                            tickFormatter={(value) => `₴${value}`}
                            width={80}
                        />

                        <Tooltip content={<CustomTooltip />} cursor={{ stroke: '#3f3f46', strokeWidth: 1, strokeDasharray: '4 4' }} />

                        <Legend wrapperStyle={{ fontSize: "12px", paddingTop: "10px", color: "#71717a" }} />

                        <Bar
                            dataKey="cac"
                            name="CAC (Вартість залучення)"
                            fill="#ef4444"
                            radius={[4, 4, 0, 0]}
                            barSize={32}
                        />

                        <Line
                            type="monotone"
                            dataKey="ltv"
                            name="LTV (Дохід)"
                            stroke="#10b981"
                            strokeWidth={2}
                            dot={{ r: 4, fill: "#10b981", stroke: "#18181b", strokeWidth: 2 }}
                            activeDot={{ r: 6, fill: "#10b981", stroke: "#18181b", strokeWidth: 2 }}
                        />
                    </ComposedChart>
                </ResponsiveContainer>
            </div>
        </div>
    )
}