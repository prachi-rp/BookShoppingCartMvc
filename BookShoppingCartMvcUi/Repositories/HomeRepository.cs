
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUi.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Author>> Authors()
        {
            return await _db.Authors.ToListAsync();
        }

        public async Task<IEnumerable<Genre>> Genres() 
        { 
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm="", int genreId=0, int authorId=0) 
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Book> books = await (from book in _db.Books
                         join genre in _db.Genres
                         on book.GenreId equals genre.Id
                         join author in _db.Authors
                         on book.AuthorId equals author.Id 
                         join stock in _db.Stocks
                         on book.Id equals stock.BookId
                         into book_stocks
                         from bookWithStock in book_stocks.DefaultIfEmpty()
                         where string.IsNullOrWhiteSpace(sTerm) || (book!=null && book.BookName.ToLower().StartsWith(sTerm))
                         select new Book
                         {
                             Id = book.Id,
                             Image = book.Image,
                             AuthorId = book.AuthorId,
                             BookName = book.BookName,
                             GenreId = book.GenreId,
                             Price = book.Price,
                             Description = book.Description,
                             GenreName = genre.GenreName,
                             AuthorName = author.AuthorName,
                             Quantity = bookWithStock==null?
                             0 : bookWithStock.Quantity
                         }
                         ).ToListAsync();
            if(genreId>0)
            {
                books=  books.Where(a=>a.GenreId==genreId).ToList();
            }
            if (authorId > 0)
            {
                books = books.Where(b => b.AuthorId == authorId).ToList();
            }
            return books;
        }
    }
}
