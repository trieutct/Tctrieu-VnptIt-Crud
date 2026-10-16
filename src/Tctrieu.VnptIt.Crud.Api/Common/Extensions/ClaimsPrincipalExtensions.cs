using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tctrieu.VnptIt.Crud.Application.Common.Exceptions;

namespace Tctrieu.VnptIt.Crud.Api.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Lấy id user đang đăng nhập từ token, không có thì ném UnauthorizedException
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // JwtBearer mặc định map claim "sub" sang NameIdentifier
            var user_id_claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(user_id_claim, out var user_id))
                throw new UnauthorizedException("Token không hợp lệ");

            return user_id;
        }
    }
}
