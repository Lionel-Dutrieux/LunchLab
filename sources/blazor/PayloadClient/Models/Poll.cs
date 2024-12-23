using System.Text.Json.Serialization;
using PayloadClient.Converters;

namespace PayloadClient.Models;

public class Poll
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("options")]
    public List<PollOption> Options { get; set; } = new();

    [JsonPropertyName("createdBy")]
    [JsonConverter(typeof(EntityRefConverter<UserRef>))]
    public UserRef CreatedBy { get; set; } = new();

    [JsonPropertyName("totalVotes")]
    public int TotalVotes { get; set; }

    [JsonPropertyName("mostVoted")]
    [JsonConverter(typeof(EntityRefConverter<RestaurantRef>))]
    public RestaurantRef? MostVoted { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class PollOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("restaurant")]
    [JsonConverter(typeof(EntityRefConverter<RestaurantRef>))]
    public RestaurantRef Restaurant { get; set; } = new();

    [JsonPropertyName("votes")]
    public List<PollVote> Votes { get; set; } = new();

    [JsonPropertyName("addedBy")]
    [JsonConverter(typeof(EntityRefConverter<UserRef>))]
    public UserRef AddedBy { get; set; } = new();
}

public class PollVote
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    [JsonConverter(typeof(EntityRefConverter<UserRef>))]
    public UserRef User { get; set; } = new();

    [JsonPropertyName("votedAt")]
    public DateTime VotedAt { get; set; }
} 

public class UserRef
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("trigram")]
    public string Trigram { get; set; } = string.Empty;

} 