namespace BookShoppingCartMvcUi.Models.DTOs
{
    public class BookDisplay
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
        public int AuthorId { get; set; } = 0;

    }
}
