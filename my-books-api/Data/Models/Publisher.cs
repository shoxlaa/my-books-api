using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace my_books_api.Data.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        
        //Navigation Prop
        public List<Book> Books { get; set; }
    }
}
