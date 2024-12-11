using BookShoppingCartMvcUi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genreRepo;

        public GenreController(IGenreRepository genreRepo)
        {
            _genreRepo = genreRepo;
        }

        // GET: api/Genre/AllGenres
        [HttpGet("AllGenres")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreRepo.GetGenres();
            return Ok(genres);
        }

        // GET: api/Genre/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre == null)
            {
                return NotFound(new { message = $"Genre with id: {id} not found." });
            }
            return Ok(genre);
        }

        // POST: api/Genre/AddGenre
        [HttpPost("AddGenre")]
        public async Task<IActionResult> AddGenre([FromBody] GenreDTO genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var genreToAdd = new Genre { GenreName = genre.GenreName, Id = genre.Id };
                await _genreRepo.AddGenre(genreToAdd);
                return CreatedAtAction(nameof(GetGenreById), new { id = genreToAdd.Id }, genreToAdd);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Genre could not be added due to a server error." });
            }
        }

        // PUT: api/Genre/UpdateGenre/{id}
        [HttpPut("UpdateGenre/{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreDTO genreToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var genre = await _genreRepo.GetGenreById(id);
            if (genre == null)
            {
                return NotFound(new { message = $"Genre with id: {id} not found." });
            }

            genre.GenreName = genreToUpdate.GenreName;

            try
            {
                await _genreRepo.UpdateGenre(genre);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Genre could not be updated due to a server error." });
            }
        }

        // DELETE: api/Genre/DeleteGenre/{id}
        [HttpDelete("DeleteGenre/{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre == null)
            {
                return NotFound(new { message = $"Genre with id: {id} not found." });
            }

            try
            {
                await _genreRepo.DeleteGenre(genre);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Genre could not be deleted due to a server error." });
            }
        }
    }
}








//using BookShoppingCartMvcUi.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace BookShoppingCartMvcUi.Controllers
//{
//    //[Authorize(Roles =nameof(Roles.Admin))]
//    public class GenreController : Controller
//    {
//        private readonly IGenreRepository _genreRepo;

//        public GenreController(IGenreRepository genreRepo)
//        {
//            _genreRepo = genreRepo;
//        }
//        public async Task<IActionResult> Index()
//        {
//            var genres = await _genreRepo.GetGenres();
//            return View(genres);
//        }

//        public IActionResult AddGenre()
//        {
//            return View();
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddGenre(GenreDTO genre)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(genre);
//            }
//            try
//            {
//                var genreToAdd = new Genre { GenreName = genre.GenreName, Id = genre.Id };
//                await _genreRepo.AddGenre(genreToAdd);
//                TempData["successMessage"] = "Genre added successfully";
//                return RedirectToAction(nameof(AddGenre));
//            }
//            catch (Exception ex)
//            {
//                TempData["errorMessage"] = "Genre could not added!";
//                return View(genre);
//            }
//        }

//        public async Task<IActionResult> UpdateGenre(int id)
//        {
//            var genre = await _genreRepo.GetGenreById(id);
//            if (genre is null)
//                throw new InvalidOperationException($"Genre with id: {id} does not found");
//            var genreToUpdate = new GenreDTO
//            {
//                Id = genre.Id,
//                GenreName = genre.GenreName
//            };
//            return View(genreToUpdate);
//        }
//        [HttpPost]
//        public async Task<IActionResult> UpdateGenre(GenreDTO genreToUpdate)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(genreToUpdate);
//            }
//            try
//            {
//                var genre = new Genre { GenreName = genreToUpdate.GenreName, Id = genreToUpdate.Id };
//                await _genreRepo.UpdateGenre(genre);
//                TempData["successMessage"] = "Genre updated successfully";
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                TempData["errorMessage"] = "Genre could not updated!";
//                return View(genreToUpdate);
//            }
//        }

//        public async Task<IActionResult> DeleteGenre(int id)
//        {
//            var genre = await _genreRepo.GetGenreById(id);
//            if (genre is null)
//                throw new InvalidOperationException($"Genre with id: {id} does not found");
//            await _genreRepo.DeleteGenre(genre);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
