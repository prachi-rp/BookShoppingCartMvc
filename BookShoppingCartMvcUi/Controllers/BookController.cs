using BookShoppingCartMvcUi.Models;
using BookShoppingCartMvcUi.Models.DTOs;
using BookShoppingCartMvcUi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUi.Controllers;

//[Authorize(Roles = nameof(Roles.Admin))]
[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepo;
    private readonly IGenreRepository _genreRepo;
    private readonly IFileService _fileService;
    private readonly IAuthorRepository _authorRepo;

    public BookController(IBookRepository bookRepo, IGenreRepository genreRepo, IFileService fileService, IAuthorRepository authorRepo)
    {
        _bookRepo = bookRepo;
        _genreRepo = genreRepo;
        _fileService = fileService;
        _authorRepo = authorRepo;
    }

    // GET: api/Book/AllBooks
    [HttpGet("AllBooks")]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookRepo.GetBooks();

        if (books == null || !books.Any())
        {
            return NotFound(new { message = "No books found" });
        }

        var bookDtos = books.Select(b => new
        {
            b.Id,
            b.BookName,
            b.Price,
            b.Image,
            b.Description,
            Genre = new
            {
                b.Genre.Id,
                b.Genre.GenreName
            },
            Author = b.Author != null ? new
            {
                b.Author.Id,
                b.Author.AuthorName
            } : null
        }).ToList();

        //return Ok(new { $values = bookDtos });
        return Ok(bookDtos );
    }

    // GET: api/Book/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookRepo.GetBookById(id);

        if (book == null)
        {
            return NotFound(new { message = $"Book with id: {id} not found." });
        }

        var bookDto = new 
        {
            book.Id,
            book.BookName,
            book.Price,
            book.Image,
            book.Description,
            Genre = new
            {
                book.Genre.Id,
                book.Genre.GenreName
            },
            Author =  new
            {
                book.Author.Id,
                book.Author.AuthorName
            } 
        };

        return Ok(bookDto);
    }

    // POST: api/Book/AddBook
    [HttpPost("AddBook")]
    public async Task<IActionResult> AddBook([FromForm] BookDTO bookToAdd)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Handle file upload
            if (bookToAdd.ImageFile != null)
            {
                if (bookToAdd.ImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image file cannot exceed 1 MB." });
                }

                string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                string imageName = await _fileService.SaveFile(bookToAdd.ImageFile, allowedExtensions);
                bookToAdd.Image = imageName;
            }

            // Map BookDTO to Book entity
            var book = new Book
            {
                BookName = bookToAdd.BookName,
                AuthorId = bookToAdd.AuthorId,
                GenreId = bookToAdd.GenreId,
                Price = bookToAdd.Price,
                Image = bookToAdd.Image,
                Description = bookToAdd.Description
            };

            await _bookRepo.AddBook(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while adding the book." });
        }
    }

    // PUT: api/Book/UpdateBook/{id}
    [HttpPut("UpdateBook/{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromForm] BookDTO bookToUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = await _bookRepo.GetBookById(id);
        if (book == null)
        {
            return NotFound(new { message = $"Book with id: {id} not found." });
        }

        try
        {
            // Handle file upload and update image
            string oldImage = book.Image;
            if (bookToUpdate.ImageFile != null)
            {
                if (bookToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image file cannot exceed 1 MB." });
                }

                string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                string newImageName = await _fileService.SaveFile(bookToUpdate.ImageFile, allowedExtensions);
                bookToUpdate.Image = newImageName;
            }

            // Update the book details
            book.BookName = bookToUpdate.BookName;
            book.AuthorId = bookToUpdate.AuthorId;
            book.GenreId = bookToUpdate.GenreId;
            book.Price = bookToUpdate.Price;
            book.Description = bookToUpdate.Description;
            book.Image = bookToUpdate.Image ?? book.Image;

            await _bookRepo.UpdateBook(book);

            if (!string.IsNullOrWhiteSpace(oldImage))
            {
                _fileService.DeleteFile(oldImage);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the book." });
        }
    }

    // DELETE: api/Book/DeleteBook/{id}
    [HttpDelete("DeleteBook/{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _bookRepo.GetBookById(id);
        if (book == null)
        {
            return NotFound(new { message = $"Book with id: {id} not found." });
        }

        try
        {
            await _bookRepo.DeleteBook(book);

            if (!string.IsNullOrWhiteSpace(book.Image))
            {
                _fileService.DeleteFile(book.Image);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the book." });
        }
    }
}






//using BookShoppingCartMvcUi.Models;
//using BookShoppingCartMvcUi.Shared;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace BookShoppingCartMvcUi.Controllers;

//[Authorize(Roles = nameof(Roles.Admin))]
//public class BookController : Controller
//{
//    private readonly IBookRepository _bookRepo;
//    private readonly IGenreRepository _genreRepo;
//    private readonly IFileService _fileService;
//    private readonly IAuthorRepository _authorRepo;

//    public BookController(IBookRepository bookRepo, IGenreRepository genreRepo, IFileService fileService, IAuthorRepository authorRepo)
//    {
//        _bookRepo = bookRepo;
//        _genreRepo = genreRepo;
//        _fileService = fileService;
//        _authorRepo = authorRepo;
//    }

//    public async Task<IActionResult> Index()
//    {
//        var books = await _bookRepo.GetBooks();
//        return View(books);
//    }

//    public async Task<IActionResult> AddBook()
//    {
//        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
//        {
//            Text = genre.GenreName,
//            Value = genre.Id.ToString()
//        });

//        var authorSelectList = (await _authorRepo.GetAuthor()).Select(author => new SelectListItem
//        {
//            Text = author.AuthorName,
//            Value = author.Id.ToString()
//        });

//        BookDTO bookToAdd = new() { GenreList = genreSelectList, AuthorList = authorSelectList };
//        return View(bookToAdd);
//    }

//    [HttpPost]
//    public async Task<IActionResult> AddBook(BookDTO bookToAdd)
//    {
//        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
//        {
//            Text = genre.GenreName,
//            Value = genre.Id.ToString()
//        });
//        bookToAdd.GenreList = genreSelectList;

//        var authorSelectList = (await _authorRepo.GetAuthor()).Select(author => new SelectListItem
//        {
//            Text = author.AuthorName,
//            Value = author.Id.ToString()
//        });
//        bookToAdd.AuthorList = authorSelectList;

//        if (!ModelState.IsValid)
//            return View(bookToAdd);

//        try
//        {
//            if (bookToAdd.ImageFile != null)
//            {
//                if (bookToAdd.ImageFile.Length > 1 * 1024 * 1024)
//                {
//                    throw new InvalidOperationException("Image file can not exceed 1 MB");
//                }
//                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
//                string imageName = await _fileService.SaveFile(bookToAdd.ImageFile, allowedExtensions);
//                bookToAdd.Image = imageName;
//            }
//            // manual mapping of BookDTO -> Book
//            Book book = new()
//            {
//                Id = bookToAdd.Id,
//                BookName = bookToAdd.BookName,
//                AuthorId = bookToAdd.AuthorId,
//                Image = bookToAdd.Image,
//                GenreId = bookToAdd.GenreId,
//                Price = bookToAdd.Price,
//                Description = bookToAdd.Description
//            };
//            await _bookRepo.AddBook(book);
//            TempData["successMessage"] = "Book is added successfully";
//            return RedirectToAction(nameof(AddBook));
//        }
//        catch (InvalidOperationException ex)
//        {
//            TempData["errorMessage"] = ex.Message;
//            return View(bookToAdd);
//        }
//        catch (FileNotFoundException ex)
//        {
//            TempData["errorMessage"] = ex.Message;
//            return View(bookToAdd);
//        }
//        catch (Exception ex)
//        {
//            TempData["errorMessage"] = "Error on saving data";
//            return View(bookToAdd);
//        }
//    }

//    public async Task<IActionResult> UpdateBook(int id)
//    {
//        var book = await _bookRepo.GetBookById(id);
//        if (book == null)
//        {
//            TempData["errorMessage"] = $"Book with the id: {id} does not found";
//            return RedirectToAction(nameof(Index));
//        }
//        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
//        {
//            Text = genre.GenreName,
//            Value = genre.Id.ToString(),
//            Selected = genre.Id == book.GenreId
//        });


//        var authorSelectList = (await _authorRepo.GetAuthor()).Select(author => new SelectListItem
//        {
//            Text = author.AuthorName,
//            Value = author.Id.ToString(),
//            Selected = author.Id == book.AuthorId
//        });

//        BookDTO bookToUpdate = new()
//        {
//            GenreList = genreSelectList,
//            BookName = book.BookName,
//            AuthorId = book.AuthorId,
//            GenreId = book.GenreId,
//            Price = book.Price,
//            Image = book.Image,
//            Description = book.Description
//        };
//        return View(bookToUpdate);
//    }

//    [HttpPost]
//    public async Task<IActionResult> UpdateBook(BookDTO bookToUpdate)
//    {
//        var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
//        {
//            Text = genre.GenreName,
//            Value = genre.Id.ToString(),
//            Selected = genre.Id == bookToUpdate.GenreId
//        });
//        var authorSelectList = (await _authorRepo.GetAuthor()).Select(author => new SelectListItem
//        {
//            Text = author.AuthorName,
//            Value = author.Id.ToString(),
//            Selected = author.Id == bookToUpdate.AuthorId
//        });
//        bookToUpdate.GenreList = genreSelectList;
//        bookToUpdate.AuthorList = authorSelectList;

//        if (!ModelState.IsValid)
//            return View(bookToUpdate);

//        try
//        {
//            string oldImage = "";
//            if (bookToUpdate.ImageFile != null)
//            {
//                if (bookToUpdate.ImageFile.Length > 1 * 1024 * 1024)
//                {
//                    throw new InvalidOperationException("Image file can not exceed 1 MB");
//                }
//                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
//                string imageName = await _fileService.SaveFile(bookToUpdate.ImageFile, allowedExtensions);
//                // hold the old image name. Because we will delete this image after updating the new
//                oldImage = bookToUpdate.Image;
//                bookToUpdate.Image = imageName;
//            }
//            // manual mapping of BookDTO -> Book
//            Book book = new()
//            {
//                Id = bookToUpdate.Id,
//                BookName = bookToUpdate.BookName,
//                AuthorId = bookToUpdate.AuthorId,
//                GenreId = bookToUpdate.GenreId,
//                Price = bookToUpdate.Price,
//                Image = bookToUpdate.Image,
//                Description = bookToUpdate.Description
//            };
//            await _bookRepo.UpdateBook(book);
//            // if image is updated, then delete it from the folder too
//            if (!string.IsNullOrWhiteSpace(oldImage))
//            {
//                _fileService.DeleteFile(oldImage);
//            }
//            TempData["successMessage"] = "Book is updated successfully";
//            return RedirectToAction(nameof(Index));
//        }
//        catch (InvalidOperationException ex)
//        {
//            TempData["errorMessage"] = ex.Message;
//            return View(bookToUpdate);
//        }
//        catch (FileNotFoundException ex)
//        {
//            TempData["errorMessage"] = ex.Message;
//            return View(bookToUpdate);
//        }
//        catch (Exception ex)
//        {
//            TempData["errorMessage"] = "Error on saving data";
//            return View(bookToUpdate);
//        }
//    }

//    public async Task<IActionResult> DeleteBook(int id)
//    {
//        try
//        {
//            var book = await _bookRepo.GetBookById(id);
//            if (book == null)
//            {
//                TempData["errorMessage"] = $"Book with the id: {id} does not found";
//            }
//            else
//            {
//                await _bookRepo.DeleteBook(book);
//                if (!string.IsNullOrWhiteSpace(book.Image))
//                {
//                    _fileService.DeleteFile(book.Image);
//                }
//            }
//        }
//        catch (InvalidOperationException ex)
//        {
//            TempData["errorMessage"] = ex.Message;
//        }
//        catch (FileNotFoundException ex)
//        {
//            TempData["errorMessage"] = ex.Message;
//        }
//        catch (Exception ex)
//        {
//            TempData["errorMessage"] = "Error on deleting the data";
//        }
//        return RedirectToAction(nameof(Index));
//    }

//}