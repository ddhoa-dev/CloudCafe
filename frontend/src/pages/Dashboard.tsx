import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Package, Layers, ShoppingCart, TrendingUp, Coffee, DollarSign } from 'lucide-react'
import { motion } from 'framer-motion'
import {
    LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
    BarChart, Bar
} from 'recharts'
import { productService } from '../services/productService'
import { ingredientService } from '../services/ingredientService'
import { orderService } from '../services/orderService'

const weeklyData = [
    { name: 'T2', orders: 40, revenue: 240 },
    { name: 'T3', orders: 30, revenue: 139 },
    { name: 'T4', orders: 45, revenue: 380 },
    { name: 'T5', orders: 27, revenue: 190 },
    { name: 'T6', orders: 58, revenue: 480 },
    { name: 'T7', orders: 85, revenue: 680 },
    { name: 'CN', orders: 120, revenue: 930 },
];

const containerVariants = {
    hidden: { opacity: 0 },
    visible: {
        opacity: 1,
        transition: { staggerChildren: 0.1 }
    }
}

const itemVariants = {
    hidden: { y: 20, opacity: 0 },
    visible: {
        y: 0,
        opacity: 1,
        transition: { type: 'spring' as const, stiffness: 100 }
    }
}

export default function Dashboard() {
    const [stats, setStats] = useState({
        totalProducts: 0,
        totalIngredients: 0,
        totalOrders: 0,
        lowStockIngredients: 0,
    })

    useEffect(() => {
        // Cuộn mượt lên trên cùng khi vừa load Dashboard
        window.scrollTo(0, 0);
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
            title: 'Tổng Đơn Hàng',
            value: stats.totalOrders,
            icon: ShoppingCart,
            color: 'from-blue-500 to-indigo-600',
            shadow: 'shadow-indigo-500/30',
            link: '/orders',
        },
        {
            title: 'Món Đang Phục Vụ',
            value: stats.totalProducts,
            icon: Coffee,
            color: 'from-emerald-400 to-teal-500',
            shadow: 'shadow-teal-500/30',
            link: '/products',
        },
        {
            title: 'Mặt Hàng Trong Kho',
            value: stats.totalIngredients,
            icon: Layers,
            color: 'from-orange-400 to-amber-500',
            shadow: 'shadow-amber-500/30',
            link: '/ingredients',
        },
        {
            title: 'Sắp Hết Hàng',
            value: stats.lowStockIngredients,
            icon: TrendingUp,
            color: 'from-rose-500 to-red-600',
            shadow: 'shadow-red-500/30',
            link: '/ingredients?lowStock=true',
        },
    ]

    return (
        <div className="pb-10 font-sans">
            <motion.div
                initial={{ opacity: 0, y: -20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5 }}
                className="mb-10 flex flex-col sm:flex-row justify-between items-start sm:items-end gap-4"
            >
                <div>
                    <h1 className="text-4xl font-extrabold text-slate-800 tracking-tight">Tổng Quan Trạm Quản Lý</h1>
                    <p className="text-slate-500 mt-2 font-medium">Theo dõi hiệu suất cửa hàng cà phê thời gian thực</p>
                </div>
                <Link to="/orders/create" className="btn btn-primary flex items-center gap-2 px-6 py-3">
                    <Package size={20} /> <span className="hidden sm:inline">Tạo Đơn Siêu Tốc</span><span className="sm:hidden">Tạo Lệnh</span>
                </Link>
            </motion.div>

            {/* Thẻ Thống Kê Điểm Nhấn */}
            <motion.div
                variants={containerVariants}
                initial="hidden"
                animate="visible"
                className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10"
            >
                {statCards.map((stat) => (
                    <Link key={stat.title} to={stat.link}>
                        <motion.div variants={itemVariants} className="card group relative overflow-hidden h-full flex flex-col justify-between cursor-pointer">
                            {/* Hiệu ứng Background Phát sáng tự động */}
                            <div className={`absolute -top-10 -right-10 w-40 h-40 bg-gradient-to-br ${stat.color} rounded-full blur-3xl opacity-10 group-hover:opacity-30 transition-opacity duration-700`}></div>

                            <div className="flex items-start justify-between relative z-10">
                                <div>
                                    <p className="text-slate-500 font-bold uppercase tracking-wider text-xs">{stat.title}</p>
                                    <p className="text-4xl font-black mt-3 text-slate-800">{stat.value}</p>
                                </div>
                                <div className={`bg-gradient-to-br ${stat.color} p-4 rounded-2xl text-white shadow-lg ${stat.shadow} transform group-hover:scale-110 transition-transform duration-300`}>
                                    <stat.icon size={28} strokeWidth={2.5} />
                                </div>
                            </div>
                        </motion.div>
                    </Link>
                ))}
            </motion.div>

            {/* Dữ liệu Biểu Đồ So Sánh */}
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.4, duration: 0.5 }}
                className="grid grid-cols-1 lg:grid-cols-3 gap-6"
            >
                {/* Biểu đồ Đường - Xu Hướng Đơn */}
                <div className="card lg:col-span-2 relative overflow-hidden">
                    <div className="flex justify-between items-center mb-6">
                        <h2 className="text-xl font-extrabold text-slate-800">Biến Động Cầu Đơn (7 Ngày)</h2>
                    </div>
                    <div className="h-72 w-full">
                        <ResponsiveContainer width="100%" height="100%">
                            <LineChart data={weeklyData} margin={{ top: 5, right: 20, bottom: 5, left: -20 }}>
                                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                                <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: '#94a3b8', fontSize: 12, fontWeight: 600 }} dy={10} />
                                <YAxis axisLine={false} tickLine={false} tick={{ fill: '#94a3b8', fontSize: 12, fontWeight: 600 }} dx={-10} />
                                <Tooltip
                                    contentStyle={{ borderRadius: '1rem', border: '1px solid #e2e8f0', boxShadow: '0 20px 25px -5px rgb(0 0 0 / 0.1)', padding: '12px' }}
                                    itemStyle={{ fontWeight: 800 }}
                                    cursor={{ stroke: '#cbd5e1', strokeWidth: 2, strokeDasharray: '3 3' }}
                                />
                                <Line
                                    type="monotone"
                                    name="Đơn hàng"
                                    dataKey="orders"
                                    stroke="#4f46e5"
                                    strokeWidth={4}
                                    dot={{ r: 5, strokeWidth: 3, fill: '#ffffff', stroke: '#4f46e5' }}
                                    activeDot={{ r: 8, stroke: '#ffffff', strokeWidth: 3, fill: '#4f46e5' }}
                                />
                            </LineChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                {/* Biểu đồ Cột - Doanh Thu Mẫu */}
                <div className="card lg:col-span-1 relative overflow-hidden">
                    <div className="flex justify-between items-center mb-6">
                        <h2 className="text-xl font-extrabold text-slate-800">Doanh Thu</h2>
                        <div className="bg-emerald-100 p-2 rounded-xl">
                            <DollarSign className="text-emerald-600" size={20} strokeWidth={2.5} />
                        </div>
                    </div>
                    <div className="h-72 w-full">
                        <ResponsiveContainer width="100%" height="100%">
                            <BarChart data={weeklyData} margin={{ top: 5, right: 0, bottom: 5, left: -20 }}>
                                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                                <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: '#94a3b8', fontSize: 12, fontWeight: 600 }} dy={10} />
                                <Tooltip
                                    cursor={{ fill: '#f8fafc' }}
                                    contentStyle={{ borderRadius: '1rem', border: '1px solid #e2e8f0', boxShadow: '0 20px 25px -5px rgb(0 0 0 / 0.1)', padding: '12px' }}
                                    formatter={(value) => [`$${value}k`, 'Thu Nhập']}
                                />
                                <Bar dataKey="revenue" name="Thu Nhập" fill="#10b981" radius={[8, 8, 0, 0]} barSize={32} />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            </motion.div>
        </div>
    )
}
