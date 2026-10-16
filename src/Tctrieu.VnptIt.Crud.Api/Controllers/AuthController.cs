using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Api.Common.Responses;
using Tctrieu.VnptIt.Crud.Application.Auth;
using Tctrieu.VnptIt.Crud.Application.Auth.Dtos;
using Tctrieu.VnptIt.Crud.Application.Common.Exceptions;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;

namespace Tctrieu.VnptIt.Crud.Api.Controllers
{
    /// <summary>
    /// API xác thực: đăng ký, đăng nhập, làm mới token, đăng xuất
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// Đăng ký tài khoản mới, trả về token luôn
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
            [FromBody] RegisterRequest request,
            CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(request, ct);
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse.Success(result, "Đăng ký thành công", StatusCodes.Status201Created));
        }

        /// <summary>
        /// Đăng nhập bằng email và mật khẩu
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
            [FromBody] LoginRequest request,
            CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);
            return Ok(ApiResponse.Success(result, "Đăng nhập thành công"));
        }

        /// <summary>
        /// Đổi refresh token lấy cặp token mới
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken, ct);
            return Ok(ApiResponse.Success(result, "Làm mới token thành công"));
        }

        /// <summary>
        /// Đăng xuất: thu hồi refresh token
        /// </summary>
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> Logout(
            [FromBody] RefreshTokenRequest request,
            CancellationToken ct)
        {
            await _authService.LogoutAsync(request.RefreshToken, ct);
            return Ok(ApiResponse.Success("Đăng xuất thành công"));
        }

        /// <summary>
        /// Lấy thông tin user đang đăng nhập
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> Me(CancellationToken ct)
        {
            // JwtBearer mặc định map claim "sub" sang NameIdentifier
            var user_id_claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(user_id_claim, out var user_id))
                throw new UnauthorizedException("Token không hợp lệ");

            var user = await _userService.GetByIdAsync(user_id, ct);
            return Ok(ApiResponse.Success(user));
        }
    }
}
