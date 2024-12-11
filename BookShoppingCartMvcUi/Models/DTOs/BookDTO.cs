using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingCartMvcUi.Models.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? BookName { get; set; }

        [Required]
        public int AuthorId { get; set; }



        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        [Required]
        public int GenreId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
        public IEnumerable<SelectListItem>? AuthorList { get; set; }

    }
}
