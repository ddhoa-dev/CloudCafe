import api from './api'
import { Order, CreateOrderRequest, PaginatedResponse } from '../types'

export const orderService = {
    getAll: async (params?: {
        pageNumber?: number
        pageSize?: number
        status?: number
        fromDate?: string
        toDate?: string
        customerPhone?: string
    }): Promise<PaginatedResponse<Order>> => {
        const response = await api.get<PaginatedResponse<Order>>('/orders', { params })
        return response.data
    },

    getById: async (id: string): Promise<Order> => {
        const response = await api.get<Order>(`/orders/${id}`)
        return response.data
    },

    create: async (data: CreateOrderRequest): Promise<string> => {
        const response = await api.post<string>('/orders', data)
        return response.data
    },

    updateStatus: async (id: string, newStatus: number): Promise<void> => {
        await api.patch(`/orders/${id}/status`, { newStatus })
    },

    cancel: async (id: string, reason?: string): Promise<void> => {
        await api.post(`/orders/${id}/cancel`, { cancellationReason: reason })
    },
}
