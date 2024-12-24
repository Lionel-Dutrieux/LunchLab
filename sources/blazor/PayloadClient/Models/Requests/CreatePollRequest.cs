namespace PayloadClient.Models.Requests;

public class CreatePollRequest
{
    public string Title { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public List<string> RestaurantIds { get; set; } = new();
} 