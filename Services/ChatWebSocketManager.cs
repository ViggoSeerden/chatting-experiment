using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using chatexperiment.Models;

namespace chatexperiment.Services
{
    public class ChatWebSocketManager
    {
        private readonly ConcurrentDictionary<int, WebSocket> _connections = new();
        private readonly ConcurrentDictionary<int, Chat> _chats = new();

        public IReadOnlyDictionary<int, WebSocket> Connections => _connections;
        
        public void AddConnection(int userId, WebSocket webSocket)
        {
            _connections[userId] = webSocket;
        }

        public void RemoveConnection(int userId)
        {
            if (_connections.TryRemove(userId, out var webSocket))
            {
                webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        public async Task SendMessageAsync(int userId, string message)
        {
            if (_connections.TryGetValue(userId, out var webSocket))
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public void AddMessageToChat(int chatId, Message message)
        {
            if (_chats.TryGetValue(chatId, out var chat))
            {
                chat.Messages.Add(message);
            }
            else
            {
                var newChat = new Chat
                {
                    ChatId = chatId,
                    Messages = new List<Message> { message }
                };
                _chats[chatId] = newChat;
            }
        }
    }

}