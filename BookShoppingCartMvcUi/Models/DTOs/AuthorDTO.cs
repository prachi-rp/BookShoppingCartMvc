using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUi.Models.DTOs
{
    public class AuthorDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string AuthorName { get; set; }
        [Required]
        [Range(0, 9999999999)]
        public int Phone { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
