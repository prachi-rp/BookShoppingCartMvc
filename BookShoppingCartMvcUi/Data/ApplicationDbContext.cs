using BookShoppingCartMvcUi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUi.Models.DTOs;

namespace BookShoppingCartMvcUi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<CartDetail> cartDetails { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<OrderStatus> orderStatuses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BookShoppingCartMvcUi.Models.DTOs.GenreDTO> GenreDTO { get; set; } = default!;
    }
}
