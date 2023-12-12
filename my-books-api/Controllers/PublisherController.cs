using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using my_books_api.Data.Models;
using my_books_api.Data.Services;
using my_books_api.Data.ViewModels;
using my_books_api.Execptions;

namespace my_books_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     public class PublisherController : ControllerBase
    {

        private PublishersService _publisherService;
        public PublisherController(PublishersService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVm publisher)
        {
            try
            {
                var _newPublsiher = _publisherService.AddPublisher(publisher);
                return Created(nameof(AddPublisher), _newPublsiher);
            }
            catch(PublisherNameException ex) 
            {
                return BadRequest($"{ex.Message}, Publisher name :{ex.PublisherName}");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("get-all-publishers"), Authorize]
        public IActionResult GetAllPublishers(string? sortBy, string? searchString, int? page) 
        { 
            
            try
            {
               var _result= _publisherService.GetAllPublishers(sortBy, searchString, page);

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Sorry we couldn't load the publisher Error:{ex.Message}");
            }
        }
        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {
            var response = _publisherService.GetPublisherById(id);

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return NotFound();    
            }
        }

        [HttpGet("get-publisher-books-with-authors/{id}")]
        public IActionResult GetPublisherBooksWithAuthors(int id)
        {
           var response = _publisherService.GetPublisherWithBooksAndAuthors(id);  
            return Ok(response);    
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            _publisherService.DeletePublisherById(id);
            return Ok();
        }
    }
}
