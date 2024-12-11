using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUi.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }

    }
}
