import { useState } from "react"
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Loader2, X } from "lucide-react"

export default function InviteUserModal({ onClose, onInvite }) {
    const [form, setForm] = useState({ fullName: "", email: "", password: "", role: "Analyst" })
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)


    const handleSubmit = async () => {
        if (!form.fullName || !form.email || !form.password) {
            setError("Заповніть всі поля")
            return
        }
        setLoading(true)
        setError(null)
        try {
            await onInvite(form)
            onClose()
        } catch (err) {
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    return (
        <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm"
            onClick={(e) => e.target === e.currentTarget && onClose()}
        >
            <Card className="w-full max-w-md mx-4 shadow-xl">
                <CardHeader className="flex flex-row items-start justify-between space-y-0 pb-4">
                    <div>
                        <CardTitle>Додати користувача</CardTitle>
                        <CardDescription className="mt-1">
                            Додати нового користувача
                        </CardDescription>
                    </div>
                    <Button variant="ghost" size="icon" onClick={onClose} className="h-8 w-8 -mt-1">
                        <X className="h-4 w-4" />
                    </Button>
                </CardHeader>
                <CardContent className="space-y-4">
                    {error && (
                        <div className="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
                            {error}
                        </div>
                    )}
                    <div className="grid gap-2">
                        <Label>Повне ім'я</Label>
                        <Input
                            placeholder="Іван Петренко"
                            value={form.fullName}
                            onChange={(e) => setForm(p => ({ ...p, fullName: e.target.value }))}
                        />
                    </div>
                    <div className="grid gap-2">
                        <Label>Email</Label>
                        <Input
                            type="email"
                            placeholder="ivan@company.com"
                            value={form.email}
                            onChange={(e) => setForm(p => ({ ...p, email: e.target.value }))}
                        />
                    </div>
                    <div className="grid gap-2">
                        <Label>Початковий пароль</Label>
                        <Input
                            type="password"
                            placeholder="Мінімум 8 символів"
                            value={form.password}
                            onChange={(e) => setForm(p => ({ ...p, password: e.target.value }))}
                        />
                    </div>
                    <div className="grid gap-2">
                        <Label>Роль</Label>
                        <select
                            value={form.role}
                            onChange={(e) => setForm(p => ({ ...p, role: e.target.value }))}
                            className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                        >
                            <option value="Analyst">Analyst — повний доступ до аналітики</option>
                            <option value="Viewer">Viewer — тільки перегляд</option>
                        </select>
                    </div>
                </CardContent>
                <CardFooter className="flex justify-end gap-2 border-t pt-4">
                    <Button variant="outline" onClick={onClose}>Скасувати</Button>
                    <Button onClick={handleSubmit} disabled={loading}>
                        {loading
                            ? <><Loader2 className="h-4 w-4 mr-2 animate-spin" />Створення...</>
                            : "Створити"
                        }
                    </Button>
                </CardFooter>
            </Card>
        </div>
    )
}