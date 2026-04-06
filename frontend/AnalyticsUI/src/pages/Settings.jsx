import { useState } from "react"
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import { Building, Bell, Shield, Save, Mail } from "lucide-react"

export default function Settings() {
    const [loading, setLoading] = useState(false)

    const handleSave = () => {
        setLoading(true)
        setTimeout(() => setLoading(false), 1000)
    }

    return (
        <div className="p-6 md:p-10 space-y-6 max-w-5xl mx-auto">
            <div>
                <h2 className="text-3xl font-bold tracking-tight">Налаштування</h2>
                <p className="text-muted-foreground">Керуйте параметрами вашої організації та безпекою акаунту.</p>
            </div>

            <Tabs defaultValue="general" className="space-y-6">
                <TabsList className="grid w-full grid-cols-3 lg:w-[400px]">
                    <TabsTrigger value="general" className="gap-2">
                        <Building className="h-4 w-4" /> Загальні
                    </TabsTrigger>
                    <TabsTrigger value="notifications" className="gap-2">
                        <Bell className="h-4 w-4" /> Сповіщення
                    </TabsTrigger>
                    <TabsTrigger value="security" className="gap-2">
                        <Shield className="h-4 w-4" /> Безпека
                    </TabsTrigger>
                </TabsList>

                <TabsContent value="general">
                    <Card>
                        <CardHeader>
                            <CardTitle>Профіль організації</CardTitle>
                            <CardDescription>Ці дані відображатимуться у звітах та інвойсах.</CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="grid gap-2">
                                <Label htmlFor="name">Назва компанії</Label>
                                <Input id="name" defaultValue="My SaaS Company" />
                            </div>
                            <div className="grid gap-2">
                                <Label htmlFor="email">Контактний Email</Label>
                                <Input id="email" type="email" defaultValue="admin@company.com" />
                            </div>
                            <div className="grid gap-2">
                                <Label htmlFor="website">Веб-сайт</Label>
                                <Input id="website" placeholder="https://example.com" />
                            </div>
                        </CardContent>
                        <CardFooter className="border-t px-6 py-4">
                            <Button onClick={handleSave} disabled={loading}>
                                {loading ? "Зберігання..." : "Зберегти зміни"}
                            </Button>
                        </CardFooter>
                    </Card>
                </TabsContent>
                <TabsContent value="notifications">
                    <Card>
                        <CardHeader>
                            <CardTitle>Email сповіщення</CardTitle>
                            <CardDescription>Оберіть, про які події ви хочете отримувати листи.</CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-6">
                            <div className="flex items-center justify-between space-x-2">
                                <div className="flex flex-col space-y-1">
                                    <Label>Нові замовлення</Label>
                                    <span className="text-xs text-muted-foreground">Отримувати звіт про кожну нову транзакцію.</span>
                                </div>
                                <Switch defaultChecked />
                            </div>
                            <div className="flex items-center justify-between space-x-2">
                                <div className="flex flex-col space-y-1">
                                    <Label>Щотижнева аналітика</Label>
                                    <span className="text-xs text-muted-foreground">Отримувати підсумок роботи тенанта за тиждень.</span>
                                </div>
                                <Switch defaultChecked />
                            </div>
                            <div className="flex items-center justify-between space-x-2">
                                <div className="flex flex-col space-y-1">
                                    <Label>Маркетингові звіти</Label>
                                    <span className="text-xs text-muted-foreground">Сповіщення про зміну вартості кліку (CPC).</span>
                                </div>
                                <Switch />
                            </div>
                        </CardContent>
                    </Card>
                </TabsContent>

                <TabsContent value="security">
                    <Card>
                        <CardHeader>
                            <CardTitle>Пароль</CardTitle>
                            <CardDescription>Змініть свій пароль для доступу до панелі.</CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="grid gap-2">
                                <Label htmlFor="current">Поточний пароль</Label>
                                <Input id="current" type="password" />
                            </div>
                            <div className="grid gap-2">
                                <Label htmlFor="new">Новий пароль</Label>
                                <Input id="new" type="password" />
                            </div>
                        </CardContent>
                        <CardFooter className="border-t px-6 py-4">
                            <Button variant="destructive">Оновити пароль</Button>
                        </CardFooter>
                    </Card>
                </TabsContent>
            </Tabs>
        </div>
    )
}