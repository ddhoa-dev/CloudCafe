import { Outlet, Link, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../stores/authStore'
import { LogOut, Home, Package, ShoppingCart, Layers } from 'lucide-react'

export default function Layout() {
    const { user, logout } = useAuthStore()
    const navigate = useNavigate()

    const handleLogout = () => {
        logout()
        navigate('/login')
    }

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Navbar */}
            <nav className="bg-white shadow-md">
                <div className="container mx-auto px-4">
                    <div className="flex justify-between items-center h-16">
                        <div className="flex items-center space-x-8">
                            <h1 className="text-xl font-bold text-primary-600">☕ Cafe Management</h1>

                            <div className="hidden md:flex space-x-4">
                                <Link to="/" className="flex items-center space-x-2 px-3 py-2 rounded-lg hover:bg-gray-100">
                                    <Home size={20} />
                                    <span>Dashboard</span>
                                </Link>
                                <Link to="/products" className="flex items-center space-x-2 px-3 py-2 rounded-lg hover:bg-gray-100">
                                    <Package size={20} />
                                    <span>Sản phẩm</span>
                                </Link>
                                <Link to="/ingredients" className="flex items-center space-x-2 px-3 py-2 rounded-lg hover:bg-gray-100">
                                    <Layers size={20} />
                                    <span>Nguyên liệu</span>
                                </Link>
                                <Link to="/orders" className="flex items-center space-x-2 px-3 py-2 rounded-lg hover:bg-gray-100">
                                    <ShoppingCart size={20} />
                                    <span>Đơn hàng</span>
                                </Link>
                            </div>
                        </div>

                        <div className="flex items-center space-x-4">
                            <div className="text-right">
                                <p className="text-sm font-medium">{user?.fullName}</p>
                                <p className="text-xs text-gray-500">{user?.role}</p>
                            </div>
                            <button
                                onClick={handleLogout}
                                className="flex items-center space-x-2 px-4 py-2 text-red-600 hover:bg-red-50 rounded-lg"
                            >
                                <LogOut size={20} />
                                <span>Đăng xuất</span>
                            </button>
                        </div>
                    </div>
                </div>
            </nav>

            {/* Main Content */}
            <main className="container mx-auto px-4 py-8">
                <Outlet />
            </main>
        </div>
    )
}
