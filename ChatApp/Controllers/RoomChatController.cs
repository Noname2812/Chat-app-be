
using AutoMapper;
using ChatApp.Attributes;
using ChatApp.Data.Repository.Messages;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Models.DTOs;
using ChatApp.Models.Query;
using ChatApp.Models.ResponeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{

    [Route("api/room")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class RoomChatController : ControllerBase
    {
        private Respone _res;
        private readonly IRoomChatRepository _roomChatRepository;
        private readonly IMapper _mapper;
        private readonly IMessageRespository _messageRespository;
        public RoomChatController(IRoomChatRepository roomChatRepository, IMessageRespository messageRespository, IMapper mapper)
        {
            _res = new();
            _roomChatRepository = roomChatRepository;
            _mapper = mapper;
            _messageRespository = messageRespository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Respone>> getAllRoomChatByUser()
        {
            var claimsPrincipal = HttpContext.User;
            string? userId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
            var rooms = await _roomChatRepository.GetAllRoomChatByIdUser(userId);
            var roomRes = _mapper.Map<List<RoomChatDTO>>(rooms);
            _res.data = new { rooms = roomRes };
            return Ok(_res);
        }
        [HttpGet]
        [Cache(30)]
        [Route("{id:int}", Name = "getRoomChatById")]
        public async Task<ActionResult<Respone>> getRoomChatById([FromRoute] int id, [FromQuery] FilterQuery? query)
        {
            var room = await _roomChatRepository.GetRoomChatById(id, query?.offset ?? 0, query?.limit ?? 10);
            if (room == null)
            {
                return NotFound(_res.errors = new { message = $"Not found room with id is {id}" });
            }
            var roomRes = _mapper.Map<RoomChatDTO>(room);
            _res.data = new { room = roomRes };
            return Ok(_res);
        }
        [HttpGet]
        [Route("get-room-private/{id:int}")]
        public async Task<ActionResult<Respone>> GetPrivateRoomBetweenTwoUsers([FromRoute] int id)
        {
            try
            {
                var claimsPrincipal = HttpContext.User;
                string? userId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
                var room = await _roomChatRepository.GetRoomChatPrivateBetweenTwoUser(int.Parse(userId), id);
                _res.data = _mapper.Map<RoomChatDTO>(room);
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
