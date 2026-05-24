import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { User, Building, CreditCard } from "lucide-react"

import ProfileTab from "./tabs/profileTab"
import WorkspaceTab from "./tabs/workspaceTab"

function BillingTab() {
    return (
        <Card>
            <CardHeader>
                <CardTitle>Білінг</CardTitle>
            </CardHeader>
            <CardContent>
                <p className="text-sm text-muted-foreground">Розділ у розробці.</p>
            </CardContent>
        </Card>
    )
}

export default function Settings() {
    return (
        <div className="px-0 md:px-2 py-6 space-y-6 w-full min-w-0 overflow-x-hidden">
            <div>
                <h2 className="text-3xl font-bold tracking-tight">Налаштування</h2>
                <p className="text-muted-foreground">
                    Керуйте параметрами вашого профілю, робочого простору та білінгу.
                </p>
            </div>

            <Tabs defaultValue="profile" className="space-y-6">
                <TabsList className="flex w-full p-1 gap-1">
                    <TabsTrigger value="profile" className="flex-1 flex items-center justify-center gap-2 py-2">
                        <User className="h-4 w-4" /> Мій профіль
                    </TabsTrigger>
                    <TabsTrigger value="workspace" className="flex-1 flex items-center justify-center gap-2 py-2">
                        <Building className="h-4 w-4" /> Команда
                    </TabsTrigger>
                    <TabsTrigger value="billing" className="flex-1 flex items-center justify-center gap-2 py-2">
                        <CreditCard className="h-4 w-4" /> Білінг
                    </TabsTrigger>
                </TabsList>

                <TabsContent value="profile"><ProfileTab /></TabsContent>
                <TabsContent value="workspace"><WorkspaceTab /></TabsContent>
                <TabsContent value="billing"><BillingTab /></TabsContent>
            </Tabs>
        </div>
    )
}