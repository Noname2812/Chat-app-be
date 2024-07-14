using AutoMapper;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using ChatApp.Models.Request;
using ChatApp.Models.ResponeModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatApp.Services.ChatServices;
using ChatApp.Data.Modals;
using ChatApp.Data.UnitOfWork;
namespace ChatApp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubService _chatService;
        private readonly IMapper _mapper;
        private readonly Respone _res;
        public ChatController(IUnitOfWork unitOfWork, IMapper mapper, IHubService chatService, Respone res)
        {
            _unitOfWork = unitOfWork;
            _chatService = chatService;
            _mapper = mapper;
            _res = res;
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
            try
            {
                var userId = Guid.Parse(userIdClaim.Value);
                if (req.RoomId is null)
                {
                    if (req.isPrivate && req.to is not null)
                    {
                        // create room
                        List<Guid> arrUserId = new List<Guid>() { req.to ?? Guid.NewGuid(), userId };
                        var name = await FunctionHelper.GetNameRoomByTwoIds(req.to, userId, _unitOfWork.UserRepository);
                        RoomChat newRoomChat = await _unitOfWork.RoomChatRepository.CreateRoomChat(
                            new RoomChat { Id = Guid.NewGuid(), CreatAt = DateTime.Now, ModifiedDate = DateTime.Now, IsPrivate = true, Name = name ?? "Default" },
                            arrUserId);
                        List<Message> msgs = [new Message { Id = Guid.NewGuid(), Content = req.Message, CreateAt = DateTime.Now, RoomChatId = newRoomChat.Id, UserId = userId }];
                        if (req.Images is not null)
                        {
                            foreach (var img in req.Images)
                            {
                                msgs.Add(new Message { Id = Guid.NewGuid(), CreateAt = DateTime.Now, RoomChatId = newRoomChat.Id, UserId = userId, ImageUrl = img });
                            }
                        }
                        await _unitOfWork.MessageRespository.AddListData(msgs);
                        await _chatService.SendMessage(new MessageRequest { from = userId, RoomId = newRoomChat.Id });
                        _res.data = new { messages = _mapper.Map<List<MessageDTO>>(msgs), roomId = newRoomChat.Id };
                        return Ok(_res);
                    }
                    _res.errors = "Please provide room id !";
                    return BadRequest(_res);
                }
                var room = await _unitOfWork.RoomChatRepository.GetItemByQuery(x => x.Id == req.RoomId, true);
                if (room == null)
                {
                    _res.errors = new
                    {
                        message = $"Not found room with ID is {req.RoomId}"
                    };
                    return NotFound(_res);
                }
                // insert message into database
                List<Message> messages = [new Message { Content = req.Message, CreateAt = DateTime.Now, RoomChatId = room.Id, UserId = userId }];
                if (req.Images is not null)
                {
                    foreach (var img in req.Images)
                    {
                        messages.Add(new Message { CreateAt = DateTime.Now, RoomChatId = room.Id, UserId = userId, ImageUrl = img });
                    }
                }
                await _unitOfWork.MessageRespository.AddListData(messages);
                await _chatService.SendMessage(new MessageRequest { from = userId, RoomId = room.Id });
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

