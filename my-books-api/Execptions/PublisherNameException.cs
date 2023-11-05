using my_books_api.Data.ViewModels;
using System.Net;

namespace my_books_api.Execptions
{
    public class CustomExceptionMidlleWare
    {
        private readonly RequestDelegate _next;
        public CustomExceptionMidlleWare(RequestDelegate next )
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = new ErrorVm()
            {
                StatusCode= httpContext.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware",
                Path= "path-goes-here"
            };
            return httpContext.Response.WriteAsync(response.ToString());
        }
    }
    public class PublisherNameException:Exception
    {
        public string PublisherName { get; set; }   

        public PublisherNameException()
        {

        }

        public PublisherNameException(string messege): base (messege)
        {
            
        }
        public PublisherNameException(string messege, Exception inner) : base (messege, inner)
        { }
        public PublisherNameException(string messege, string publsiherName): this (messege)
        {
            PublisherName = publsiherName;
        }
    }
}
