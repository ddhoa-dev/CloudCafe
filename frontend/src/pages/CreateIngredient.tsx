import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ingredientService } from '../services/ingredientService';
import toast from 'react-hot-toast';
import { motion } from 'framer-motion';
import { ArrowLeft, Save, Plus, Scale, DollarSign, Database, FileText } from 'lucide-react';

export default function CreateIngredient() {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    // Form state corresponding to CreateIngredientCommand backend structure
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        unit: 'ml',
        quantityInStock: 0,
        minimumStockLevel: 0,
        unitPrice: 0,
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        // Validation
        if (formData.quantityInStock < 0 || formData.minimumStockLevel < 0 || formData.unitPrice < 0) {
            toast.error('Các giá trị số lượng và giá phải lớn hơn hoặc bằng 0.');
            setLoading(false);
            return;
        }

        try {
            await ingredientService.create(formData);
            toast.success('Dữ liệu nguyên liệu đã được khởi tạo thành công!');
            navigate('/ingredients');
        } catch (error: any) {
            const errorMsg = error.response?.data?.message || 'Có lỗi xảy ra khi tạo mã nguyên liệu';
            toast.error(errorMsg);
        } finally {
            setLoading(false);
        }
    };

    const containerVariants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: { staggerChildren: 0.1 }
        }
    };

    const itemVariants = {
        hidden: { y: 20, opacity: 0 },
        visible: {
            y: 0,
            opacity: 1,
            transition: { type: 'spring' as const, stiffness: 100 }
        }
    };

    return (
        <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="pb-10 font-sans max-w-4xl mx-auto"
        >
            <div className="flex items-center justify-between mb-8 pb-4 border-b border-slate-200">
                <div className="flex items-center gap-4">
                    <button
                        onClick={() => navigate('/ingredients')}
                        className="p-2 hover:bg-slate-100 rounded-full transition-colors text-slate-500"
                        title="Quay lại"
                    >
                        <ArrowLeft size={24} />
                    </button>
                    <div>
                        <h1 className="text-3xl font-extrabold text-slate-800 tracking-tight flex items-center gap-3">
                            <Plus className="text-emerald-500" size={32} /> Nhập Nguyên Liệu
                        </h1>
                        <p className="text-slate-500 mt-1 font-medium">Bổ sung tài sản vật tư vào hệ thống chuỗi cung ứng</p>
                    </div>
                </div>
            </div>

            <motion.form 
                variants={containerVariants}
                initial="hidden"
                animate="visible"
                onSubmit={handleSubmit} 
                className="bg-white/70 backdrop-blur-xl shadow-xl shadow-slate-200/50 rounded-3xl p-8 border border-white"
            >
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    {/* Cột 1: Thông tin cơ bản */}
                    <div className="space-y-6">
                        <motion.div variants={itemVariants} className="bg-slate-50/50 p-6 rounded-2xl border border-slate-100">
                            <h2 className="text-xl font-bold text-slate-700 mb-6 flex items-center gap-2">
                                <FileText className="text-blue-500" size={20} /> Danh tính Vật tư
                            </h2>
                            <div className="space-y-5">
                                <div>
                                    <label className="block text-sm font-semibold text-slate-700 mb-2">Tên Nguyên Liệu *</label>
                                    <input
                                        type="text"
                                        required
                                        value={formData.name}
                                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                        className="w-full px-4 py-3 rounded-xl border-slate-200 bg-white focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all shadow-sm"
                                        placeholder="VD: Cà phê hạt Arabica Cầu Đất"
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-semibold text-slate-700 mb-2">Thông số kỹ thuật / Mô tả</label>
                                    <textarea
                                        value={formData.description}
                                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                        className="w-full px-4 py-3 rounded-xl border-slate-200 bg-white focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all shadow-sm h-32 resize-none"
                                        placeholder="Đặc tính, nguồn gốc, yêu cầu bảo quản..."
                                    />
                                </div>
                            </div>
                        </motion.div>
                    </div>

                    {/* Cột 2: Thông số vật lý & Cảnh báo */}
                    <div className="space-y-6">
                        <motion.div variants={itemVariants} className="bg-emerald-50/30 p-6 rounded-2xl border border-emerald-100/50">
                            <h2 className="text-xl font-bold text-emerald-800 mb-6 flex items-center gap-2">
                                <Database className="text-emerald-500" size={20} /> Định mức Tồn kho
                            </h2>
                            <div className="space-y-5">
                                <div>
                                    <label className="block text-sm font-semibold text-emerald-900 mb-2 flex items-center gap-2">
                                        <Scale size={16}/> Đơn vị đo lường cơ sở *
                                    </label>
                                    <select
                                        value={formData.unit}
                                        onChange={(e) => setFormData({ ...formData, unit: e.target.value })}
                                        className="w-full px-4 py-3 rounded-xl border-emerald-200 bg-white focus:ring-2 focus:ring-emerald-500 focus:border-transparent transition-all shadow-sm text-emerald-900 font-medium"
                                    >
                                        <option value="ml">Mililit (ml)</option>
                                        <option value="g">Gram (g)</option>
                                        <option value="kg">Kilogram (kg)</option>
                                        <option value="lit">Lít (l)</option>
                                        <option value="hop">Hộp / Lon</option>
                                        <option value="cai">Cái / Chiếc</option>
                                    </select>
                                </div>
                                
                                <div className="grid grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-semibold text-emerald-900 mb-2">Tồn kho ban đầu</label>
                                        <div className="relative">
                                            <input
                                                type="number"
                                                min="0"
                                                step="0.01"
                                                required
                                                value={formData.quantityInStock}
                                                onChange={(e) => setFormData({ ...formData, quantityInStock: parseFloat(e.target.value) || 0 })}
                                                className="w-full px-4 py-3 rounded-xl border-emerald-200 bg-white focus:ring-2 focus:ring-emerald-500 focus:border-transparent transition-all shadow-sm font-mono text-emerald-800"
                                            />
                                            <span className="absolute right-4 top-3 text-emerald-400 font-medium">{formData.unit}</span>
                                        </div>
                                    </div>
                                    <div>
                                        <label className="block text-sm font-semibold text-orange-800 mb-2">Ngưỡng báo động</label>
                                        <div className="relative">
                                            <input
                                                type="number"
                                                min="0"
                                                step="0.01"
                                                required
                                                value={formData.minimumStockLevel}
                                                onChange={(e) => setFormData({ ...formData, minimumStockLevel: parseFloat(e.target.value) || 0 })}
                                                className="w-full px-4 py-3 rounded-xl border-orange-200 bg-white focus:ring-2 focus:ring-orange-400 focus:border-transparent transition-all shadow-sm font-mono text-orange-700"
                                            />
                                            <span className="absolute right-4 top-3 text-orange-400 font-medium">{formData.unit}</span>
                                        </div>
                                    </div>
                                </div>

                                <div>
                                    <label className="block text-sm font-semibold text-indigo-900 mb-2 flex items-center gap-2 mt-4">
                                        <DollarSign size={16}/> Giá vốn nhập hàng (VND / 1 {formData.unit}) *
                                    </label>
                                    <div className="relative">
                                        <input
                                            type="number"
                                            min="0"
                                            required
                                            value={formData.unitPrice}
                                            onChange={(e) => setFormData({ ...formData, unitPrice: parseFloat(e.target.value) || 0 })}
                                            className="w-full px-4 py-3 pl-12 rounded-xl border-indigo-200 bg-white focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all shadow-sm font-mono text-indigo-800 text-lg"
                                        />
                                        <span className="absolute left-4 top-3 text-indigo-400 font-bold">₫</span>
                                    </div>
                                </div>
                            </div>
                        </motion.div>
                    </div>
                </div>

                <motion.div variants={itemVariants} className="mt-10 pt-6 border-t border-slate-200 flex justify-end gap-4">
                    <button
                        type="button"
                        onClick={() => navigate('/ingredients')}
                        className="px-6 py-3 rounded-xl font-semibold text-slate-600 bg-slate-100 hover:bg-slate-200 transition-colors"
                    >
                        Hủy bỏ
                    </button>
                    <button
                        type="submit"
                        disabled={loading}
                        className="px-8 py-3 rounded-xl font-bold text-white bg-gradient-to-r from-emerald-500 to-teal-500 hover:from-emerald-600 hover:to-teal-600 shadow-lg shadow-emerald-500/30 flex items-center gap-2 transition-all hover:scale-105 active:scale-95 disabled:opacity-50 disabled:hover:scale-100"
                    >
                        {loading ? (
                            <span className="animate-pulse">Đang ghi nhận...</span>
                        ) : (
                            <>
                                <Save size={20} /> Xác nhận Nhập Kho
                            </>
                        )}
                    </button>
                </motion.div>
            </motion.form>
        </motion.div>
    );
}
