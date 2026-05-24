import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { useProfile } from "@/hooks/settings/useProfile"
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Loader2, User, Mail, Shield, LogOut, CheckCircle2 } from "lucide-react"

export default function ProfileTab() {
    const navigate = useNavigate()
    const { profile, loading, error, isSaving, fetchProfile, changePassword } = useProfile()

    const [passwords, setPasswords] = useState({ current: "", next: "" })
    const [pwdStatus, setPwdStatus] = useState({ error: null, success: false })

    useEffect(() => {
        fetchProfile()
    }, [fetchProfile])

    const handleLogout = () => {
        localStorage.removeItem("token")
        navigate("/login", { replace: true })
    }

    const handleSavePassword = async () => {
        setPwdStatus({ error: null, success: false })
        const res = await changePassword({
            currentPassword: passwords.current,
            newPassword: passwords.next
        })

        if (res.success) {
            setPasswords({ current: "", next: "" })
            setPwdStatus({ error: null, success: true })
            setTimeout(() => setPwdStatus(s => ({ ...s, success: false })), 3000)
        } else {
            setPwdStatus({ error: res.error, success: false })
        }
    }

    return (
        <Card className="border-zinc-800 bg-zinc-950/50">
            <CardHeader className="border-b border-zinc-800 pb-6">
                <CardTitle className="text-xl">Особистий профіль</CardTitle>
                <CardDescription className="text-zinc-400">
                    Для зміни імені або пошти зверніться до адміністратора робочого простору.
                </CardDescription>
            </CardHeader>
            <CardContent className="space-y-8 pt-6">
                {loading ? (
                    <div className="flex justify-center items-center py-12">
                        <Loader2 className="h-6 w-6 animate-spin text-zinc-500" />
                    </div>
                ) : error ? (
                    <div className="p-4 text-sm text-red-400 bg-red-950/30 border border-red-900/50 rounded-md">
                        {error}
                    </div>
                ) : (
                    <>
                        <div className="space-y-4">
                            <h3 className="text-sm font-medium text-zinc-400 flex items-center gap-2">
                                <User className="h-4 w-4" /> Основна інформація
                            </h3>
                            <div className="grid md:grid-cols-2 gap-6 bg-zinc-900/50 p-4 rounded-lg border border-zinc-800/50">
                                <div className="space-y-2">
                                    <Label className="text-zinc-500">Повне ім'я</Label>
                                    <Input value={profile.fullName} readOnly className="bg-zinc-950 border-zinc-800 text-zinc-300 cursor-not-allowed focus-visible:ring-0" />
                                </div>
                                <div className="space-y-2">
                                    <Label className="text-zinc-500">Email</Label>
                                    <Input value={profile.email} readOnly className="bg-zinc-950 border-zinc-800 text-zinc-300 cursor-not-allowed focus-visible:ring-0" />
                                </div>
                                <div className="space-y-2">
                                    <Label className="text-zinc-500">Роль</Label>
                                    <Input value={profile.role || "Користувач"} readOnly className="bg-zinc-950 border-zinc-800 text-zinc-300 cursor-not-allowed focus-visible:ring-0" />
                                </div>
                            </div>
                        </div>

                        <div className="space-y-4">
                            <h3 className="text-sm font-medium text-zinc-400 flex items-center gap-2">
                                <Shield className="h-4 w-4" /> Безпека та пароль
                            </h3>
                            <div className="grid md:grid-cols-2 gap-6 bg-zinc-900/50 p-4 rounded-lg border border-zinc-800/50">
                                <div className="space-y-2">
                                    <Label className="text-zinc-300">Поточний пароль</Label>
                                    <Input
                                        type="password"
                                        className="bg-zinc-950 border-zinc-800 text-zinc-100"
                                        value={passwords.current}
                                        onChange={(e) => setPasswords(p => ({ ...p, current: e.target.value }))}
                                    />
                                </div>
                                <div className="space-y-2">
                                    <Label className="text-zinc-300">Новий пароль</Label>
                                    <Input
                                        type="password"
                                        className="bg-zinc-950 border-zinc-800 text-zinc-100"
                                        value={passwords.next}
                                        onChange={(e) => setPasswords(p => ({ ...p, next: e.target.value }))}
                                    />
                                </div>
                                <div className="md:col-span-2 flex items-center justify-between">
                                    <div className="text-sm">
                                        {pwdStatus.error && <span className="text-red-400">{pwdStatus.error}</span>}
                                        {pwdStatus.success && <span className="text-emerald-400 flex items-center gap-1"><CheckCircle2 className="h-4 w-4"/> Пароль успішно оновлено</span>}
                                    </div>
                                    <Button
                                        onClick={handleSavePassword}
                                        disabled={isSaving || !passwords.current || !passwords.next}
                                        className="bg-zinc-100 text-zinc-900 hover:bg-zinc-200"
                                    >
                                        {isSaving ? <><Loader2 className="h-4 w-4 mr-2 animate-spin" />Збереження...</> : "Оновити пароль"}
                                    </Button>
                                </div>
                            </div>
                        </div>
                    </>
                )}
            </CardContent>
            <CardFooter className="border-t border-zinc-800 bg-zinc-900/20 px-6 py-4 flex justify-end">
                <Button variant="outline" onClick={handleLogout} className="gap-2 border-red-900/30 text-red-400 hover:bg-red-950/30 hover:text-red-300">
                    <LogOut className="h-4 w-4" /> Вийти з акаунту
                </Button>
            </CardFooter>
        </Card>
    )
}