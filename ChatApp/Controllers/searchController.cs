using AutoMapper;
using ChatApp.Data.Repository.Users;
using ChatApp.Data.UnitOfWork;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using ChatApp.Models.Query;
using ChatApp.Models.ResponeModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/search")]
    [ApiController]
    [Authorize]
    public class searchController : ControllerBase
    {
        private readonly Respone _res;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public searchController(IUnitOfWork unitOfWork, IMapper mapper, Respone respone)
        {
            _res = respone;
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        [HttpGet]
        [Route("{value}", Name = "GetUsersByQuery")]
        public async Task<ActionResult<Respone>> GetUsersByQuery([FromRoute] string value, [FromQuery] FilterQuery? query)
        {
            try
            {
                var userId = FunctionHelper.GetUserId(HttpContext);
                var users = await _unitOfWork.UserRepository.SearchUserByQuery(value, int.Parse(userId));
                _res.data = users;
                return Ok(_res);
            }
            catch (Exception ex)
            {
                _res.data = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }
        }
    }
}
