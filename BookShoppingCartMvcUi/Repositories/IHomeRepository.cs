namespace BookShoppingCartMvcUi
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0, int authorId = 0);
        Task<IEnumerable<Genre>> Genres();
        Task<IEnumerable<Author>> Authors();
    }
}