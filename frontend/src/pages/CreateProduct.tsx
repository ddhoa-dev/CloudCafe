import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Plus, Trash2, Save, X, Coffee, Image, Info, DollarSign, Layers } from 'lucide-react'
import { motion, AnimatePresence } from 'framer-motion'
import { productService } from '../services/productService'
import { ingredientService } from '../services/ingredientService'
import { Ingredient } from '../types'
import toast from 'react-hot-toast'

interface RecipeItem {
    ingredientId: string
    quantityRequired: number
}

// Map Enum ProductCategory từ Backend
const CATEGORIES = [
    { value: 1, label: 'Cà phê' },
    { value: 2, label: 'Trà' },
    { value: 3, label: 'Sinh tố' },
    { value: 4, label: 'Nước ép' },
    { value: 5, label: 'Tráng miệng' },
    { value: 99, label: 'Khác' },
]

export default function CreateProduct() {
    const navigate = useNavigate()
    const [loading, setLoading] = useState(false)
    const [availableIngredients, setAvailableIngredients] = useState<Ingredient[]>([])

    // Product Info
    const [name, setName] = useState('')
    const [description, setDescription] = useState('')
    const [price, setPrice] = useState<number | ''>('')
    const [category, setCategory] = useState<number>(1)
    const [imageUrl, setImageUrl] = useState('')

    // Recipe Info
    const [recipeItems, setRecipeItems] = useState<RecipeItem[]>([])

    useEffect(() => {
        loadIngredients()
    }, [])

    const loadIngredients = async () => {
        try {
            const response = await ingredientService.getAll({ pageSize: 200 })
            setAvailableIngredients(response.items)
        } catch (error) {
            toast.error('Lỗi khi tải danh sách nguyên liệu')
        }
    }

    const addRecipeRow = () => {
        setRecipeItems([...recipeItems, { ingredientId: '', quantityRequired: 0 }])
    }

    const removeRecipeRow = (index: number) => {
        const newItems = [...recipeItems]
        newItems.splice(index, 1)
        setRecipeItems(newItems)
    }

    const updateRecipeRow = (index: number, field: keyof RecipeItem, value: any) => {
        const newItems = [...recipeItems]
        newItems[index] = { ...newItems[index], [field]: value }
        setRecipeItems(newItems)
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()

        if (!name.trim()) return toast.error('Vui lòng nhập tên sản phẩm')
        if (!price || price <= 0) return toast.error('Giá bán phải lớn hơn 0')
        if (recipeItems.length === 0) return toast.error('Một sản phẩm phải có ít nhất 1 nguyên liệu/công thức')
        
        // Kiểm tra tính hợp lệ của công thức
        for (let i = 0; i < recipeItems.length; i++) {
            if (!recipeItems[i].ingredientId) return toast.error(`Nguyên liệu dòng số ${i + 1} chưa được chọn`)
            if (recipeItems[i].quantityRequired <= 0) return toast.error(`Định lượng dòng số ${i + 1} phải > 0`)
        }

        setLoading(true)
        try {
            const payload = {
                name,
                description: description || undefined,
                price: Number(price),
                category,
                imageUrl: imageUrl || undefined,
                ingredients: recipeItems
            }

            await productService.create(payload)
            toast.success('Hệ thống đã nhận sản phẩm & công thức chuẩn!')
            navigate('/products')
        } catch (error: any) {
            toast.error(error.response?.data?.message || 'Có lỗi xảy ra khi tạo mã sản phẩm')
        } finally {
            setLoading(false)
        }
    }

    return (
        <motion.div 
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="pb-10 font-sans max-w-6xl mx-auto"
        >
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-3xl font-extrabold text-slate-800 tracking-tight flex items-center gap-3">
                        <Coffee className="text-indigo-600" size={32} /> R&D Sản Phẩm Mới
                    </h1>
                    <p className="text-slate-500 mt-2">Thiết lập thông tin thương mại và công thức tiêu chuẩn</p>
                </div>
                <button 
                    onClick={() => navigate('/products')}
                    className="btn btn-secondary flex items-center gap-2"
                >
                    <X size={20} /> Trở về kho
                </button>
            </div>

            <form onSubmit={handleSubmit} className="grid grid-cols-1 lg:grid-cols-12 gap-8">
                
                {/* Information Column */}
                <div className="lg:col-span-5 space-y-6">
                    <div className="card border-t-4 border-t-indigo-500">
                        <h2 className="text-xl font-bold mb-6 text-slate-800 flex items-center gap-2">
                            <Info size={20} className="text-indigo-500"/> Thông Tin Đinh Danh
                        </h2>
                        
                        <div className="space-y-5">
                            <div>
                                <label className="block text-sm font-semibold text-slate-700 mb-2">Tên Món <span className="text-red-500">*</span></label>
                                <input 
                                    type="text" 
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                    placeholder="VD: Cà phê Phin Muối"
                                    className="input focus:ring-indigo-500"
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-semibold text-slate-700 mb-2">Phân Loại</label>
                                    <select 
                                        value={category}
                                        onChange={(e) => setCategory(Number(e.target.value))}
                                        className="input focus:ring-indigo-500 cursor-pointer"
                                    >
                                        {CATEGORIES.map(cat => (
                                            <option key={cat.value} value={cat.value}>{cat.label}</option>
                                        ))}
                                    </select>
                                </div>
                                <div>
                                    <label className="block text-sm font-semibold text-slate-700 mb-2 flex items-center gap-2">
                                        Giá Bán (VNĐ) <span className="text-red-500">*</span>
                                    </label>
                                    <div className="relative">
                                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                            <DollarSign className="h-5 w-5 text-gray-400" />
                                        </div>
                                        <input 
                                            type="number" 
                                            min="0"
                                            value={price}
                                            onChange={(e) => setPrice(e.target.value ? Number(e.target.value) : '')}
                                            placeholder="45000"
                                            className="input pl-10 focus:ring-indigo-500"
                                        />
                                    </div>
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm font-semibold text-slate-700 mb-2">Đường Dẫn Ảnh Cover (URL)</label>
                                <div className="relative">
                                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                        <Image className="h-5 w-5 text-gray-400" />
                                    </div>
                                    <input 
                                        type="url" 
                                        value={imageUrl}
                                        onChange={(e) => setImageUrl(e.target.value)}
                                        placeholder="https://example.com/image.png"
                                        className="input pl-10 focus:ring-indigo-500"
                                    />
                                </div>
                                {imageUrl && (
                                    <div className="mt-3 w-full h-32 rounded-xl overflow-hidden border border-slate-200">
                                        <img src={imageUrl} alt="Preview" className="w-full h-full object-cover" onError={(e) => (e.target as HTMLImageElement).style.display = 'none'} />
                                    </div>
                                )}
                            </div>

                            <div>
                                <label className="block text-sm font-semibold text-slate-700 mb-2">Mô Tả Bán Hàng</label>
                                <textarea 
                                    value={description}
                                    onChange={(e) => setDescription(e.target.value)}
                                    placeholder="Điểm độc đáo của món này..."
                                    className="input focus:ring-indigo-500 min-h-[100px] resize-y"
                                />
                            </div>
                        </div>
                    </div>
                </div>

                {/* Recipe Column */}
                <div className="lg:col-span-7">
                    <div className="card h-full flex flex-col border-t-4 border-t-emerald-500">
                        <div className="flex justify-between items-center mb-6">
                            <h2 className="text-xl font-bold text-slate-800 flex items-center gap-2">
                                <Layers size={20} className="text-emerald-500"/> Công Thức Định Lượng (Recipe) 
                            </h2>
                            <span className="bg-emerald-100 text-emerald-800 text-xs font-bold px-3 py-1 rounded-full uppercase tracking-wider">
                                Inventory Engine
                            </span>
                        </div>
                        
                        <p className="text-sm text-slate-500 mb-6 bg-slate-50 p-4 rounded-xl border border-slate-100 italic">
                            Hệ thống sẽ tự động trừ kho nguyên liệu dựa trên định lượng bạn cài đặt ở dưới mỗi khi có một đơn hàng được tạo.
                        </p>

                        <div className="space-y-4 mb-6 flex-grow">
                            <AnimatePresence>
                                {recipeItems.map((item, index) => (
                                    <motion.div 
                                        key={index}
                                        initial={{ opacity: 0, x: -20 }}
                                        animate={{ opacity: 1, x: 0 }}
                                        exit={{ opacity: 0, scale: 0.95 }}
                                        className="flex flex-col sm:flex-row gap-3 items-end sm:items-center bg-slate-50 p-4 rounded-2xl border border-slate-200 shadow-sm"
                                    >
                                        <div className="w-full sm:w-2/3">
                                            <label className="block text-xs font-semibold text-slate-500 mb-1">Mặt Hàng Kho</label>
                                            <select
                                                value={item.ingredientId}
                                                onChange={(e) => updateRecipeRow(index, 'ingredientId', e.target.value)}
                                                className="input border-slate-300 focus:ring-emerald-500 bg-white"
                                            >
                                                <option value="">-- Chiết xuất từ --</option>
                                                {availableIngredients.map(ing => (
                                                    <option key={ing.id} value={ing.id}>
                                                        {ing.name} (Tồn: {ing.quantityInStock} {ing.unit})
                                                    </option>
                                                ))}
                                            </select>
                                        </div>
                                        
                                        <div className="w-full sm:w-1/3">
                                            <label className="block text-xs font-semibold text-slate-500 mb-1">Tiêu hao</label>
                                            <div className="relative">
                                                <input
                                                    type="number"
                                                    min="0.1"
                                                    step="0.1"
                                                    value={item.quantityRequired || ''}
                                                    onChange={(e) => updateRecipeRow(index, 'quantityRequired', Number(e.target.value))}
                                                    className="input border-slate-300 focus:ring-emerald-500 pr-12 bg-white"
                                                    placeholder="0"
                                                />
                                                <div className="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                                                    <span className="text-gray-500 text-sm font-semibold">
                                                        {item.ingredientId ? availableIngredients.find(x => x.id === item.ingredientId)?.unit : 'đv'}
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                        <button
                                            type="button"
                                            onClick={() => removeRecipeRow(index)}
                                            className="w-full sm:w-auto mt-2 sm:mt-0 p-3 bg-red-100 text-red-600 hover:bg-red-200 rounded-xl transition-colors shrink-0 flex justify-center items-center h-[50px] mb-[2px]"
                                            title="Xóa nguyên liệu"
                                        >
                                            <Trash2 size={20} />
                                        </button>
                                    </motion.div>
                                ))}
                            </AnimatePresence>

                            {recipeItems.length === 0 && (
                                <div className="text-center py-10 border-2 border-dashed border-slate-200 rounded-2xl bg-slate-50 text-slate-400">
                                    <Layers size={48} className="mx-auto mb-3 opacity-20" />
                                    <p>Chưa có nguyên liệu nào.</p>
                                    <p className="text-sm">Thêm một nguyên liệu để hệ thống tự động trừ kho.</p>
                                </div>
                            )}
                        </div>

                        <div className="mt-auto space-y-4 pt-6 border-t border-slate-100">
                            <button
                                type="button"
                                onClick={addRecipeRow}
                                className="w-full py-3 rounded-xl border-2 border-dashed border-emerald-500 text-emerald-600 font-bold hover:bg-emerald-50 transition-colors flex justify-center items-center gap-2"
                            >
                                <Plus size={20} /> Thêm Công Thức
                            </button>
                            
                            <button
                                type="submit"
                                disabled={loading}
                                className="btn w-full bg-gradient-to-r from-emerald-600 to-teal-500 text-white shadow-lg shadow-emerald-500/30 hover:shadow-emerald-500/50 flex justify-center items-center gap-2 py-4 text-lg"
                            >
                                <Save size={24} /> {loading ? 'Hệ thống đang ghi nhận...' : 'Phát Hành Sản Phẩm'}
                            </button>
                        </div>
                    </div>
                </div>
            </form>
        </motion.div>
    )
}
