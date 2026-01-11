using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LunaArch.AspNetCore.Serialization;

/// <summary>
/// JSON serialization context for AOT-compatible serialization of API types.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ValidationProblemDetails))]
public sealed partial class LunaArchJsonContext : JsonSerializerContext;
