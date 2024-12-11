using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUi.Models
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string? Email { get; set; }
        [Required]
        [MaxLength(10)]
        public string? Phone { get; set; }
        
        public string? Image { get; set; }
        
    }
}
