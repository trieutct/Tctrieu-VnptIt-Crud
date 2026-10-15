using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Exceptions;
using Tctrieu.VnptIt.Crud.Application.Common.Interfaces;
using Tctrieu.VnptIt.Crud.Application.Common.Models;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Application.Users
{
    /// <summary>
    /// Xử lý nghiệp vụ User
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        public async Task<UserDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var user = await GetUserOrThrowAsync(id, ct);
            return UserDto.FromEntity(user);
        }

        /// <summary>
        /// Lấy danh sách user có lọc và phân trang
        /// </summary>
        public async Task<PagedResult<UserDto>> GetListAsync(GetUsersQuery query, CancellationToken ct = default)
        {
            // Chuẩn hóa tham số phân trang
            if (query.Page < 1) query.Page = 1;
            if (query.PageSize < 1) query.PageSize = GetUsersQuery.DEFAULT_PAGE_SIZE;
            if (query.PageSize > GetUsersQuery.MAX_PAGE_SIZE) query.PageSize = GetUsersQuery.MAX_PAGE_SIZE;

            var (items, totalCount) = await _userRepository.GetPagedAsync(query, ct);

            var dtos = items.Select(UserDto.FromEntity).ToList();
            return new PagedResult<UserDto>(dtos, totalCount, query.Page, query.PageSize);
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            // Kiểm tra trùng email
            var email = request.Email.Trim().ToLowerInvariant();
            if (await _userRepository.EmailExistsAsync(email, null, ct))
                throw new ConflictException("Email đã được sử dụng");

            // Mã hóa mật khẩu và tạo entity
            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Create(request.FullName, email, passwordHash, request.PhoneNumber);

            // Lưu xuống DB, sau bước này user.Id mới có giá trị
            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return UserDto.FromEntity(user);
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        public async Task<UserDto> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await GetUserOrThrowAsync(id, ct);

            user.UpdateProfile(request.FullName, request.PhoneNumber);
            await _unitOfWork.SaveChangesAsync(ct);

            return UserDto.FromEntity(user);
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        public async Task ActivateAsync(int id, CancellationToken ct = default)
        {
            var user = await GetUserOrThrowAsync(id, ct);

            user.Activate();
            await _unitOfWork.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        public async Task DeactivateAsync(int id, CancellationToken ct = default)
        {
            var user = await GetUserOrThrowAsync(id, ct);

            user.Deactivate();
            await _unitOfWork.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await GetUserOrThrowAsync(id, ct);

            _userRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Lấy user theo id, không có thì ném NotFoundException
        /// </summary>
        private async Task<User> GetUserOrThrowAsync(int id, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                throw new NotFoundException($"Không tìm thấy user có id = {id}");

            return user;
        }
    }
}
