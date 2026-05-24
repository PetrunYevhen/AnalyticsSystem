import { useState, useEffect } from "react"
import { useTeam } from "@/hooks/settings/useTeam"
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Loader2, UserPlus, Trash2, ShieldAlert } from "lucide-react"
import InviteUserModal from "../components/InviteUserModal"
import {useTenant} from "@/hooks/settings/useTenant";
import {Input} from "@/components/ui/input";
import {Label} from "@/components/ui/label"
import { useAuth } from "@/hooks/auth/useAuth"
import {StatusBadge} from "@/components/StatusBadge";

export default function WorkspaceTab() {
    const { members, loading, error, fetchTeam, inviteMember, removeMember } = useTeam()
    const [showModal, setShowModal] = useState(false)
    const [actionError, setActionError] = useState(null)
    const [deletingId, setDeletingId] = useState(null)
    const { tenant, loading: tenantLoading, fetchTenant } = useTenant()
    const { isAdmin } = useAuth()


    useEffect(() => {
        fetchTenant()
        fetchTeam()
    }, [fetchTenant, fetchTeam])

    const handleInvite = async (form) => {
        setActionError(null)
        const res = await inviteMember(form)
        if (!res.success) {
            setActionError(res.error)
            throw new Error(res.error)
        }
    }

    const handleRemove = async (userId) => {
        setDeletingId(userId)
    }

    const confirmRemove = async (userId) => {
        setActionError(null)
        const res = await removeMember(userId)
        if (!res.success) setActionError(res.error)
        setDeletingId(null)
    }

    return (
        <>
            {showModal && (
                <InviteUserModal
                    onClose={() => setShowModal(false)}
                    onInvite={handleInvite}
                />
            )}
            <Card className="border-zinc-800 bg-zinc-950/50">
                <CardHeader className="border-b border-zinc-800 pb-6">
                    <CardTitle className="text-xl">Організація</CardTitle>
                </CardHeader>
                <CardContent className="pt-6 space-y-4">
                    {tenantLoading ? (
                        <Loader2 className="h-5 w-5 animate-spin text-zinc-500"/>
                    ) : (
                        <div
                            className="grid md:grid-cols-2 gap-6 bg-zinc-900/50 p-4 rounded-lg border border-zinc-800/50">
                            <div className="space-y-2">
                                <Label className="text-zinc-500">Назва компанії</Label>
                                <Input value={tenant?.companyName ?? "—"} readOnly
                                       className="bg-zinc-950 border-zinc-800 text-zinc-300 cursor-not-allowed focus-visible:ring-0"/>
                            </div>
                            <div className="space-y-2">
                                <Label className="text-zinc-500">ID організації</Label>
                                <Input value={tenant?.id ?? "—"} readOnly
                                       className="bg-zinc-950 border-zinc-800 text-zinc-300 font-mono text-xs cursor-not-allowed focus-visible:ring-0"/>
                            </div>
                            <div className="space-y-2">
                                <Label className="text-zinc-500">API ключ</Label>
                                <Input value={tenant?.apiKeyPrefix ? `${tenant.apiKeyPrefix}••••••••` : "—"} readOnly
                                       className="bg-zinc-950 border-zinc-800 text-zinc-300 font-mono text-xs cursor-not-allowed focus-visible:ring-0"/>
                            </div>
                        </div>
                    )}
                </CardContent>
            </Card>

            <Card className="border-zinc-800 bg-zinc-950/50">
                <CardHeader className="border-b border-zinc-800 pb-6">
                    <div className="flex items-start justify-between flex-wrap gap-4">
                        <div className="flex items-center justify-between w-full flex-wrap gap-4">
                            <div>
                                <CardTitle className="text-xl">Команда</CardTitle>
                                <CardDescription className="text-zinc-400">Керування доступом до робочого простору.</CardDescription>
                            </div>
                            {isAdmin && (
                                <Button onClick={() => setShowModal(true)} className="gap-2 bg-emerald-600 hover:bg-emerald-700 text-white">
                                    <UserPlus className="h-4 w-4"/> Запросити
                                </Button>
                            )}
                        </div>
                    </div>
                </CardHeader>
                <CardContent className="p-0">
                    {actionError && (
                        <div
                            className="m-4 p-3 text-sm text-red-400 bg-red-950/30 border border-red-900/50 rounded-md flex items-center gap-2">
                            <ShieldAlert className="h-4 w-4"/> {actionError}
                        </div>
                    )}

                    {loading ? (
                        <div className="flex justify-center items-center py-16">
                            <Loader2 className="h-6 w-6 animate-spin text-zinc-500"/>
                        </div>
                    ) : error ? (
                        <div className="p-8 text-center text-red-400">{error}</div>
                    ) : members.length === 0 ? (
                        <div className="flex flex-col items-center justify-center py-20 text-zinc-500 gap-3">
                            <div
                                className="h-12 w-12 rounded-full bg-zinc-900 flex items-center justify-center border border-zinc-800">
                                <UserPlus className="h-5 w-5 opacity-50"/>
                            </div>
                            <p className="text-sm">Ви єдиний учасник цього простору.</p>
                        </div>
                    ) : (
                        <div className="divide-y divide-zinc-800/50">
                            {members.map((member) => (
                                <div
                                    key={member.id}
                                    className="flex flex-col sm:flex-row sm:items-center justify-between p-4 sm:px-6 hover:bg-zinc-900/30 transition-colors gap-4"
                                >
                                    <div className="flex items-center gap-4">
                                        <div className="h-10 w-10 rounded-full bg-zinc-800 border border-zinc-700 flex items-center justify-center text-sm font-medium text-zinc-300 shrink-0">
                                            {member.fullName?.charAt(0)?.toUpperCase() ?? "?"}
                                        </div>
                                        <div className="min-w-0">
                                            <p className="text-sm font-medium text-zinc-200 truncate">{member.fullName}</p>
                                            <div className="flex items-center gap-2 flex-wrap">
                                                <p className="text-xs text-zinc-500 truncate">{member.email}</p>
                                                {member.createdAt && (
                                                    <span className="text-xs text-zinc-600">
                                · {new Date(member.createdAt).toLocaleDateString("uk-UA")}
                            </span>
                                                )}
                                            </div>
                                        </div>
                                    </div>

                                    <div className="flex items-center justify-between sm:justify-end gap-4 w-full sm:w-auto">
                                        <StatusBadge status={member.role} type="role" />

                                        {isAdmin && (
                                            deletingId === member.id ? (
                                                <div className="flex items-center gap-2">
                                                    <Button size="sm" variant="ghost" className="h-8 text-xs text-zinc-400"
                                                            onClick={() => setDeletingId(null)}>Скасувати</Button>
                                                    <Button size="sm" variant="destructive" className="h-8 text-xs"
                                                            onClick={() => confirmRemove(member.id)}>Видалити</Button>
                                                </div>
                                            ) : (
                                                <Button variant="ghost" size="icon"
                                                        className="h-8 w-8 text-zinc-500 hover:text-red-400 hover:bg-red-950/30 shrink-0"
                                                        onClick={() => handleRemove(member.id)}
                                                >
                                                    <Trash2 className="h-4 w-4"/>
                                                </Button>
                                            )
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>
        </>
    );
}