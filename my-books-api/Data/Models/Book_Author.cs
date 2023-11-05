using System.ComponentModel.DataAnnotations;

namespace my_books_api.Data.Models
{
    public class Book_Author
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; } 
        public Book Book { get; set; }
        public int AuthorId { get; set; }   
        public Author Author { get; set; }  
    }
}
