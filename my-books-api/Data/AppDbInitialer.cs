using my_books_api.Data.Models;

namespace my_books_api.Data
{
    public class AppDbInitialer
    {
        public static void Seed(IApplicationBuilder appBuilder)
        {
            using (var service = appBuilder.ApplicationServices.CreateScope())
            {
                var context = service.ServiceProvider.GetService<AppDbContext>();

                if (!context.Books.Any())
                {
                    context.Books.AddRange(new Book()
                    {
                        Title = "1st Book ",
                        Description = "! des ",
                        IsReaded = false,
                        DateRead = DateTime.Now,
                        Rate = 0,
                      
                        DateAded = DateTime.Now,
                        CoverUrl = "ht"
                    });
                    context.Books.AddRange(new Book()
                    {
                        Title = "2st Book ",
                        Description = " 2 Books des  ",
                        IsReaded = false,
                        DateRead = DateTime.Now.AddDays(20),
                        Rate = 0,
                
                        DateAded = DateTime.Now,
                        CoverUrl = "ht"
                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
