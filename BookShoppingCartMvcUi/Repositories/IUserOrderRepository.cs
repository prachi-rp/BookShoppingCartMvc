﻿namespace BookShoppingCartMvcUi.Repositories
{
    public interface IUserOrderRepository
    {
        Task<List<OrderDto>> UserOrders(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusModel data);
        Task TogglePaymentStatus(int orderId);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses();
    }
}