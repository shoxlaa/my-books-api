using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using my_books_api.Data;
using my_books_api.Data.Models;
using my_books_api.Data.Services;
using my_books_api.Execptions;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database config 
builder.Services.AddDbContext<AppDbContext>(options=> 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

//Configure the Service 

builder.Services.AddTransient<BookService>(); 
builder.Services.AddTransient<AuthorsService>();   
builder.Services.AddTransient<PublishersService>();   

//Add Identity 
builder.Services.AddIdentity<ApplicationUser,IdentityRole>().
    AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Add Auhtentication 
builder.Services.AddAuthentication(options =>
{
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;    
})

    //Add JWT Bearer 
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
           
            ValidateIssuer = true , 
            ValidIssuer = builder.Configuration["JWT:Issuer"], 

            ValidateAudience = true ,  
            ValidAudience = builder.Configuration["JWT:Audience"]
        };
    });


var app = builder.Build();
//AppDbInitialer.Seed(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

//Exception handling 
//app.ConfigureBuildInExceptionHandler();
app.ConfigureCustomExceptionHandler();  

app.MapControllers();

app.Run();
