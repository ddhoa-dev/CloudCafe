export interface User {
    id: string
    username: string
    email: string
    fullName: string
    role: string
}

export interface LoginRequest {
    username: string
    password: string
}

export interface RegisterRequest {
    username: string
    email: string
    password: string
    fullName: string
    phoneNumber?: string
}

export interface AuthResponse {
    accessToken: string
    tokenType: string
    expiresIn: number
    user: User
}

export interface Product {
    id: string
    name: string
    description?: string
    price: number
    category: number
    categoryName: string
    isAvailable: boolean
    imageUrl?: string
    ingredients: ProductIngredient[]
}

export interface ProductIngredient {
    ingredientId: string
    ingredientName: string
    quantityRequired: number
    unit: string
}

export interface Ingredient {
    id: string
    name: string
    description?: string
    unit: string
    quantityInStock: number
    minimumStockLevel: number
    unitPrice: number
    isLowStock: boolean
}

export interface Order {
    id: string
    orderNumber: string
    orderDate: string
    status: number
    statusName: string
    totalAmount: number
    discountAmount?: number
    finalAmount: number
    customerName?: string
    customerPhone?: string
    notes?: string
    items: OrderItem[]
}

export interface OrderItem {
    productId: string
    productName: string
    quantity: number
    unitPrice: number
    totalPrice: number
    notes?: string
}

export interface CreateOrderRequest {
    customerName?: string
    customerPhone?: string
    notes?: string
    discountAmount?: number
    items: CreateOrderItem[]
}

export interface CreateOrderItem {
    productId: string
    quantity: number
    notes?: string
}

export interface PaginatedResponse<T> {
    items: T[]
    pageNumber: number
    pageSize: number
    totalPages: number
    totalCount: number
    hasPreviousPage: boolean
    hasNextPage: boolean
}
