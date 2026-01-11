using LunaArch.Abstractions.Common;
using LunaArch.Abstractions.Results;
using LunaArch.AspNetCore.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace LunaArch.AspNetCore.MinimalApi;

/// <summary>
/// Extension methods for mapping Result types to IResult for Minimal APIs.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an appropriate IResult response.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>An IResult representing the outcome.</returns>
    public static IResult ToResponse<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? HttpResults.Ok(ApiResponse<T>.Ok(result.Value))
            : HttpResults.BadRequest(ApiResponse<T>.Fail(result.Error ?? "An error occurred"));
    }

    /// <summary>
    /// Converts a Result to an appropriate IResult response with a created location.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="uri">The URI of the created resource.</param>
    /// <returns>An IResult representing the outcome.</returns>
    public static IResult ToCreatedResponse<T>(this Result<T> result, string uri)
    {
        return result.IsSuccess
            ? HttpResults.Created(uri, ApiResponse<T>.Ok(result.Value))
            : HttpResults.BadRequest(ApiResponse<T>.Fail(result.Error ?? "An error occurred"));
    }

    /// <summary>
    /// Converts a Result to an appropriate IResult response with a created location.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="routeName">The name of the route to generate the URI.</param>
    /// <param name="routeValues">The route values for URI generation.</param>
    /// <returns>An IResult representing the outcome.</returns>
    public static IResult ToCreatedAtRouteResponse<T>(
        this Result<T> result,
        string routeName,
        RouteValueDictionary? routeValues = null)
    {
        return result.IsSuccess
            ? HttpResults.CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(result.Value))
            : HttpResults.BadRequest(ApiResponse<T>.Fail(result.Error ?? "An error occurred"));
    }

    /// <summary>
    /// Converts a nullable value to a NotFound or Ok response.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>Ok if value exists, NotFound otherwise.</returns>
    public static IResult ToResponse<T>(this T? value) where T : class
    {
        return value is not null
            ? HttpResults.Ok(ApiResponse<T>.Ok(value))
            : HttpResults.NotFound(ApiResponse<T>.Fail("Resource not found", "NOT_FOUND"));
    }

    /// <summary>
    /// Converts Unit to a NoContent response.
    /// </summary>
    /// <param name="_">The unit value (unused).</param>
    /// <returns>A NoContent result.</returns>
    public static IResult ToNoContentResponse(this Unit _)
    {
        return HttpResults.NoContent();
    }
}
