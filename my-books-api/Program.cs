using Microsoft.EntityFrameworkCore;
using my_books_api.Data;
using my_books_api.Data.Services;
using my_books_api.Execptions;
using Serilog;

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
