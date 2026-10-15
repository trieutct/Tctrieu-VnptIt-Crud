using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Models;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;

namespace Tctrieu.VnptIt.Crud.Api.Controllers
{/// <summary>
 /// API quản lý người dùng
 /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách user có lọc và phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> GetList(
            [FromQuery] GetUsersQuery query,
            CancellationToken ct)
        {
            var result = await _userService.GetListAsync(query, ct);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            return Ok(user);
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserDto>> Create(
            [FromBody] CreateUserRequest request,
            CancellationToken ct)
        {
            var user = await _userService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> Update(
            int id,
            [FromBody] UpdateUserRequest request,
            CancellationToken ct)
        {
            var user = await _userService.UpdateAsync(id, request, ct);
            return Ok(user);
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            await _userService.ActivateAsync(id, ct);
            return NoContent();
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
        {
            await _userService.DeactivateAsync(id, ct);
            return NoContent();
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _userService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
