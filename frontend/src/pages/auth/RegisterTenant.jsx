import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { motion, AnimatePresence } from "framer-motion";
import {
  Building,
  Mail,
  Lock,
  Loader2,
  User,
  CheckCircle2,
  Eye,
  EyeOff,
} from "lucide-react";

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

import {
  registerTenant,
  saveAuthSession,
  validateEmail,
  validatePassword,
} from "@/api/auth";

const INITIAL_FORM = {
  companyName: "",
  fullName: "",
  email: "",
  password: "",
};

export default function RegisterTenant() {
  const navigate = useNavigate();

  const [formData, setFormData] = useState(INITIAL_FORM);
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
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

    const passwordError = validatePassword(formData.password);
    if (passwordError) return setErrorMessage(passwordError);

    setLoading(true);
    try {
      const result = await registerTenant(formData);
      saveAuthSession(result);
      setIsSuccess(true);
      setTimeout(() => navigate("/"), 3000);
    } catch (err) {
      setErrorMessage(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
      <div className="min-h-screen flex items-center justify-center bg-muted/40 p-4 overflow-hidden">
        <AnimatePresence mode="wait">
          {!isSuccess ? (
              <motion.div
                  key="register-form"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, scale: 0.95 }}
                  transition={{ duration: 0.4 }}
                  className="w-full max-w-md"
              >
                <Card className="shadow-lg">
                  <form onSubmit={handleSubmit} noValidate>
                    <CardHeader className="space-y-1 text-center">
                      <CardTitle className="text-2xl font-bold tracking-tight">
                        Реєстрація
                      </CardTitle>
                      <CardDescription>
                        Створіть свій аналітичний простір
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
                        <label className="text-sm font-medium" htmlFor="companyName">
                          Назва компанії
                        </label>
                        <div className="relative">
                          <Building className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
                          <Input
                              id="companyName"
                              placeholder="My Store"
                              className="pl-9"
                              value={formData.companyName}
                              onChange={handleChange}
                              autoComplete="organization"
                              required
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <label className="text-sm font-medium" htmlFor="fullName">
                          Ваше ім'я
                        </label>
                        <div className="relative">
                          <User className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
                          <Input
                              id="fullName"
                              placeholder="Адмін"
                              className="pl-9"
                              value={formData.fullName}
                              onChange={handleChange}
                              autoComplete="name"
                              required
                          />
                        </div>
                      </div>

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
                              required
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <label className="text-sm font-medium" htmlFor="password">
                          Пароль
                        </label>
                        <div className="relative">
                          <Lock className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
                          <Input
                              id="password"
                              type={showPassword ? "text" : "password"}
                              placeholder="••••••••"
                              className="pl-9 pr-10"
                              value={formData.password}
                              onChange={handleChange}
                              autoComplete="new-password"
                              required
                              minLength={8}
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
                        <p className="text-xs text-muted-foreground">
                          Мінімум 8 символів, одна велика літера, одна цифра
                        </p>
                      </div>
                    </CardContent>

                    <CardFooter className="flex flex-col gap-4">
                      <Button type="submit" className="w-full h-10" disabled={loading}>
                        {loading ? (
                            <>
                              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                              Створюємо простір...
                            </>
                        ) : (
                            "Зареєструватись"
                        )}
                      </Button>

                      <p className="text-sm text-center text-muted-foreground">
                        Вже маєте акаунт?{" "}
                        <Link
                            to="/login"
                            className="text-primary font-medium hover:underline"
                        >
                          Увійти
                        </Link>
                      </p>
                    </CardFooter>
                  </form>
                </Card>
              </motion.div>
          ) : (
              <motion.div
                  key="success-message"
                  initial={{ opacity: 0, scale: 0.8 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ type: "spring", damping: 15 }}
                  className="w-full max-w-md text-center space-y-4"
              >
                <div className="flex justify-center">
                  <motion.div
                      initial={{ rotate: -45, scale: 0 }}
                      animate={{ rotate: 0, scale: 1 }}
                      transition={{ delay: 0.2, type: "spring" }}
                  >
                    <CheckCircle2 className="h-24 w-24 text-green-500" />
                  </motion.div>
                </div>

                <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.4 }}
                >
                  <h2 className="text-3xl font-bold text-foreground">Вітаємо!</h2>
                  <p className="text-muted-foreground mt-2">
                    Організацію <strong>{formData.companyName}</strong> успішно
                    створено.
                  </p>
                  <p className="text-sm text-muted-foreground/60 mt-4 italic">
                    Перенаправляємо на дашборд...
                  </p>
                </motion.div>

                <div className="w-48 h-1.5 bg-muted rounded-full mx-auto mt-6 overflow-hidden">
                  <motion.div
                      className="h-full bg-green-500"
                      initial={{ width: 0 }}
                      animate={{ width: "100%" }}
                      transition={{ duration: 2.5 }}
                  />
                </div>
              </motion.div>
          )}
        </AnimatePresence>
      </div>
  );
}