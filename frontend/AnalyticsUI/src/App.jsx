import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Dashboard from "./pages/Dashboard";
import Customers from "./pages/Customers";
import Profile from "./pages/Profile";
import RegisterTenant from "@/pages/RegisterTenant";
import Orders from "@/pages/Orders";
import Marketing from "@/pages/Marketing";
import Settings from "@/pages/Settings";
import Transactions from "@/pages/Transactions";

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<Layout/>}>
                    <Route path="/" element={<Dashboard/>}/>
                    <Route path="/customers" element={<Customers/>}/>
                    <Route path="/profile" element={<Profile/>}/>
                    <Route path="/orders" element={<Orders/>}/>/
                    <Route path="/marketing" element={<Marketing/>}/>
                    <Route path="/transactions" element={<Transactions/>}/>
                    <Route path="/settings" element={<Settings/>}/>
                </Route>

                <Route path="/register" element={<RegisterTenant/>}/>
            </Routes>
        </BrowserRouter>
    );

}