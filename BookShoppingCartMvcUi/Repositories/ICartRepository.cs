namespace BookShoppingCartMvcUi.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(int userId = 0);
        Task<ShoppingCart> GetCart(int userId);
        Task<bool> DoCheckout(CheckoutModel model);
    }
}