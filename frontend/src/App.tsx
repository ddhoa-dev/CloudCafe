import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './stores/authStore'
import Layout from './components/Layout'
import Login from './pages/Login'
import Register from './pages/Register'
import Dashboard from './pages/Dashboard'
import Products from './pages/Products'
import Ingredients from './pages/Ingredients'
import Orders from './pages/Orders'
import CreateOrder from './pages/CreateOrder'

function App() {
    const { isAuthenticated } = useAuthStore()

    return (
        <Routes>
            <Route path="/login" element={!isAuthenticated ? <Login /> : <Navigate to="/" />} />
            <Route path="/register" element={!isAuthenticated ? <Register /> : <Navigate to="/" />} />

            <Route path="/" element={isAuthenticated ? <Layout /> : <Navigate to="/login" />}>
                <Route index element={<Dashboard />} />
                <Route path="products" element={<Products />} />
                <Route path="ingredients" element={<Ingredients />} />
                <Route path="orders" element={<Orders />} />
                <Route path="orders/create" element={<CreateOrder />} />
            </Route>
        </Routes>
    )
}

export default App
