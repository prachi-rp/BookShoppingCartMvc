using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BookShoppingCartMvcUi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int bookId, int qty)
        {
            int? userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User is not Logged-In");
                }
                var cart = await GetCart((int)userId);
                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = (int)userId
                    };
                    _db.shoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart details section
                var cartItem = _db.cartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price
                    };
                    _db.cartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex) {
            }
            var cartItemCount = await GetCartItemCount((int)userId);
            return cartItemCount;
        }


        public async Task<int> RemoveItem(int bookId)
        {
            //using var transaction = _db.Database.BeginTransaction();
            int? userId = GetUserId();
            try
            {
                if (userId == null)
                    throw new UnauthorizedAccessException("User is not Logged-In");
                
                var cart = await GetCart((int)userId);
                if (cart == null)
                   throw new InvalidOperationException("Invalid Cart");
                
                // cart details section
                var cartItem = _db.cartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                    throw new InvalidOperationException("Not items in cart");
                else if (cartItem.Quantity == 1) 
                    _db.cartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
                //transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount((int)userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid UserId");
            var shoppingcart = await _db.shoppingCarts
                                .Include(a => a.CartDetails)
                                .ThenInclude(a => a.Book)
                                .ThenInclude(a => a.Stock)
                                .Include(a => a.CartDetails)
                                .ThenInclude(a => a.Book)
                                .ThenInclude(a => a.Genre)
                                .Where(a=>a.UserId == userId).FirstOrDefaultAsync();
            return shoppingcart;

        }

        public async Task<ShoppingCart> GetCart(int userId)
        {
            var cart = await _db.shoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> GetCartItemCount(int userId=0)
        {
            if(userId==null)
            {
                userId = (int) GetUserId();
            }
            var data = await (from cart in _db.shoppingCarts
                              join cartDetail in _db.cartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              where cart.UserId==userId
                              select new {cartDetail.Id}
                              ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                //move data from cartDeetail to order and order detail then we will remove cart detail
                //entry->Order, order details
                //remove data->cart details
                var userId = GetUserId();
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User is not Logged-in");
                }
                var cart = await GetCart((int)userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid Cart");
                var cartDetail = _db.cartDetails
                                            .Where(a=>a.ShoppingCartId==cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Cart is Empty");
                var pendingRecord = _db.orderStatuses.FirstOrDefault(s => s.StatusName == "Pending");  
                if(pendingRecord is null)
                    throw new InvalidOperationException("Order Status does not have Pending status");
                var order = new Order
                {
                    UserId = (int)userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email= model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address= model.Address,
                    IsPaid=false,
                    OrderStatusId = pendingRecord.Id

                };
                _db.orders.Add(order);
                _db.SaveChanges();

                foreach(var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookID = item.BookId,
                        OrderID = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.orderDetails.Add(orderDetail);

                    //update stock here
                    var stock = await _db.Stocks.FirstOrDefaultAsync(
                        a => a.BookId == item.BookId);
                    if(stock == null)
                    {
                        throw new InvalidOperationException("Stock is null");
                    }

                    if(item.Quantity > stock.Quantity)
                    {
                        throw new InvalidOperationException($"Only {stock.Quantity} item(s) are available in the stock");
                    }
                    //decrease the num of quantity from the stock table
                    stock.Quantity -= item.Quantity;


                }

                

                _db.SaveChanges();

                //removing cartDetails
                _db.cartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private int? GetUserId()
        {
            //var principal = _httpContextAccessor.HttpContext.User;
            //string userId = _userManager.GetUserId(principal);
            //return userId;

            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            return userId;
        }
    }
}
