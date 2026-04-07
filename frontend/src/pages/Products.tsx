import { useEffect, useState } from 'react'
import { productService } from '../services/productService'
import { Product } from '../types'
import toast from 'react-hot-toast'

export default function Products() {
    const [products, setProducts] = useState<Product[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
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
        return <div className="text-center py-8">Đang tải...</div>
    }

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">Quản lý Sản phẩm</h1>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {products.map((product) => (
                    <div key={product.id} className="card">
                        <h3 className="text-xl font-bold mb-2">{product.name}</h3>
                        <p className="text-gray-600 text-sm mb-4">{product.description}</p>
                        <div className="flex justify-between items-center">
                            <span className="text-2xl font-bold text-primary-600">
                                {product.price.toLocaleString('vi-VN')}đ
                            </span>
                            <span className={`px-3 py-1 rounded-full text-sm ${product.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                                }`}>
                                {product.isAvailable ? 'Còn hàng' : 'Hết hàng'}
                            </span>
                        </div>
                        {product.ingredients.length > 0 && (
                            <div className="mt-4 pt-4 border-t">
                                <p className="text-sm font-medium mb-2">Công thức:</p>
                                <ul className="text-sm text-gray-600 space-y-1">
                                    {product.ingredients.map((ing) => (
                                        <li key={ing.ingredientId}>
                                            • {ing.ingredientName}: {ing.quantityRequired} {ing.unit}
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        )}
                    </div>
                ))}
            </div>
        </div>
    )
}
