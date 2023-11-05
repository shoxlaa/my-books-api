namespace my_books_api.Data.ViewModels
{
    public class BookVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsReaded { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string CoverUrl { get; set; }
        public DateTime DateAded { get; set; }

        //navigation prop
        public int PublisherId { get; set; }    
        public List<int> AuthorId { get; set;}
    }

    public class BookWithAuthorsVm
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsReaded { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string CoverUrl { get; set; }

        //navigation prop
        public string PublisherName { get; set; }
        public List<string> AuthorNames { get; set; }
    }
}
