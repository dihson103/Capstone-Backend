﻿using Domain.Abstractions.Exceptions.Base;

namespace WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (MyException excepion)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = excepion.StatusCode;

            var error = new
            {
                Status = excepion.StatusCode,
                Message = excepion.Message,
                Error = excepion.Error
            };

            await context.Response.WriteAsJsonAsync(error);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var error = new
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = "Internal Server Error"
            };

            Console.WriteLine(ex.StackTrace);

            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
