using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Api.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            var failures = ex.Errors?.ToList() ?? new();
            var errors = failures.Count > 0
                ? failures.GroupBy(e => e.PropertyName ?? string.Empty)
                          .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                : new Dictionary<string, string[]> { [""] = new[] { ex.Message } };

            await WriteJson(ctx, StatusCodes.Status400BadRequest, new
            {
                title = "Validation failed",
                status = StatusCodes.Status400BadRequest,
                errors
            });
        }
        catch (BadHttpRequestException ex)
        {
            await WriteProblem(ctx, new ProblemDetails { Title = "Bad request", Detail = ex.Message, Status = 400 });
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(ctx, new ProblemDetails { Title = "Not found", Detail = ex.Message, Status = 404 });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict");
            await WriteProblem(ctx, new ProblemDetails { Title = "Concurrency conflict", Status = 409 });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database update error");
            await WriteProblem(ctx, new ProblemDetails
            {
                Title = "Database update error",
                Detail = ex.InnerException?.Message ?? ex.Message,
                Status = 400
            });
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            ctx.Response.StatusCode = 499; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(ctx, new ProblemDetails
            {
                Title = "Server error",
                Detail = "An unexpected error occurred.",
                Status = 500
            });
        }
    }

    private static async Task WriteProblem(HttpContext ctx, ProblemDetails pd)
        => await WriteJson(ctx, pd.Status ?? 500, new
        {
            type = pd.Type,
            title = pd.Title,
            status = pd.Status,
            detail = pd.Detail,
            pd.Instance
        });

    private static async Task WriteJson(HttpContext ctx, int statusCode, object payload)
    {
        if (ctx.Response.HasStarted) return;
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = statusCode;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOpts));
    }
}
