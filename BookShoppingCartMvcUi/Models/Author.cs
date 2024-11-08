using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUi.Models
{
    [Table("Author")]
    public class Author
    {
        public int Id { get; set; }
        public String AuthorName { get; set; }
        public String Image { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public List<Book> Book { get; set; }
    }
}
