using System;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Auth.Dtos;
using Tctrieu.VnptIt.Crud.Application.Common.Exceptions;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;
using Tctrieu.VnptIt.Crud.Application.Users;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;
using Tctrieu.VnptIt.Crud.Domain.Entities;
using Tctrieu.VnptIt.Crud.Domain.Enums;

namespace Tctrieu.VnptIt.Crud.Application.Auth
{
    /// <summary>
    /// Xử lý đăng ký, đăng nhập, làm mới token, đăng xuất
    /// </summary>
    public class AuthService : IAuthService
    {
        private const string INVALID_CREDENTIALS_MESSAGE = "Email hoặc mật khẩu không đúng";
        private const string INVALID_REFRESH_TOKEN_MESSAGE = "Refresh token không hợp lệ hoặc đã hết hạn";
        private const string USER_INACTIVE_MESSAGE = "Tài khoản đã bị khóa";

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Đăng ký tài khoản mới và đăng nhập luôn
        /// </summary>
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            // Kiểm tra trùng email
            var email = request.Email.Trim().ToLowerInvariant();
            if (await _userRepository.EmailExistsAsync(email, null, ct))
                throw new ConflictException("Email đã được sử dụng");

            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Create(request.FullName, email, passwordHash, request.PhoneNumber);

            // Lưu user trước để có user.Id gắn vào refresh token
            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await IssueTokensAsync(user, ct);
        }

        /// <summary>
        /// Đăng nhập bằng email và mật khẩu
        /// </summary>
        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, ct);

            // Không phân biệt sai email hay sai mật khẩu để tránh dò tài khoản
            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException(INVALID_CREDENTIALS_MESSAGE);

            if (user.Status != UserStatusEnum.Active)
                throw new ForbiddenException(USER_INACTIVE_MESSAGE);

            return await IssueTokensAsync(user, ct);
        }

        /// <summary>
        /// Đổi refresh token cũ lấy cặp token mới (token cũ bị thu hồi)
        /// </summary>
        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var tokenHash = _tokenService.HashRefreshToken(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

            if (storedToken == null)
                throw new UnauthorizedException(INVALID_REFRESH_TOKEN_MESSAGE);

            // Token đã thu hồi mà vẫn được dùng lại => có thể bị lộ, thu hồi toàn bộ phiên của user
            if (storedToken.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllActiveAsync(storedToken.UserId, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                throw new UnauthorizedException(INVALID_REFRESH_TOKEN_MESSAGE);
            }

            if (storedToken.IsExpired)
                throw new UnauthorizedException(INVALID_REFRESH_TOKEN_MESSAGE);

            var user = await _userRepository.GetByIdAsync(storedToken.UserId, ct);
            if (user == null)
                throw new UnauthorizedException(INVALID_REFRESH_TOKEN_MESSAGE);

            if (user.Status != UserStatusEnum.Active)
                throw new ForbiddenException(USER_INACTIVE_MESSAGE);

            // Xoay vòng: cấp token mới, thu hồi token cũ
            var (newRefreshToken, newRefreshTokenExpiresAt) = _tokenService.CreateRefreshToken();
            var newTokenHash = _tokenService.HashRefreshToken(newRefreshToken);

            storedToken.Revoke(newTokenHash);
            await _refreshTokenRepository.AddAsync(
                RefreshToken.Create(user.Id, newTokenHash, newRefreshTokenExpiresAt), ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return BuildResponse(user, newRefreshToken, newRefreshTokenExpiresAt);
        }

        /// <summary>
        /// Đăng xuất: thu hồi refresh token hiện tại
        /// </summary>
        public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            var tokenHash = _tokenService.HashRefreshToken(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

            // Token không tồn tại hoặc đã thu hồi thì coi như đã đăng xuất
            if (storedToken == null || storedToken.IsRevoked) return;

            storedToken.Revoke();
            await _unitOfWork.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Cấp cặp access token + refresh token mới và lưu refresh token
        /// </summary>
        private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
        {
            var (refreshToken, refreshTokenExpiresAt) = _tokenService.CreateRefreshToken();
            var tokenHash = _tokenService.HashRefreshToken(refreshToken);

            await _refreshTokenRepository.AddAsync(
                RefreshToken.Create(user.Id, tokenHash, refreshTokenExpiresAt), ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return BuildResponse(user, refreshToken, refreshTokenExpiresAt);
        }

        private AuthResponse BuildResponse(User user, string refreshToken, DateTime refreshTokenExpiresAt)
        {
            var (accessToken, accessTokenExpiresAt) = _tokenService.CreateAccessToken(user);

            return new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                User = UserDto.FromEntity(user),
            };
        }
    }
}
