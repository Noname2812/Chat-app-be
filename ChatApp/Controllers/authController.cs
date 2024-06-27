using AutoMapper;
using ChatApp.Data;
using ChatApp.Data.Repository.Users;
using ChatApp.Interfaces;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using ChatApp.Models.RequestModels;
using ChatApp.Models.ResponeModels;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        private Respone _res;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _dbContext;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public authController(IConfiguration configuration, IUserRepository dbContext, IMapper mapper, ICacheService cacheService)
        {
            _res = new();
            _configuration = configuration;
            _dbContext = dbContext;
            _mapper = mapper;
            _cacheService = cacheService;
        }
        [HttpPost]
        [Route("register", Name = "registerAsync")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Respone>> registerAsync([FromBody] RegisterUserModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _res.errors = "Please enter your infomation !";
                    return BadRequest(_res);
                }
                var isExists = await _dbContext.GetItemByQuery(x => x.UserName == user.UserName);
                if (isExists != null)
                {
                    _res.errors = new
                    {
                        message = $"{user.UserName} is exists !"
                    };
                    return StatusCode(StatusCodes.Status406NotAcceptable, _res);
                }
                User newUser = await _dbContext.Create(new User { Address = user.Address, Email = user.Email, Password = user.Password, Phone = user.Phone, UserName = user.UserName, Name = user.Name, createAt = DateTime.Now, modifiedDate = DateTime.Now, userTypeId = 1 });
                _res.data = _mapper.Map<UserDTO>(newUser);
                return Ok(_res);
            }
            catch (Exception ex)
            {
                _res.errors = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }
        }
        [HttpPost]
        [Route("login", Name = "loginLocal")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Respone>> loginLocal([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                _res.errors = "Please provide username and password !";
                return BadRequest(_res);
            }
            try
            {
                User? userExists = await _dbContext.GetItemByQuery(x => x.UserName == loginModel.Username && x.Password == loginModel.Password);
                if (userExists == null)
                {
                    _res.errors = new
                    {
                        message = "Username or Password is inexactly !"
                    };
                    return NotFound(_res);
                }
                var tokenGenarated = FunctionHelper.GenarateToken(_configuration, userExists.Id);
                var refreshToken = FunctionHelper.GenerateRefreshToken();
                userExists.RefreshToken = refreshToken;
                userExists.RefreshTokenExpired = DateTime.Now.AddDays(10);
                var userRes = _mapper.Map<UserDTO>(userExists);
                await _dbContext.Update(userExists);
                _res.data = new { message = "Login successfully !", user = userRes, token = tokenGenarated, refreshToken };
                return Ok(_res);
            }
            catch (Exception ex)
            {
                _res.errors = new
                {
                    message = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }
        }
        [HttpPost]
        [Route("login-with-google", Name = "loginWithGoogle")]
        public async Task<ActionResult<Respone>> loginWithGoogle([FromBody] LoginWithGoogle body)
        {
            var isExists = await _dbContext.GetItemByQuery(x => x.Email == body.Email);
            var refreshToken = FunctionHelper.GenerateRefreshToken();
            if (isExists == null)
            {
                User newUser = new User { Email = body.Email, createAt = DateTime.Now, modifiedDate = DateTime.Now, Name = body.Name, Avatar = body.Image, UserName = body.Email, Password = body.Email, Address = "Google", userTypeId = 4, Phone = "", RefreshToken = refreshToken, RefreshTokenExpired = DateTime.Now.AddDays(10) };
                var user = await _dbContext.Create(newUser);
                var token = FunctionHelper.GenarateToken(_configuration, user.Id);
                _res.data = new { message = "Login successfully !", user, token, refreshToken };
                return Ok(_res);
            }
            var tokenGenarated = FunctionHelper.GenarateToken(_configuration, isExists.Id);
            isExists.RefreshToken = refreshToken;
            isExists.RefreshTokenExpired = DateTime.Now.AddDays(10);
            await _dbContext.Update(isExists);
            _res.data = new { message = "Login successfully !", user = isExists, token = tokenGenarated, refreshToken };
            return Ok(_res);
        }

        [HttpPost]
        [Route("refresh-token", Name = "RefreshToken")]
        public async Task<ActionResult<Respone>> RefreshToken([FromBody] Token token)
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _res.errors = "Authorization header is missing or invalid!";
                    return Unauthorized(_res);
                }
                var jwtToken = authHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecret"));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                };
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out validatedToken);
                var userIdClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "userId");
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

                User? user = await _dbContext.GetItemByQuery(x => x.Id == parsedUserId);
                if (user == null)
                {
                    _res.errors = "User not found!";
                    return NotFound(_res);
                }

                if (user.RefreshToken != token.refreshToken)
                {
                    _res.errors = "Invalid refresh token!";
                    return Unauthorized(_res);
                }

                if (user.RefreshTokenExpired < DateTime.Now)
                {
                    _res.errors = "Refresh token has expired!";
                    return Unauthorized(_res);
                }
                var newAccessToken = FunctionHelper.GenarateToken(_configuration, user.Id);
                var newRefreshToken = FunctionHelper.GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpired = DateTime.Now.AddDays(10);
                await _dbContext.Update(user);
                _res.data = new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
                return Ok(_res);
            }
            catch (Exception ex)
            {
                _res.errors = new
                {
                    message = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }

        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
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
                // add token into blackList cache
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _cacheService.SetData($"black-list-token:{token}", token, TimeSpan.FromSeconds(600));
                var user = await _dbContext.GetItemByQuery(x => x.Id == int.Parse(userId), true);
                if (user == null)
                {
                    _res.errors = "";
                    return BadRequest(_res);
                }
                user.RefreshTokenExpired = DateTime.Now;
                await _dbContext.Update(user);
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _res.errors = new
                {
                    message = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _res);
            }
        }
    }
}
