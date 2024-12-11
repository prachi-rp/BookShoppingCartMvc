using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUi.Models.DTOs
{
    public class loginDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
