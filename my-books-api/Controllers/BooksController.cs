using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books_api.Data.Services;
using my_books_api.Data.ViewModels;

namespace my_books_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        public BookService _bookService; 

        public BooksController(BookService bookService) 
        {
            _bookService = bookService;
        }
        [HttpPost("add-book-with-authors")]
        public IActionResult AddBook([FromBody] BookVM book)
        {
            _bookService.AddBookWithAuthors(book);
            return Ok();
        }

        [HttpGet("get-all-books")] 
        public IActionResult GetAllBooks()
        {
            var allBooks= _bookService.GetAllBooks();
            return Ok(allBooks);
        }
        [HttpGet("get-book-by-id/{id}")]
        public IActionResult GetBook(int id) 
        {
            var book = _bookService.GetBookById(id);
            return Ok(book);
        }

        [HttpPut("update-book-by-id/{id}")] 

        public IActionResult UpdateBookById(int id , [FromBody] BookVM book) {

            var updateBook = _bookService.UpdateBookById(id, book); 
            return Ok(updateBook);

        }


        [HttpDelete("delete-book-by-id/{id}")]

        public IActionResult DeleteBookById(int id)
        {
            _bookService.DeleteById(id);
            return Ok();

        }        
    }
  
}
