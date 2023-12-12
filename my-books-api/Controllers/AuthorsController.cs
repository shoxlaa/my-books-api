using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_books_api.Data.Models;
using my_books_api.Data.Services;
using my_books_api.Data.ViewModels;

namespace my_books_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private AuthorsService _authorsService; 
        public AuthorsController (AuthorsService authorsService)
        {
            _authorsService = authorsService;
        }

        [HttpPost("add-author")]
        public IActionResult AddAuthor([FromBody] AuthorVm author)
        {
            _authorsService.AddAuthor(author);
            return Ok();
        }

        [HttpGet("get-author-books-by-id/{id}")]
        public IActionResult GetAuthorWithBooks(int id)
        {
            var response = _authorsService.GetAuthorWithBooks(id);
            return Ok(response);
        } 

    }
}
