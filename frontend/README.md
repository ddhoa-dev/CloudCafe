# Cafe Management Frontend

Frontend cho hệ thống quản lý quán Cafe được xây dựng với React + TypeScript + Vite + Tailwind CSS.

## Tech Stack

- **React 18** - UI Library
- **TypeScript** - Type Safety
- **Vite** - Build Tool (cực nhanh)
- **Tailwind CSS** - Utility-first CSS
- **React Router** - Routing
- **Zustand** - State Management
- **Axios** - HTTP Client
- **React Hot Toast** - Notifications
- **Lucide React** - Icons

## Cài đặt

```bash
cd frontend
npm install
```

## Chạy Development

```bash
npm run dev
```

Truy cập: http://localhost:3000

## Build Production

```bash
npm run build
```

## Cấu trúc

```
frontend/
├── src/
│   ├── components/      # Reusable components
│   ├── pages/          # Page components
│   ├── services/       # API services
│   ├── stores/         # Zustand stores
│   ├── types/          # TypeScript types
│   ├── App.tsx         # Main app component
│   ├── main.tsx        # Entry point
│   └── index.css       # Global styles
├── public/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── tailwind.config.js
```

## Tính năng

- ✅ Login/Register với JWT
- ✅ Dashboard với thống kê
- ✅ Quản lý sản phẩm
- ✅ Quản lý nguyên liệu (hiển thị cảnh báo sắp hết)
- ✅ Quản lý đơn hàng
- ✅ Tạo đơn hàng (tự động trừ kho)
- ✅ Responsive design
- ✅ Toast notifications
- ✅ Protected routes

## API Proxy

Vite được cấu hình để proxy `/api` requests tới backend:

```
http://localhost:3000/api → http://localhost:5000/api
```

## Environment Variables

Không cần thiết lập environment variables. API URL được cấu hình trong `vite.config.ts`.
