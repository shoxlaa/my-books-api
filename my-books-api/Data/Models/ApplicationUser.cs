using Microsoft.AspNetCore.Identity;

namespace my_books_api.Data.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Custom { get; set; } = "salam";

        
    }
}
