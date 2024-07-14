
using AutoMapper;
using ChatApp.Data.UnitOfWork;
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
        private readonly Respone _res;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RoomChatController(IUnitOfWork unitOfWork, IMapper mapper, Respone respone)
        {
            _res = respone;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[Cache(3600)]
        public async Task<ActionResult<Respone>> getAllRoomChatByUser()
        {
            var claimsPrincipal = HttpContext.User;
            string? userId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
            if(Guid.TryParse(userId, out Guid guid))
            {
                var rooms = await _unitOfWork.RoomChatRepository.GetAllRoomChatByIdUser(guid);
                var roomRes = _mapper.Map<List<RoomChatDTO>>(rooms);
                _res.data = new { rooms = roomRes };
                return Ok(_res);
            }
            _res.errors = "Cannot get User ID !";
            return BadRequest(_res);

        }
        [HttpGet]
        [Route("{id}", Name = "getRoomChatById")]
        public async Task<ActionResult<Respone>> getRoomChatById([FromRoute] Guid id, [FromQuery] FilterQuery? query)
        {
            var room = await _unitOfWork.RoomChatRepository.GetRoomChatById(id, query?.offset ?? 0, query?.limit ?? 10);
            if (room == null)
            {
                return NotFound(_res.errors = new { message = $"Not found room with id is {id}" });
            }
            var roomRes = _mapper.Map<RoomChatDTO>(room);
            _res.data = new { room = roomRes };
            return Ok(_res);
        }
        [HttpGet]
        [Route("get-room-private/{id}")]
        public async Task<ActionResult<Respone>> GetPrivateRoomBetweenTwoUsers([FromRoute] Guid id)
        {
            try
            {
                var claimsPrincipal = HttpContext.User;
                string? userId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
                var room = await _unitOfWork.RoomChatRepository.GetRoomChatPrivateBetweenTwoUser(Guid.Parse(userId), id);
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
