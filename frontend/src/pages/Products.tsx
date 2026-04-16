import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { productService } from '../services/productService'
import { useAuthStore } from '../stores/authStore'
import { Product } from '../types'
import toast from 'react-hot-toast'
import { motion } from 'framer-motion'
import { Plus, Coffee, Tag, Package } from 'lucide-react'

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

export default function Products() {
    const { user } = useAuthStore()
    const [products, setProducts] = useState<Product[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        window.scrollTo(0, 0);
        loadProducts()
    }, [])

    const loadProducts = async () => {
        try {
            const response = await productService.getAll({ pageSize: 100 })
            setProducts(response.items)
        } catch (error) {
            toast.error('Không thể tải danh sách sản phẩm')
        } finally {
            setLoading(false)
        }
    }

    if (loading) {
        return <div className="text-center py-20 text-slate-500 font-bold animate-pulse">Đang đồng bộ dữ liệu...</div>
    }

    return (
        <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="pb-10 font-sans max-w-7xl mx-auto"
        >
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-end mb-8 gap-4">
                <div>
                    <h1 className="text-4xl font-extrabold text-slate-800 tracking-tight flex items-center gap-3">
                        <Coffee className="text-indigo-600" size={32} /> Quản Lý Dịch Vụ
                    </h1>
                    <p className="text-slate-500 mt-2 font-medium">Trung tâm điều khiển thương hiệu và công thức pha chế</p>
                </div>
                {(user?.role === 'Admin' || user?.role === 'Manager') && (
                    <Link to="/products/create" className="btn btn-primary flex items-center gap-2 px-6 py-3">
                        <Plus size={20} /> <span className="hidden sm:inline">Thêm Món Mới</span><span className="sm:hidden">Tạo Lệnh</span>
                    </Link>
                )}
            </div>

            <motion.div
                variants={containerVariants}
                initial="hidden"
                animate="visible"
                className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
            >
                {products.map((product) => (
                    <motion.div key={product.id} variants={itemVariants} className="card group hover:-translate-y-1 transition-transform duration-300 flex flex-col justify-between">
                        <div>
                            <div className="flex justify-between items-start mb-3">
                                <h3 className="text-xl font-bold text-slate-800 line-clamp-2">{product.name}</h3>
                                <span className={`px-3 py-1 rounded-full text-[10px] font-black uppercase tracking-wider ${product.isAvailable ? 'bg-emerald-100 text-emerald-700' : 'bg-red-100 text-red-700'}`}>
                                    {product.isAvailable ? 'Đang Bán' : 'Tạm Ngưng'}
                                </span>
                            </div>

                            <p className="text-slate-500 text-sm mb-4 min-h-[40px]">
                                {product.description || 'Chưa có mô tả thương mại.'}
                            </p>

                            <div className="flex items-center gap-2 text-2xl font-black text-indigo-600 mb-4">
                                <Tag size={20} className="text-indigo-400" />
                                {product.price.toLocaleString('vi-VN')}đ
                            </div>
                        </div>

                        {product.ingredients.length > 0 ? (
                            <div className="mt-4 pt-4 border-t border-slate-100 bg-slate-50 p-4 rounded-xl">
                                <p className="text-xs font-bold text-slate-400 uppercase tracking-wider mb-2 flex items-center gap-1">
                                    <Package size={14} /> Công Thức Tiêu Hao (Recipe)
                                </p>
                                <ul className="text-sm text-slate-600 space-y-1.5 font-medium">
                                    {product.ingredients.map((ing) => (
                                        <li key={ing.ingredientId} className="flex justify-between items-center">
                                            <span className="truncate pr-2">• {ing.ingredientName}</span>
                                            <span className="text-indigo-600 font-bold shrink-0">{ing.quantityRequired} {ing.unit}</span>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        ) : (
                            <div className="mt-4 pt-4 border-t border-slate-100">
                                <p className="text-xs font-bold text-rose-400 uppercase tracking-wider bg-rose-50 p-2 rounded-lg text-center">⚠ Cảnh Báo: Chưa ráp thành phần kho</p>
                            </div>
                        )}
                    </motion.div>
                ))}
            </motion.div>

            {products.length === 0 && (
                <div className="text-center py-20 bg-white/50 backdrop-blur-md rounded-3xl shadow-sm border border-slate-100 mt-10">
                    <Coffee size={48} className="mx-auto text-slate-300 mb-4" />
                    <h2 className="text-2xl font-bold text-slate-700 mb-2">Chưa Có Dữ Liệu</h2>
                    <p className="text-slate-500 mb-6">Hãy là người đầu tiên tạo ra món mới của quán!</p>
                    {(user?.role === 'Admin' || user?.role === 'Manager') && (
                        <Link to="/products/create" className="btn btn-primary inline-flex items-center gap-2 px-8 py-4">
                            <Plus size={20} /> Sang Phòng R&D
                        </Link>
                    )}
                </div>
            )}
        </motion.div>
    )
}
