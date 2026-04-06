namespace CafeManagement.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,      // Chờ xử lý
    Preparing = 2,    // Đang pha chế
    Ready = 3,        // Sẵn sàng
    Completed = 4,    // Hoàn thành
    Cancelled = 5     // Đã hủy
}
