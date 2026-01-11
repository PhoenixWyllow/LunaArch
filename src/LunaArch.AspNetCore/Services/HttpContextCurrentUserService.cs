using LunaArch.Abstractions.Services;
using Microsoft.AspNetCore.Http;

namespace LunaArch.AspNetCore.Services;

/// <summary>
/// Implementation of ICurrentUserService that retrieves user information from HttpContext.
/// </summary>
public sealed class HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    /// <inheritdoc />
    public string? UserId =>
        httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ??
        httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    /// <inheritdoc />
    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
