using AutoMapper;
using ChatApp.Data;
using ChatApp.Data.Repository.Messages;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Data.Repository.Users;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using ChatApp.Models.Request;
using ChatApp.Models.ResponeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMessageRespository _messageRespository;
        private readonly IRoomChatRepository _roomChatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private Respone _res;
        public ChatController(IMessageRespository messageRespository, IRoomChatRepository roomChatRepository, IMapper mapper, IUserRepository userRepository)
        {
            _messageRespository = messageRespository;
            _roomChatRepository = roomChatRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _res = new();
        }
        [HttpPost]
        public async Task<ActionResult<Respone>> sendMessage([FromBody] MessageRequest req)
        {
            var claimsPrincipal = HttpContext.User;
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "userId");
            if (userIdClaim == null)
            {
                _res.errors = "User ID claim is missing!";
                return Unauthorized(_res);
            }
            string userId = userIdClaim.Value;
            if (!int.TryParse(userId, out int parsedUserId))
            {
                _res.errors = "Invalid user ID!";
                return BadRequest(_res);
            }
            try
            {
                if (req.RoomId < 0)
                {
                    if (req.isPrivate && req.to is not null)
                    {
                        // create room
                        List<int> arrUserId = new List<int>() { req.to ?? -1, Convert.ToInt32(userId) };
                        var name = await FunctionHelper.GetNameRoomByTwoIds(req.to ?? -1, Convert.ToInt32(userId), _userRepository);
                        RoomChat? newRoomChat = await _roomChatRepository.CreateRoomChat(
                            new RoomChat { CreatAt = DateTime.Now, ModifiedDate = DateTime.Now, IsPrivate = true, Name = name ?? "Default" }, arrUserId);
                        List<Message> msgs = [new Message { Content = req.Message, CreateAt = DateTime.Now, RoomChatId = newRoomChat.Id, UserId = Convert.ToInt32(userId) }];
                        if (req.Images is not null)
                        {
                            foreach (var img in req.Images)
                            {
                                msgs.Add(new Message { CreateAt = DateTime.Now, RoomChatId = newRoomChat.Id, UserId = Convert.ToInt32(userId), ImageUrl = img });
                            }
                        }
                        await _messageRespository.AddListData(msgs);
                        _res.data = new { messages = _mapper.Map<List<MessageDTO>>(msgs), roomId = newRoomChat.Id, to = req.to };
                        return Ok(_res);
                    }
                    _res.errors = "Please provide room id !";
                    return BadRequest(_res);
                }
                var room = await _roomChatRepository.GetItemByQuery(x => x.Id == req.RoomId, true);
                if (room == null)
                {
                    _res.errors = new
                    {
                        message = $"Not found room with ID is {req.RoomId}"
                    };
                    return NotFound(_res);
                }
                // insert message into database
                List<Message> messages = [new Message { Content = req.Message, CreateAt = DateTime.Now, RoomChatId = room.Id, UserId = Convert.ToInt32(userId) }];
                if (req.Images is not null)
                {
                    foreach (var img in req.Images)
                    {
                        messages.Add(new Message { CreateAt = DateTime.Now, RoomChatId = room.Id, UserId = Convert.ToInt32(userId), ImageUrl = img });
                    }
                }
                await _messageRespository.AddListData(messages);
                var messagesRes = _mapper.Map<List<MessageDTO>>(messages);
                _res.data = new { messages = messagesRes, roomId = room.Id };
                return Ok(_res);
            }
            catch (Exception ex)
            {
                _res.errors = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }
        }
    }
}
