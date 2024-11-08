using BookShoppingCartMvcUi.Models;
using BookShoppingCartMvcUi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShoppingCartMvcUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm="", int genreId=0, int authorId=0)
        {
            
            IEnumerable<Book> books = await _homeRepository.GetBooks(sterm,genreId, authorId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            IEnumerable<Author> authors = await _homeRepository.Authors();
            BookDisplay bookModel = new BookDisplay
            {
                Books = books,
                Genres = genres,
                Authors = authors,
                STerm = sterm,
                AuthorId = authorId,
                GenreId = genreId
            };
            
            return View(bookModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
