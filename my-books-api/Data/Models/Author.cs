using System.ComponentModel.DataAnnotations;

namespace my_books_api.Data.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }   
        
        //Navigation Perop 
        public List<Book_Author> Book_Author { get; set; }
    }
}
