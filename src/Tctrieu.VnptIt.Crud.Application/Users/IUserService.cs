using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tctrieu.VnptIt.Crud.Application.Common.Models;
using Tctrieu.VnptIt.Crud.Application.Users.Dtos;

namespace Tctrieu.VnptIt.Crud.Application.Users
{
    // Các nghiệp vụ liên quan tới User
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<UserDto>> GetListAsync(GetUsersQuery query, CancellationToken ct = default);
        Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
        Task<UserDto> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
        Task ActivateAsync(int id, CancellationToken ct = default);
        Task DeactivateAsync(int id, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
