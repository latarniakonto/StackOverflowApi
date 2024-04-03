namespace StackOverflow.Api.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string? Message { get; set; }

    public bool HasMessage => !string.IsNullOrEmpty(Message);
}
