using my_books_api.Data.Models;
using my_books_api.Data.ViewModels;

namespace my_books_api.Data.Services
{
    public class AuthorsService
    {
        private AppDbContext _context;

        public AuthorsService(AppDbContext context)
        {
            _context = context;
        }

        public void AddAuthor(AuthorVm authorVm)
        {

            var _author = new Author()
            {
                FullName = authorVm.FullName,
            };
            _context.Authors.Add(_author);
            _context.SaveChanges();
        }


        public AuthorWithBooksVm GetAuthorWithBooks(int authorId)
        {
            var _author =  _context.Authors.Where(n=> n.Id == authorId).Select(n=> new AuthorWithBooksVm() {
                FullName=n.FullName, 
                BookTitle=n.Book_Author.Select(n=> n.Book.Title).ToList() }).FirstOrDefault();


            return _author;
        }

    }
}
