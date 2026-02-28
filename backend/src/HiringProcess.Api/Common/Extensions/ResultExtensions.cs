using Microsoft.AspNetCore.Mvc;

namespace HiringProcess.Api.Common.Extensions;

/// <summary>
/// Maps Result types to appropriate HTTP responses in controllers.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error.Code switch
        {
            "NotFound"     => new NotFoundObjectResult(new ProblemDetail(result.Error)),
            "Unauthorized" => new UnauthorizedObjectResult(new ProblemDetail(result.Error)),
            "Conflict"     => new ConflictObjectResult(new ProblemDetail(result.Error)),
            "Validation"   => new BadRequestObjectResult(new ProblemDetail(result.Error)),
            _              => new BadRequestObjectResult(new ProblemDetail(result.Error))
        };
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return result.Error.Code switch
        {
            "NotFound"     => new NotFoundObjectResult(new ProblemDetail(result.Error)),
            "Unauthorized" => new UnauthorizedObjectResult(new ProblemDetail(result.Error)),
            "Conflict"     => new ConflictObjectResult(new ProblemDetail(result.Error)),
            "Validation"   => new BadRequestObjectResult(new ProblemDetail(result.Error)),
            _              => new BadRequestObjectResult(new ProblemDetail(result.Error))
        };
    }

    public static IActionResult ToCreatedResult<T>(this Result<T> result, string routeName, object routeValues)
    {
        if (result.IsSuccess)
            return new CreatedAtRouteResult(routeName, routeValues, result.Value);

        return result.ToActionResult();
    }
}

/// <summary>
/// Simplified problem detail for error responses.
/// </summary>
public sealed record ProblemDetail(string Code, string Message)
{
    public ProblemDetail(Error error) : this(error.Code, error.Message) { }
}
