using System;

namespace Ergonaut.Api.Errors;

public sealed record ApiError(string Code, string Message, Guid? ProjectId = null)
{
    public static ApiError ProjectNotFound(Guid projectId) =>
        new("ProjectNotFound", $"Project {projectId} was not found.", projectId);

    public static ApiError UnknownError(string message) =>
        new("UnknownError", string.IsNullOrWhiteSpace(message) ? "An unexpected error occurred." : message);
}
