using Microsoft.Identity.Client;
using my_books_api.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace my_books_api.Data.ViewModels
{
    public class PublisherVm
    {
        public string Name { get; set; }
    }
    public class PublisherWithBooksAndAuthorsVm
    {
        public string Name { get; set; }
        public List<BookAuhthorVm> BookAuhthors { get; set; }
    }

    public class BookAuhthorVm
    {
        public string BookName { get; set; }
        public List<string> BookAuthors { get; set; }

    }
}