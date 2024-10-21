using Microsoft.AspNetCore.Mvc;

namespace chatexperiment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private Chat[] chats;

        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetChats")]
        public IEnumerable<Chat> Get()
        {
            return null;
        }
    }
}