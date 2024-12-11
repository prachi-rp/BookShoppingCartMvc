using BookShoppingCartMvcUi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookShoppingCartMvcUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepo;

        public AuthorController(IAuthorRepository authorRepo)
        {
            _authorRepo = authorRepo;
        }

        // GET: api/Author/AllAuthors
        [HttpGet("AllAuthors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorRepo.GetAuthor();
            return Ok(authors); // Returns a JSON response with the list of authors
        }

        // GET: api/Author/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorRepo.GetAuthorById(id);
            if (author == null)
            {
                return NotFound(new { message = $"Author with id: {id} not found." });
            }
            return Ok(author);
        }

        // POST: api/Author/AddAuthor
        [HttpPost("AddAuthor")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorDTO author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns validation errors
            }
            try
            {
                var authorToAdd = new Author
                {
                    AuthorName = author.AuthorName,
                    Id = author.Id,
                    Phone = author.Phone,
                    Email = author.Email,
                    Gender = author.Gender
                };
                await _authorRepo.AddAuthor(authorToAdd);
                return CreatedAtAction(nameof(GetAuthorById), new { id = authorToAdd.Id }, authorToAdd);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Author could not be added due to a server error." });
            }
        }

        // PUT: api/Author/UpdateAuthor
        [HttpPut("UpdateAuthor/{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorDTO authorToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = await _authorRepo.GetAuthorById(id);
            if (author == null)
            {
                return NotFound(new { message = $"Author with id: {id} not found." });
            }

            author.AuthorName = authorToUpdate.AuthorName;
            author.Phone = authorToUpdate.Phone;
            author.Email = authorToUpdate.Email;
            author.Gender = authorToUpdate.Gender;

            try
            {
                await _authorRepo.UpdateAuthor(author);
                return NoContent(); // Successfully updated, no content to return
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Author could not be updated due to a server error." });
            }
        }

        // DELETE: api/Author/DeleteAuthor/{id}
        [HttpDelete("DeleteAuthor/{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _authorRepo.GetAuthorById(id);
            if (author == null)
            {
                return NotFound(new { message = $"Author with id: {id} not found." });
            }

            try
            {
                await _authorRepo.DeleteAuthor(author);
                return NoContent(); // Successfully deleted, no content to return
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Author could not be deleted due to a server error." });
            }
        }
    }
}







//using BookShoppingCartMvcUi.Models.DTOs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace BookShoppingCartMvcUi.Controllers
//{
//    [Authorize(Roles = nameof(Roles.Admin))]
//    public class AuthorController : Controller
//    {
//        private readonly IAuthorRepository _authorRepo;

//        public AuthorController(IAuthorRepository authorRepo)
//        {
//            _authorRepo = authorRepo;
//        }
//        public async Task<IActionResult> Index()
//        {
//            var authors= await _authorRepo.GetAuthor();
//            return View(authors);
//        }

//        public IActionResult AddAuthor()
//        {
//            return View();
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddAuthor(AuthorDTO author)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(author);
//            }
//            try
//            {
//                var authorToAdd = new Author { AuthorName = author.AuthorName, 
//                                                Id = author.Id,
//                                                Phone =author.Phone,
//                                                Email =author.Email,
//                                                Gender =author.Gender
//                                            };
//                await _authorRepo.AddAuthor(authorToAdd);
//                TempData["successMessage"] = "Author added successfully";
//                return RedirectToAction(nameof(AddAuthor));
//            }
//            catch (Exception ex)
//            {
//                TempData["errorMessage"] = "Author could not added!";
//                return View(author);
//            }
//        }

//        public async Task<IActionResult> UpdateAuthor(int id)
//        {
//            var author = await _authorRepo.GetAuthorById(id);
//            if (author is null)
//                throw new InvalidOperationException($"Author with id: {id} does not found");
//            var authorToUpdate = new AuthorDTO
//            {
//                Id = author.Id,
//                AuthorName = author.AuthorName,
//                Phone = author.Phone,
//                Email = author.Email,
//                Gender = author.Gender
//            };
//            return View(authorToUpdate);
//        }
//        [HttpPost]
//        public async Task<IActionResult> UpdateAuthor(AuthorDTO authorToUpdate)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(authorToUpdate);
//            }
//            try
//            {
//                var author = new Author
//                {
//                    AuthorName = authorToUpdate.AuthorName,
//                    Id = authorToUpdate.Id,
//                    Phone = authorToUpdate.Phone,
//                    Email = authorToUpdate.Email,
//                    Gender = authorToUpdate.Gender
//                };
//                await _authorRepo.UpdateAuthor(author);
//                TempData["successMessage"] = "Author updated successfully";
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                TempData["errorMessage"] = "Author could not updated!";
//                return View(authorToUpdate);
//            }
//        }

//        public async Task<IActionResult> DeleteAuthor(int id)
//        {
//            var author = await _authorRepo.GetAuthorById(id);
//            if (author is null)
//                throw new InvalidOperationException($"Author with id: {id} does not found");
//            await _authorRepo.DeleteAuthor(author);
//            return RedirectToAction(nameof(Index));
//        }
//    }

//}
