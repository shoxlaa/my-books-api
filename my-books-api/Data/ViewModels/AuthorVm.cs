namespace my_books_api.Data.ViewModels
{
    public class AuthorVm
    {
        public string FullName { get; set; }
    }
    // View Model 
    public class AuthorWithBooksVm
    {
        public string FullName { get; set;}
        public List<string> BookTitle { get; set;}  

    }

    // для начала нужно понять зачем мне писать все эти View Models  
    // 


}