import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ingredientService } from '../services/ingredientService'
import { useAuthStore } from '../stores/authStore'
import { Ingredient } from '../types'
import toast from 'react-hot-toast'
import { AlertTriangle } from 'lucide-react'

export default function Ingredients() {
    const { user } = useAuthStore()
    const [ingredients, setIngredients] = useState<Ingredient[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        loadIngredients()
    }, [])

    const loadIngredients = async () => {
        try {
            const response = await ingredientService.getAll({ pageSize: 100 })
            setIngredients(response.items)
        } catch (error) {
            toast.error('Không thể tải danh sách nguyên liệu')
        } finally {
            setLoading(false)
        }
    }

    if (loading) {
        return <div className="text-center py-8">Đang tải...</div>
    }

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">Quản lý Nguyên liệu</h1>
                {(user?.role === 'Admin' || user?.role === 'Manager') && (
                    <Link to="/ingredients/create" className="btn btn-primary bg-emerald-500 hover:bg-emerald-600 border-none shadow-lg shadow-emerald-500/30 flex items-center gap-2">
                        <span className="hidden sm:inline">Nhập Nguyên Liệu Mới</span><span className="sm:hidden">Thêm</span>
                    </Link>
                )}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {ingredients.map((ingredient) => (
                    <div key={ingredient.id} className={`card ${ingredient.isLowStock ? 'border-2 border-red-500' : ''}`}>
                        <div className="flex justify-between items-start mb-4">
                            <div>
                                <h3 className="text-xl font-bold">{ingredient.name}</h3>
                                <p className="text-gray-600 text-sm">{ingredient.description}</p>
                            </div>
                            {ingredient.isLowStock && (
                                <AlertTriangle className="text-red-500" size={24} />
                            )}
                        </div>

                        <div className="space-y-2 text-sm">
                            <div className="flex justify-between">
                                <span className="text-gray-600">Tồn kho:</span>
                                <span className={`font-bold ${ingredient.isLowStock ? 'text-red-600' : 'text-green-600'}`}>
                                    {ingredient.quantityInStock} {ingredient.unit}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-600">Ngưỡng tối thiểu:</span>
                                <span className="font-medium">{ingredient.minimumStockLevel} {ingredient.unit}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-600">Giá:</span>
                                <span className="font-medium">{ingredient.unitPrice.toLocaleString('vi-VN')}đ/{ingredient.unit}</span>
                            </div>
                        </div>

                        {ingredient.isLowStock && (
                            <div className="mt-4 p-3 bg-red-50 rounded-lg">
                                <p className="text-sm text-red-800 font-medium">⚠️ Nguyên liệu sắp hết!</p>
                            </div>
                        )}
                    </div>
                ))}
            </div>
        </div>
    )
}
