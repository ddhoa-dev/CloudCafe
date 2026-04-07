import api from './api'
import { Product, PaginatedResponse } from '../types'

export const productService = {
    getAll: async (params?: {
        pageNumber?: number
        pageSize?: number
        category?: number
        isAvailable?: boolean
        searchTerm?: string
    }): Promise<PaginatedResponse<Product>> => {
        const response = await api.get<PaginatedResponse<Product>>('/products', { params })
        return response.data
    },

    getById: async (id: string): Promise<Product> => {
        const response = await api.get<Product>(`/products/${id}`)
        return response.data
    },

    create: async (data: any): Promise<string> => {
        const response = await api.post<string>('/products', data)
        return response.data
    },

    update: async (id: string, data: any): Promise<void> => {
        await api.put(`/products/${id}`, data)
    },

    delete: async (id: string): Promise<void> => {
        await api.delete(`/products/${id}`)
    },
}
