using AutoMapper;
using ChatApp.Data.Repository.Messages;
using ChatApp.Data.Repository.Users;
using ChatApp.Models.DTOs;
using ChatApp.Models.RequestModels;
using ChatApp.Models.ResponeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private Respone _res;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _res = new();
        }
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<Respone>> UpdateProfileUser([FromBody] UserRequest user)
        {
            try
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
                if (user == null)
                {
                    _res.errors = "Please provide user !";
                    return BadRequest(_res);
                }
                var userExist = await _userRepository.GetItemByQuery(x => x.Id == parsedUserId);
                if (userExist == null)
                {
                    _res.errors = "User Invalid";
                    return NotFound(_res);
                }
                userExist.Name = user.Name ?? userExist.Name;
                userExist.Email = user.Email ?? userExist.Email;
                userExist.Phone = user.Phone ?? userExist.Phone;
                userExist.Avatar = user.Avatar ?? userExist.Avatar;
                userExist.Address = user.Address ?? userExist.Address;
                var userUdpdate = await _userRepository.Update(userExist);
                _res.data = new { message = "Update successfully !", user = _mapper.Map<UserDTO>(userUdpdate) };
                return Ok(_res);
            }
            catch (Exception ex)
            {
                _res.errors = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }
        }
        [HttpPut]
        [Route("change-password")]
        public async Task<ActionResult<Respone>> ChangePassword([FromBody] UserRequest user)
        {
            try
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
                if (user == null)
                {
                    _res.errors = "Please provide user !";
                    return BadRequest(_res);
                }
                var userExist = await _userRepository.GetItemByQuery(x => x.Id == parsedUserId);
                if (userExist == null)
                {
                    _res.errors = "User Invalid";
                    return NotFound(_res);
                }
                if (userExist.Password != user.OldPassword)
                {
                    _res.errors = "OldPassword is wrong !";
                    return BadRequest(_res);
                }
                userExist.Password = user.newPassword ?? userExist.Password;
                await _userRepository.Update(userExist);
                _res.data = new { message = "Change password successfully !" };
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
