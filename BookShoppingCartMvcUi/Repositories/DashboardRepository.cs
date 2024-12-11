using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUi.Repositories
{
    public class DashboardRepository: IDashboardRepository
    {
        private readonly ApplicationDbContext _db;

        public DashboardRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> CountOrders()
        {
            var data =  (from order in _db.orders
                              select new { order.Id }
                              ).ToList();
            var orderCount=  data.Count;
            return orderCount;
        }
        public async Task<int> TotalAmounts()
        {
            var data = await (from order in _db.orderDetails
                        select new { order.Quantity, order.UnitPrice }
                              ).ToListAsync();
            // var orderCount = data.Count;
            var total = 0;
            if(data is not null)
            {
                foreach (var item in data)
                {
                    total += item.Quantity * (int)item.UnitPrice;
                }
            }
            return total;
        }
        public async Task<int> CountBooks()
        {
            var data = (from Book in _db.Books
                        select new { Book.Id }
                              ).ToList();
            var orderCount = data.Count;
            return orderCount;
        }
        public async Task<int> CountAuthors()
        {
            var data = (from Authors in _db.Authors
                        select new { Authors.Id }
                              ).ToList();
            var orderCount = data.Count;
            return orderCount;
        }
        public async Task<int> CountGenres()
        {
            var data = (from Genres in _db.Genres
                        select new { Genres.Id }
                              ).ToList();
            var orderCount = data.Count;
            return orderCount;
        }
        public async Task<int> CountCustomers()
        {
            var data = (from user in _db.Users
                        select new { user.Id }
                              ).ToList();
            var orderCount = data.Count;
            return orderCount;
        }

    }
    public interface IDashboardRepository
    {
        Task<int> CountOrders();
        Task<int> TotalAmounts();
        Task<int> CountBooks();
        Task<int> CountAuthors();
        Task<int> CountGenres();
        Task<int> CountCustomers();
       
    }
}
