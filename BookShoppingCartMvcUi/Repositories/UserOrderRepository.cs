using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookShoppingCartMvcUi.Repositories
{
    public class UserOrderRepository :IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserOrderRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ChangeOrderStatus(UpdateOrderStatusModel data)
        {
            var order = await _db.orders.FindAsync(data.OrderId);
            if(order == null)
            {
                throw new InvalidOperationException($"order with id: {data.OrderId} does not found");
            }
            order.OrderStatusId = data.OrderStatusId;
            await _db.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _db.orders.FindAsync(id);
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _db.orderStatuses.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await _db.orders.FindAsync(orderId);
            if(order == null )
            {
                throw new InvalidOperationException($"order with id:{orderId} does not found");
            }
            order.IsPaid = !order.IsPaid;
            await _db.SaveChangesAsync();
        }

        public async Task<List<OrderDto>> UserOrders(bool getAll = false)
        {
            var ordersQuery = _db.orders
        .Include(x => x.OrderStatus)
        .Include(x => x.OrderDetail)
        .ThenInclude(x => x.Book)
        .ThenInclude(x => x.Genre)
        .AsQueryable();

            if (!getAll)
            {
                var userId = GetUserId();
                if (userId == null)
                    throw new Exception("User is not Logged-In");

                ordersQuery = ordersQuery.Where(x => x.UserId == userId);
            }

            // Project into flattened structure
            return (List<OrderDto>)(IEnumerable<OrderDto>)await ordersQuery.Select(order => new OrderDto
            {
                Id = order.Id,
                CreateDate = order.CreateDate,
                Name = order.Name,
                Email = order.Email,
                MobileNumber = order.MobileNumber,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                IsPaid = order.IsPaid,
                OrderStatus = order.OrderStatus.StatusName,
                OrderDetails = order.OrderDetail.Select(detail => new OrderDetailDto
                {
                    BookName = detail.Book.BookName,
                    Quantity = detail.Quantity,
                    UnitPrice = (decimal)detail.UnitPrice,
                    Image = detail.Book.Image
                }).ToList()
            }).ToListAsync();
        }

        private int? GetUserId()
        {
            //var principal = 
            //int userId = _userManager.GetUserId(principal);
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            return userId;
        }
    }
}
