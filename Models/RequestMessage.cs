using System.Text.Json.Serialization;
namespace Backend.Models;

public class RequestMessage
{
  [JsonPropertyName("action")]
  public string Action { get; set; }

  [JsonPropertyName("text")]
  public string Text { get; set; }

  [JsonPropertyName("key")]
  public string Key { get; set; }
}

