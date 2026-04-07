import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { productService } from '../services/productService'
import { orderService } from '../services/orderService'
import { Product } from '../types'
import toast from 'react-hot-toast'
import { Minus, Plus, Trash2 } from 'lucide-react'

interface OrderItem {
    product: Product
    quantity: number
    notes?: string
}

export default function CreateOrder() {
    const [products, setProducts] = useState<Product[]>([])
    const [orderItems, setOrderItems] = useState<OrderItem[]>([])
    const [customerName, setCustomerName] = useState('')
    const [customerPhone, setCustomerPhone] = useState('')
    const [notes, setNotes] = useState('')
    const [loading, setLoading] = useState(false)
    const navigate = useNavigate()

    useEffect(() => {
        loadProducts()
    }, [])

    const loadProducts = async () => {
        try {
            const response = await productService.getAll({ pageSize: 100, isAvailable: true })
            setProducts(response.items)
        } catch (error) {
            toast.error('Không thể tải danh sách sản phẩm')
        }
    }

    const addProduct = (product: Product) => {
        const existing = orderItems.find(item => item.product.id === product.id)
        if (existing) {
            setOrderItems(orderItems.map(item =>
                item.product.id === product.id
                    ? { ...item, quantity: item.quantity + 1 }
                    : item
            ))
        } else {
            setOrderItems([...orderItems, { product, quantity: 1 }])
        }
    }

    const updateQuantity = (productId: string, delta: number) => {
        setOrderItems(orderItems.map(item =>
            item.product.id === productId
                ? { ...item, quantity: Math.max(1, item.quantity + delta) }
                : item
        ))
    }

    const removeItem = (productId: string) => {
        setOrderItems(orderItems.filter(item => item.product.id !== productId))
    }

    const calculateTotal = () => {
        return orderItems.reduce((sum, item) => sum + (item.product.price * item.quantity), 0)
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        if (orderItems.length === 0) {
            toast.error('Vui lòng chọn ít nhất 1 sản phẩm')
            return
        }

        setLoading(true)
        try {
            await orderService.create({
                customerName: customerName || undefined,
                customerPhone: customerPhone || undefined,
                notes: notes || undefined,
                items: orderItems.map(item => ({
                    productId: item.product.id,
                    quantity: item.quantity,
                    notes: item.notes,
                })),
            })
            toast.success('Tạo đơn hàng thành công!')
            navigate('/orders')
        } catch (error: any) {
            toast.error(error.response?.data?.message || 'Tạo đơn hàng thất bại')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6">Tạo đơn hàng mới</h1>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Danh sách sản phẩm */}
                <div className="lg:col-span-2">
                    <div className="card">
                        <h2 className="text-xl font-bold mb-4">Chọn sản phẩm</h2>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            {products.map((product) => (
                                <div
                                    key={product.id}
                                    onClick={() => addProduct(product)}
                                    className="p-4 border rounded-lg hover:border-primary-500 cursor-pointer transition-colors"
                                >
                                    <h3 className="font-bold">{product.name}</h3>
                                    <p className="text-sm text-gray-600 mb-2">{product.description}</p>
                                    <p className="text-lg font-bold text-primary-600">
                                        {product.price.toLocaleString('vi-VN')}đ
                                    </p>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>

                {/* Giỏ hàng */}
                <div>
                    <form onSubmit={handleSubmit} className="card sticky top-4">
                        <h2 className="text-xl font-bold mb-4">Giỏ hàng</h2>

                        <div className="space-y-4 mb-4">
                            <input
                                type="text"
                                value={customerName}
                                onChange={(e) => setCustomerName(e.target.value)}
                                className="input"
                                placeholder="Tên khách hàng"
                            />
                            <input
                                type="tel"
                                value={customerPhone}
                                onChange={(e) => setCustomerPhone(e.target.value)}
                                className="input"
                                placeholder="Số điện thoại"
                            />
                            <textarea
                                value={notes}
                                onChange={(e) => setNotes(e.target.value)}
                                className="input"
                                placeholder="Ghi chú"
                                rows={2}
                            />
                        </div>

                        <div className="space-y-3 mb-4 max-h-64 overflow-y-auto">
                            {orderItems.map((item) => (
                                <div key={item.product.id} className="p-3 bg-gray-50 rounded-lg">
                                    <div className="flex justify-between items-start mb-2">
                                        <span className="font-medium text-sm">{item.product.name}</span>
                                        <button
                                            type="button"
                                            onClick={() => removeItem(item.product.id)}
                                            className="text-red-500 hover:text-red-700"
                                        >
                                            <Trash2 size={16} />
                                        </button>
                                    </div>
                                    <div className="flex justify-between items-center">
                                        <div className="flex items-center space-x-2">
                                            <button
                                                type="button"
                                                onClick={() => updateQuantity(item.product.id, -1)}
                                                className="p-1 hover:bg-gray-200 rounded"
                                            >
                                                <Minus size={16} />
                                            </button>
                                            <span className="font-bold">{item.quantity}</span>
                                            <button
                                                type="button"
                                                onClick={() => updateQuantity(item.product.id, 1)}
                                                className="p-1 hover:bg-gray-200 rounded"
                                            >
                                                <Plus size={16} />
                                            </button>
                                        </div>
                                        <span className="font-bold text-primary-600">
                                            {(item.product.price * item.quantity).toLocaleString('vi-VN')}đ
                                        </span>
                                    </div>
                                </div>
                            ))}
                        </div>

                        <div className="border-t pt-4 mb-4">
                            <div className="flex justify-between items-center">
                                <span className="text-lg font-bold">Tổng cộng:</span>
                                <span className="text-2xl font-bold text-primary-600">
                                    {calculateTotal().toLocaleString('vi-VN')}đ
                                </span>
                            </div>
                        </div>

                        <button
                            type="submit"
                            disabled={loading || orderItems.length === 0}
                            className="btn btn-primary w-full"
                        >
                            {loading ? 'Đang tạo...' : 'Tạo đơn hàng'}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    )
}
