using System.Net;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text.Json;
using WebAPIAFA.Helpers;

namespace WebAPIAFA.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; //next request in the pipeline...
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger,
                                   IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //Try to execute the next request in the pipeline...
            }
            catch (Exception ex) //Everthing that fails will be catched here...
            {
                //Loggear la exception en OS...
                _logger.LogError(ex, message: "An exception has occurred: {ExceptionMessage}", ex.Message);

                //Loggear la exception en nuestra db...
                var projectName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
                var classFullName = ex.TargetSite?.ReflectedType?.FullName ?? string.Empty;
                DbLog.SaveLog(ex.HResult.ToString(), projectName, $"Message: {ex.Message} || StackTrace: {ex.StackTrace?.ToString()}", classFullName);

                //Preparar la respuesta al cliente...
                var response = new ResponseObjectJsonDto();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Code = 500;
                response.Message = "Error procesando la operación";

                //Send all the stacktrace if were are on dev...
                if (_env.IsDevelopment())
                {
                    response.Message += $": {ex.Message}";
                    response.Response = $"Error: {ex.StackTrace?.ToString()}";
                }
                else
                {
                    response.Message += $"Internal Server Error.";
                }

                ///Serializar la response...
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);


                //Escribir la response al cliente..
                await context.Response.WriteAsync(json);
            }
        }
    }
}
