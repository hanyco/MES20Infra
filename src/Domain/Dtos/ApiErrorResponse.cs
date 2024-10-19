namespace Domain.Dtos;

public sealed class ApiErrorResponse(string message, int errorCode)
{
    public int ErrorCode { get; } = errorCode;
    public string Message { get; } = message;
}