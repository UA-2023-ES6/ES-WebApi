using Microsoft.AspNetCore.Mvc;
using OneCampus.Domain.Exceptions;
using System.Net.Mime;

namespace OneCampus.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await ErrorResponseAsync(context, StatusCodes.Status400BadRequest, "Bad request", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await ErrorResponseAsync(context, StatusCodes.Status404NotFound, "Not found", ex.Message);
        }
        catch (Exception ex)
        {
            await ErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "internal server error", ex.Message);
        }
    }

    public async Task ErrorResponseAsync(HttpContext context, int statusCode, string title, string message)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;

        var response = new ProblemDetails
        {
            Status = statusCode,
            Detail = message,
            Title = title
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
