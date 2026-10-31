using CopaPassenger.API.Models;
using CopaPassenger.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace CopaPassenger.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FlightNotFoundException ex)
        {
            await WriteError(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ReservationNotFoundException ex)
        {
            await WriteError(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DuplicateAncillaryServiceException ex)
        {
            await WriteError(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ValidationException ex)
        {
            var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
            await WriteError(context, StatusCodes.Status400BadRequest, errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado");
            await WriteError(context, StatusCodes.Status500InternalServerError, "Error interno del servidor.");
        }
    }

    private static async Task WriteError(HttpContext ctx, int statusCode, string message)
    {
        ctx.Response.StatusCode = statusCode;
        ctx.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(ApiResponse<object>.Fail(message));
        await ctx.Response.WriteAsync(body);
    }
}
