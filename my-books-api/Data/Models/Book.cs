namespace my_books_api.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsReaded { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string CoverUrl { get; set; }
        public DateTime DateAded { get; set; }

        //Navigation Prop
        public int PublisherId { get; set; }    
        public Publisher Publisher { get; set; }
        public List<Book_Author> Book_Author { get; set; }

    }
}
