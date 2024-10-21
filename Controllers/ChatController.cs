using System.Net.WebSockets;
using System.Text;
using chatexperiment.Models;
using chatexperiment.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace chatexperiment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatWebSocketManager _chatManager;

    public ChatController(ChatWebSocketManager chatManager)
    {
        _chatManager = chatManager;
    }

    [HttpGet("/ws")]
    public async Task Get(int userId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _chatManager.AddConnection(userId, webSocket);

            await HandleWebSocketConnection(userId, webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    private async Task HandleWebSocketConnection(int userId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while ((result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)).CloseStatus == null)
        {
            var messageContent = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var message = JsonConvert.DeserializeObject<Message>(messageContent);
            
            _chatManager.AddMessageToChat(1, message);
            
            foreach (var connection in _chatManager.Connections)
            {
                if (connection.Key != userId)
                {
                    await _chatManager.SendMessageAsync(connection.Key, messageContent);
                }
            }
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        _chatManager.RemoveConnection(userId);
    }
}