//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;

//namespace LibraryManagement.Middlewares
//{
//    public class ExceptionHandlingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

//        public ExceptionHandlingMiddleware(
//            RequestDelegate next,
//            ILogger<ExceptionHandlingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(
//                    ex,
//                    "An unhandled exception occurred.");

//                await HandleExceptionAsync(context);
//            }
//        }

//        private static async Task HandleExceptionAsync(
//            HttpContext context)
//        {
//            if (context.Response.HasStarted)
//            {
//                return;
//            }

//            context.Response.Clear();

//            context.Response.StatusCode =
//                StatusCodes.Status500InternalServerError;

//            context.Response.ContentType =
//                "text/html; charset=utf-8";

//            await context.Response.WriteAsync(
//                "خطایی در سرور رخ داده است. لطفاً دوباره تلاش کنید.");
//        }
//    }
//}