using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using my_books_api.Data;
using my_books_api.Data.Models;
using my_books_api.Data.Services;
using my_books_api.Execptions;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database config 
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

//Configure the Service 

builder.Services.AddTransient<BookService>();
builder.Services.AddTransient<AuthorsService>();
builder.Services.AddTransient<PublishersService>();
// Token ValidationParams 

var tokenValidationParametrs = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWT:Secret").Value!)),

    ValidateIssuer = true,
    ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value!,

    ValidateAudience = true,
    ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value!,

    ValidateLifetime = true , 
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParametrs); 


//Add Identity 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
    AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Add Auhtentication 

builder.Services.AddSwaggerGen(options =>
{
    //  add authorize  the authorize button 
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standart Authorization header using Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme= JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
       options. IncludeErrorDetails = true;
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParametrs;
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
//Authentication & Authorization
app.UseAuthentication();

app.UseAuthorization();

//Exception handling 
//app.ConfigureBuildInExceptionHandler();
app.ConfigureCustomExceptionHandler();

app.MapControllers();

app.Run();





