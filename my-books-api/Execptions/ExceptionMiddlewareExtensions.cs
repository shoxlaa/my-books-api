using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using my_books_api.Data.ViewModels;
using System.Net;
using System.Runtime.CompilerServices;

namespace my_books_api.Execptions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureBuildInExceptionHandler (this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode =(int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();  
                    var contextRequest = context.Features.Get<IHttpRequestFeature>();   

                    if (contextRequest != null)
                    {
                        await context.Response.WriteAsync(new ErrorVm()
                        {
                            StatusCode= context.Response.StatusCode,
                            Message= contextFeature.Error.Message,
                            Path= contextRequest.Path

                        }.ToString());
                    }
                });
            });
        } 

        public static void ConfigureCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMidlleWare>();
        }
    }
}
