using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Domain.Enums;

namespace Tctrieu.VnptIt.Crud.Api.Common.Authorization
{
    // Tên các policy phân quyền
    public static class Policies
    {
        // Chỉ user đang ở trạng thái Active mới được gọi
        public const string ACTIVE_USER = "ActiveUser";
    }

    public class ActiveUserRequirement : IAuthorizationRequirement
    {
    }

    // Kiểm tra trạng thái user trong DB (không tin claim trong token vì user có thể bị khóa sau khi đăng nhập)
    public class ActiveUserHandler : AuthorizationHandler<ActiveUserRequirement>
    {
        private readonly IUserRepository _userRepository;

        public ActiveUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ActiveUserRequirement requirement)
        {
            var user_id_claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(user_id_claim, out var user_id))
                return;

            var user = await _userRepository.GetByIdAsync(user_id);
            if (user != null && user.Status == UserStatusEnum.Active)
                context.Succeed(requirement);
        }
    }
}
