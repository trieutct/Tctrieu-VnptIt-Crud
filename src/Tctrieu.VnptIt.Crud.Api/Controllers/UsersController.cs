using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Api.Common.Authorization;
using Tctrieu.VnptIt.Crud.Api.Common.Responses;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;

namespace Tctrieu.VnptIt.Crud.Api.Controllers
{
    // API quản lý người dùng
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Lấy danh sách user có lọc và phân trang
        [HttpGet]
        [Authorize(Policy = Policies.ACTIVE_USER)]
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

        // Lấy chi tiết user
        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.ACTIVE_USER)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            return Ok(ApiResponse.Success(user));
        }

        // Tạo user mới
        [HttpPost]
        [Authorize(Policy = Policies.ACTIVE_USER)]
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

        // Cập nhật thông tin user
        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.ACTIVE_USER)]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update(
            int id,
            [FromBody] UpdateUserRequest request,
            CancellationToken ct)
        {
            var user = await _userService.UpdateAsync(id, request, ct);
            return Ok(ApiResponse.Success(user, "Cập nhật user thành công"));
        }

        // Mở khóa tài khoản
        [HttpPatch("{id:int}/activate")]
        [Authorize(Policy = Policies.ACTIVE_USER)]
        public async Task<ActionResult<ApiResponse<object>>> Activate(int id, CancellationToken ct)
        {
            await _userService.ActivateAsync(id, ct);
            return Ok(ApiResponse.Success("Mở khóa tài khoản thành công"));
        }

        // Khóa tài khoản
        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Policy = Policies.ACTIVE_USER)]
        public async Task<ActionResult<ApiResponse<object>>> Deactivate(int id, CancellationToken ct)
        {
            await _userService.DeactivateAsync(id, ct);
            return Ok(ApiResponse.Success("Khóa tài khoản thành công"));
        }

        // Xóa user
        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.ACTIVE_USER)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
        {
            await _userService.DeleteAsync(id, ct);
            return Ok(ApiResponse.Success("Xóa user thành công"));
        }
    }
}
