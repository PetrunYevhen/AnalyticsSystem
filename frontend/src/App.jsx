import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Dashboard from "./pages/dashboard/Dashboard";
import Customers from "@/pages/customers/Customers";
import RegisterTenant from "@/pages/auth/RegisterTenant";
import Orders from "@/pages/orders/Orders";
import Marketing from "@/pages/marketing/Marketing";
import Settings from "@/pages/settings/Settings";
import Transactions from "@/pages/transactions/Transactions";
import Login from "@/pages/auth/Login";

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<Layout/>}>
                    <Route path="/" element={<Dashboard/>}/>
                    <Route path="/customers" element={<Customers/>}/>
                    <Route path="/orders" element={<Orders/>}/>
                    <Route path="/marketing" element={<Marketing/>}/>
                    <Route path="/transactions" element={<Transactions/>}/>
                    <Route path="/settings" element={<Settings/>}/>
                </Route>
                <Route path="/register" element={<RegisterTenant/>}/>
                <Route path="/login" element={<Login/>}/>
            </Routes>
        </BrowserRouter>
    );
}