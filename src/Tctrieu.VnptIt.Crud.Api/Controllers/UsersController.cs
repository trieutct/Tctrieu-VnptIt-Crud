using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Api.Common.Responses;
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
        public async Task<ActionResult<ApiResponse<CommonListResponse<UserDto>>>> GetList(
            [FromQuery] GetUsersQuery query,
            CancellationToken ct)
        {
            var result = await _userService.GetListAsync(query, ct);

            // Thông tin phân trang trải phẳng vào data cùng cấp với items, total_items
            var pagination = new Dictionary<string, object>
            {
                ["page"] = result.Page,
                ["page_size"] = result.PageSize,
                ["total_pages"] = result.TotalPages,
            };

            return Ok(ApiResponse.SuccessList(result.Items, result.TotalCount, pagination));
        }

        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            return Ok(ApiResponse.Success(user));
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create(
            [FromBody] CreateUserRequest request,
            CancellationToken ct)
        {
            var user = await _userService.CreateAsync(request, ct);
            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                ApiResponse.Success(user, "Tạo user thành công", StatusCodes.Status201Created));
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update(
            int id,
            [FromBody] UpdateUserRequest request,
            CancellationToken ct)
        {
            var user = await _userService.UpdateAsync(id, request, ct);
            return Ok(ApiResponse.Success(user, "Cập nhật user thành công"));
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        [HttpPatch("{id:int}/activate")]
        public async Task<ActionResult<ApiResponse<object>>> Activate(int id, CancellationToken ct)
        {
            await _userService.ActivateAsync(id, ct);
            return Ok(ApiResponse.Success("Mở khóa tài khoản thành công"));
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        public async Task<ActionResult<ApiResponse<object>>> Deactivate(int id, CancellationToken ct)
        {
            await _userService.DeactivateAsync(id, ct);
            return Ok(ApiResponse.Success("Khóa tài khoản thành công"));
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
        {
            await _userService.DeleteAsync(id, ct);
            return Ok(ApiResponse.Success("Xóa user thành công"));
        }
    }
}
