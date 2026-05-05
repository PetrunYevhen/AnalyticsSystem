import { useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { motion, AnimatePresence } from "framer-motion";
import { Mail, Lock, Loader2, Eye, EyeOff } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
    CardFooter,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";

import { loginUser, saveAuthSession, validateEmail } from "@/api/auth";

export default function Login() {
    const navigate = useNavigate();
    const location = useLocation();
    const redirectTo = location.state?.from?.pathname || "/";

    const [formData, setFormData] = useState({ email: "", password: "" });
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");

    const handleChange = (e) => {
        const { id, value } = e.target;
        setFormData((prev) => ({ ...prev, [id]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrorMessage("");

        const emailError = validateEmail(formData.email);
        if (emailError) return setErrorMessage(emailError);
        if (!formData.password) return setErrorMessage("Введіть пароль");

        setLoading(true);
        try {
            const result = await loginUser(formData);
            saveAuthSession(result);
            navigate(redirectTo, { replace: true });
        } catch (err) {
            setErrorMessage(err.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-muted/40 p-4 overflow-hidden">
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4 }}
                className="w-full max-w-md"
            >
                <Card className="shadow-lg">
                    <form onSubmit={handleSubmit} noValidate>
                        <CardHeader className="space-y-1 text-center">
                            <CardTitle className="text-2xl font-bold tracking-tight">
                                Вхід
                            </CardTitle>
                            <CardDescription>
                                Увійдіть у свій аналітичний простір
                            </CardDescription>
                        </CardHeader>

                        <CardContent className="space-y-4">
                            <AnimatePresence>
                                {errorMessage && (
                                    <motion.div
                                        initial={{ opacity: 0, height: 0 }}
                                        animate={{ opacity: 1, height: "auto" }}
                                        exit={{ opacity: 0, height: 0 }}
                                        className="p-3 text-sm bg-red-50 text-red-600 rounded-md border border-red-200"
                                        role="alert"
                                    >
                                        {errorMessage}
                                    </motion.div>
                                )}
                            </AnimatePresence>

                            <div className="space-y-2">
                                <label className="text-sm font-medium" htmlFor="email">
                                    Email
                                </label>
                                <div className="relative">
                                    <Mail className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
                                    <Input
                                        id="email"
                                        type="email"
                                        placeholder="admin@email.com"
                                        className="pl-9"
                                        value={formData.email}
                                        onChange={handleChange}
                                        autoComplete="email"
                                        autoFocus
                                        required
                                    />
                                </div>
                            </div>

                            <div className="space-y-2">
                                <div className="flex items-center justify-between">
                                    <label className="text-sm font-medium" htmlFor="password">
                                        Пароль
                                    </label>
                                    <Link
                                        to="/forgot-password"
                                        className="text-xs text-muted-foreground hover:text-primary hover:underline"
                                    >
                                        Забули пароль?
                                    </Link>
                                </div>
                                <div className="relative">
                                    <Lock className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
                                    <Input
                                        id="password"
                                        type={showPassword ? "text" : "password"}
                                        placeholder="••••••••"
                                        className="pl-9 pr-10"
                                        value={formData.password}
                                        onChange={handleChange}
                                        autoComplete="current-password"
                                        required
                                    />
                                    <button
                                        type="button"
                                        onClick={() => setShowPassword((v) => !v)}
                                        className="absolute right-3 top-2.5 text-muted-foreground hover:text-foreground transition-colors"
                                        aria-label={
                                            showPassword ? "Сховати пароль" : "Показати пароль"
                                        }
                                    >
                                        {showPassword ? (
                                            <EyeOff className="h-4 w-4" />
                                        ) : (
                                            <Eye className="h-4 w-4" />
                                        )}
                                    </button>
                                </div>
                            </div>
                        </CardContent>

                        <CardFooter className="flex flex-col gap-4">
                            <Button type="submit" className="w-full h-10" disabled={loading}>
                                {loading ? (
                                    <>
                                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                        Входимо...
                                    </>
                                ) : (
                                    "Увійти"
                                )}
                            </Button>

                            <p className="text-sm text-center text-muted-foreground">
                                Ще немає акаунту?{" "}
                                <Link
                                    to="/register"
                                    className="text-primary font-medium hover:underline"
                                >
                                    Зареєструватись
                                </Link>
                            </p>
                        </CardFooter>
                    </form>
                </Card>
            </motion.div>
        </div>
    );
}