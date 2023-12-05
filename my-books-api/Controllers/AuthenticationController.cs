using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using my_books_api.Data;
using my_books_api.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace my_books_api.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; 

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _context=context;
            _configuration=configuration;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM  payload)
        {
            var userExists = await _userManager.FindByEmailAsync(payload.UserName);  

            if(userExists != null)
            {
                return BadRequest($"User {payload.Email} already exists");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = payload.Email,
                UserName = payload.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await  _userManager.CreateAsync(newUser, payload.Password);

            if(!result.Succeeded)
            {
                return BadRequest("User could not be created");
            }

            return Created(nameof(Register), $"User {payload.Email} already exists"); 
        }
        public IActionResult Index()
        {
            return View();
        }
    }

    public class RegisterVM
    {
        [Required(ErrorMessage ="Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }    

    }
}
