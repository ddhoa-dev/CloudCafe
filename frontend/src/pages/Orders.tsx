import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { orderService } from '../services/orderService'
import { Order } from '../types'
import toast from 'react-hot-toast'
import { Plus } from 'lucide-react'

const statusColors: Record<string, string> = {
    Pending: 'bg-yellow-100 text-yellow-800',
    Preparing: 'bg-blue-100 text-blue-800',
    Ready: 'bg-green-100 text-green-800',
    Completed: 'bg-gray-100 text-gray-800',
    Cancelled: 'bg-red-100 text-red-800',
}

export default function Orders() {
    const [orders, setOrders] = useState<Order[]>([])
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        loadOrders()
    }, [])

    const loadOrders = async () => {
        try {
            const response = await orderService.getAll({ pageSize: 100 })
            setOrders(response.items)
        } catch (error) {
            toast.error('Không thể tải danh sách đơn hàng')
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
                <h1 className="text-3xl font-bold">Quản lý Đơn hàng</h1>
                <Link to="/orders/create" className="btn btn-primary flex items-center space-x-2">
                    <Plus size={20} />
                    <span>Tạo đơn hàng</span>
                </Link>
            </div>

            <div className="space-y-4">
                {orders.map((order) => (
                    <div key={order.id} className="card">
                        <div className="flex justify-between items-start mb-4">
                            <div>
                                <h3 className="text-xl font-bold">{order.orderNumber}</h3>
                                <p className="text-gray-600 text-sm">
                                    {new Date(order.orderDate).toLocaleString('vi-VN')}
                                </p>
                            </div>
                            <span className={`px-3 py-1 rounded-full text-sm font-medium ${statusColors[order.statusName]}`}>
                                {order.statusName}
                            </span>
                        </div>

                        {order.customerName && (
                            <div className="mb-4">
                                <p className="text-sm">
                                    <span className="text-gray-600">Khách hàng:</span> {order.customerName}
                                </p>
                                {order.customerPhone && (
                                    <p className="text-sm">
                                        <span className="text-gray-600">SĐT:</span> {order.customerPhone}
                                    </p>
                                )}
                            </div>
                        )}

                        <div className="border-t pt-4">
                            <div className="space-y-2 mb-4">
                                {order.items.map((item, index) => (
                                    <div key={index} className="flex justify-between text-sm">
                                        <span>{item.productName} x{item.quantity}</span>
                                        <span className="font-medium">{item.totalPrice.toLocaleString('vi-VN')}đ</span>
                                    </div>
                                ))}
                            </div>

                            <div className="flex justify-between items-center pt-4 border-t">
                                <span className="font-bold">Tổng cộng:</span>
                                <span className="text-2xl font-bold text-primary-600">
                                    {order.finalAmount.toLocaleString('vi-VN')}đ
                                </span>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    )
}
