using System.Text.Json.Serialization;
namespace Backend.Models;
public class ResponseMessage
{
  [JsonPropertyName("action")]
  public string Action { get; set; }

  [JsonPropertyName("response")]
  public string Response { get; set; }
}