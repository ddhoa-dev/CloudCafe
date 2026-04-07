import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Package, Layers, ShoppingCart, TrendingUp } from 'lucide-react'
import { productService } from '../services/productService'
import { ingredientService } from '../services/ingredientService'
import { orderService } from '../services/orderService'

export default function Dashboard() {
    const [stats, setStats] = useState({
        totalProducts: 0,
        totalIngredients: 0,
        totalOrders: 0,
        lowStockIngredients: 0,
    })

    useEffect(() => {
        loadStats()
    }, [])

    const loadStats = async () => {
        try {
            const [products, ingredients, orders, lowStock] = await Promise.all([
                productService.getAll({ pageSize: 1 }),
                ingredientService.getAll({ pageSize: 1 }),
                orderService.getAll({ pageSize: 1 }),
                ingredientService.getAll({ pageSize: 1, isLowStock: true }),
            ])

            setStats({
                totalProducts: products.totalCount,
                totalIngredients: ingredients.totalCount,
                totalOrders: orders.totalCount,
                lowStockIngredients: lowStock.totalCount,
            })
        } catch (error) {
            console.error('Failed to load stats:', error)
        }
    }

    const statCards = [
        {
            title: 'Sản phẩm',
            value: stats.totalProducts,
            icon: Package,
            color: 'bg-blue-500',
            link: '/products',
        },
        {
            title: 'Nguyên liệu',
            value: stats.totalIngredients,
            icon: Layers,
            color: 'bg-green-500',
            link: '/ingredients',
        },
        {
            title: 'Đơn hàng',
            value: stats.totalOrders,
            icon: ShoppingCart,
            color: 'bg-purple-500',
            link: '/orders',
        },
        {
            title: 'Nguyên liệu sắp hết',
            value: stats.lowStockIngredients,
            icon: TrendingUp,
            color: 'bg-red-500',
            link: '/ingredients?lowStock=true',
        },
    ]

    return (
        <div>
            <h1 className="text-3xl font-bold mb-8">Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                {statCards.map((stat) => (
                    <Link key={stat.title} to={stat.link}>
                        <div className="card hover:shadow-lg transition-shadow cursor-pointer">
                            <div className="flex items-center justify-between">
                                <div>
                                    <p className="text-gray-600 text-sm">{stat.title}</p>
                                    <p className="text-3xl font-bold mt-2">{stat.value}</p>
                                </div>
                                <div className={`${stat.color} p-4 rounded-lg`}>
                                    <stat.icon className="text-white" size={32} />
                                </div>
                            </div>
                        </div>
                    </Link>
                ))}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <div className="card">
                    <h2 className="text-xl font-bold mb-4">Thao tác nhanh</h2>
                    <div className="space-y-3">
                        <Link to="/orders/create" className="btn btn-primary w-full">
                            Tạo đơn hàng mới
                        </Link>
                        <Link to="/products" className="btn btn-secondary w-full">
                            Quản lý sản phẩm
                        </Link>
                        <Link to="/ingredients" className="btn btn-secondary w-full">
                            Quản lý nguyên liệu
                        </Link>
                    </div>
                </div>

                <div className="card">
                    <h2 className="text-xl font-bold mb-4">Thông tin hệ thống</h2>
                    <div className="space-y-3 text-sm">
                        <div className="flex justify-between">
                            <span className="text-gray-600">Phiên bản:</span>
                            <span className="font-medium">1.0.0</span>
                        </div>
                        <div className="flex justify-between">
                            <span className="text-gray-600">Backend:</span>
                            <span className="font-medium">.NET 8</span>
                        </div>
                        <div className="flex justify-between">
                            <span className="text-gray-600">Database:</span>
                            <span className="font-medium">PostgreSQL</span>
                        </div>
                        <div className="flex justify-between">
                            <span className="text-gray-600">Architecture:</span>
                            <span className="font-medium">Clean Architecture</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}
