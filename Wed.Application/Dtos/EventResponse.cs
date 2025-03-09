namespace Wed.Application.Dtos;

public class EventResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string OwnerId { get; init; } = string.Empty;
}