using my_books_api.Data.Models;
using my_books_api.Data.Paging;
using my_books_api.Data.ViewModels;
using my_books_api.Execptions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace my_books_api.Data.Services
{
    public class PublishersService
    {
        private AppDbContext _context;

        public PublishersService(AppDbContext context)
        {
            _context = context;
        }

        public Publisher AddPublisher(PublisherVm publisherVm)
        {
            if (StirngStartsWithNumber(publisherVm.Name)) throw new PublisherNameException("Name starts with number ", publisherVm.Name);

            var _publisher = new Publisher()
            {
                Name= publisherVm.Name,
            };
            _context.Publishers.Add(_publisher);
            _context.SaveChanges();

            return _publisher;
        }

        public List<Publisher> GetAllPublishers(string sortBy, string searchString, int? pageNumber)
        {
            var allPublsihers = _context.Publishers.ToList();
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        allPublsihers= allPublsihers.OrderByDescending(x => x.Name).ToList();
                        break;

                    default: break;
                }
            }
            //Search 
            if (!string.IsNullOrEmpty(searchString))
            {
                allPublsihers = allPublsihers.Where(x=> x.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList(); 
            }

            // Paging

            int pageSize = 5; 
            allPublsihers = PaginatedList<Publisher>.Create(allPublsihers.AsQueryable(), pageNumber??1, pageSize);

            return allPublsihers;
        }

        public Publisher GetPublisherById(int id) => _context.Publishers.FirstOrDefault(x => x.Id == id);

        public PublisherWithBooksAndAuthorsVm GetPublisherWithBooksAndAuthors(int publisherId)
        {
            var _publisherData = _context.Publishers.Where(x => x.Id == publisherId).Select(x => new PublisherWithBooksAndAuthorsVm()
            {
                Name = x.Name,
                BookAuhthors = x.Books.Select(
                    c => new BookAuhthorVm()
                    {
                        BookName = c.Title,
                        BookAuthors = c.Book_Author.Where(x => x.Book.Title==c.Title).Select(x => x.Author.FullName).ToList()
                    }).ToList()
            }).FirstOrDefault();

            return _publisherData;
        }

        public void DeletePublisherById(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(x => x.Id == id);

            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($" the pubsliher with id: {id} does not exist ");
            }

        }

        public bool StirngStartsWithNumber(string name) => (Regex.IsMatch(name, @"^\d"));

    }
}
