using my_books_api.Data.Models;
using my_books_api.Data.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace my_books_api.Data.Services
{
    public class BookService
    {
        private AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public void AddBookWithAuthors(BookVM bookVm)
        {
            // создаем книгу и кидаем её в бд 

            var _book = new Book()
            {
                Title = bookVm.Title,
                Description = bookVm.Description,
                CoverUrl = bookVm.CoverUrl,
                DateAded = bookVm.DateAded,
                DateRead = bookVm.IsReaded ? bookVm.DateRead.Value : null,
                IsReaded = bookVm.IsReaded,
                Rate = bookVm.IsReaded ? bookVm.Rate.Value : null, 
                PublisherId = bookVm.PublisherId
            };
            _context.Books.Add(_book);
            _context.SaveChanges();    
            
     
            // данные о книге и о авторах добавляем в бд
            //
            foreach( var id in bookVm.AuthorId )
            {
                var _book_author = new Book_Author()
                {
                    BookId = _book.Id,
                    AuthorId = id
                };
                _context.AuthorBooks.Add(_book_author);
                _context.SaveChanges();

            }
        }

        public List<Book> GetAllBooks()=> _context.Books.ToList();

        // view model of Book with authors
        public BookWithAuthorsVm? GetBookById(int bookid)
        {
            var _bookWithAuthors = _context.Books.Where(n=>n.Id ==bookid).Select(bookVm => new BookWithAuthorsVm()
            {
                Title = bookVm.Title,
                Description = bookVm.Description,
                CoverUrl = bookVm.CoverUrl,
                DateRead = bookVm.IsReaded ? bookVm.DateRead.Value : null,
                IsReaded = bookVm.IsReaded,
                Rate = bookVm.IsReaded ? bookVm.Rate.Value : null,
                PublisherName = bookVm.Publisher.Name, 
                AuthorNames = bookVm.Book_Author.Select(n=> n.Author.FullName).ToList()  
            }).FirstOrDefault();

            return _bookWithAuthors;
        }

        public Book UpdateBookById(int Id, BookVM bookVm) 
        {
            var _book = _context.Books.FirstOrDefault(x=> x.Id ==Id);
            if (_book != null)
            {
                _book.Title = bookVm.Title;
                _book.Description = bookVm.Description;
                _book.CoverUrl = bookVm.CoverUrl;
                _book.DateAded = bookVm.DateAded;
                _book.DateRead = bookVm.IsReaded ? bookVm.DateRead.Value : null;
                _book.IsReaded = bookVm.IsReaded;
                _book.Rate = bookVm.IsReaded ? bookVm.Rate.Value : null;

                _context.SaveChanges() ;
            }
            return _book; 
        }

        public void DeleteById(int id)
        {
            var book= _context.Books.FirstOrDefault(x => x.Id == id);
            if (book != null) {

                _context.Remove(book);
                _context.SaveChanges();
            }

        }
    }
}
