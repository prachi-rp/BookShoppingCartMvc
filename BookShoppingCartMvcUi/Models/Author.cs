using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUi.Models
{
    [Table("Author")]
    public class Author
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string AuthorName { get; set; }
        [Required]
        [MaxLength(10)]
        public int Phone { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        public List<Book> Book { get; set; }
    }
}
