using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Backend.Models;
using Backend.Services;

namespace Backend.Handlers
{
  public class WebSocketHandler
  {
    private readonly CipherService _cipherService;

    public WebSocketHandler(CipherService cipherService)
    {
      _cipherService = cipherService;
    }

    public async Task HandleAsync(HttpContext context)
    {
      WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
      var buffer = new byte[1024 * 16];
      WebSocketReceiveResult result;
      var receivedData = new List<byte>();

      do
      {
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        receivedData.AddRange(buffer.Take(result.Count));
      }
      while (!result.EndOfMessage);

      string receivedMessage = Encoding.UTF8.GetString(receivedData.ToArray());
      Console.WriteLine($"Received message: {receivedMessage}");
      var request = JsonSerializer.Deserialize<RequestMessage>(receivedMessage);

      string responseText = request.Action switch
      {
        "generateKey" => _cipherService.GenerateKey(),
        "encrypt" => _cipherService.Encrypt(request.Text, request.Key),
        "decrypt" => _cipherService.Decrypt(request.Text, request.Key),
        _ => throw new InvalidOperationException("Unknown action")
      };

      var responseMessage = new ResponseMessage
      {
        Action = request.Action,
        Response = responseText
      };

      var responseJson = JsonSerializer.Serialize(responseMessage);
      var responseBuffer = Encoding.UTF8.GetBytes(responseJson);
      await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
  }
}
