import api from './api'
import { Ingredient, PaginatedResponse } from '../types'

export const ingredientService = {
    getAll: async (params?: {
        pageNumber?: number
        pageSize?: number
        isLowStock?: boolean
        searchTerm?: string
    }): Promise<PaginatedResponse<Ingredient>> => {
        const response = await api.get<PaginatedResponse<Ingredient>>('/ingredients', { params })
        return response.data
    },

    getById: async (id: string): Promise<Ingredient> => {
        const response = await api.get<Ingredient>(`/ingredients/${id}`)
        return response.data
    },

    create: async (data: any): Promise<string> => {
        const response = await api.post<string>('/ingredients', data)
        return response.data
    },

    update: async (id: string, data: any): Promise<void> => {
        await api.put(`/ingredients/${id}`, data)
    },
}
