using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.User;
using Kpi.Service.DTOs.User;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.User;
using Kpi.Service.StringExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;

namespace Kpi.Service.Service.User
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<Domain.Entities.User.User> _userRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserService(IGenericRepository<Domain.Entities.User.User> userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<UserModel> CreateAsync(UserForCreateDTO @dto)
        {
            var user = new Domain.Entities.User.User()
            {
                TeamId = dto.TeamId,
                FullName = dto.FullName,
                Role = dto.Role,
                UserName = dto.UserName,
                Password = dto.Password.Encrypt(),
                RoomId = dto.RoomId
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(user);
        }

        public async ValueTask<UserModel> UpdateAsync(UserForUpdateDTO @dto)
        {
            var existUser = await _userRepository.GetAsync(x => x.Id == dto.Id && x.IsDeleted == 0);

            if (existUser == null) throw new KpiException(404, "user_not_found");

            existUser.Role = dto.Role;
            existUser.FullName = dto.FullName;
            existUser.Password = dto.Password ?? existUser.Password;
            existUser.TeamId = dto.TeamId;
            existUser.UpdatedAt = DateTime.UtcNow;
            existUser.RoomId = dto.RoomId;

            _userRepository.UpdateAsync(existUser);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(existUser);
        }

        public async ValueTask<bool> DeleteAsync([Required] int id)
        {
            var existUser = await _userRepository.GetAsync(x => x.Id == id && x.IsDeleted == 0);

            if (existUser == null) throw new KpiException(404, "user_not_found");

            existUser.UpdatedAt = DateTime.UtcNow;
            existUser.IsDeleted = 1;

            _userRepository.UpdateAsync(existUser);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async ValueTask<PagedResult<UserModel>> GetAllAsync(UserForFilterDTO @dto)
        {
            var query = _userRepository.GetAll(x => x.IsDeleted == dto.IsDeleted)
                .Include(x => x.AssignedGoals)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.Text))
            {
                query = query.Where(x => x.UserName.Contains(dto.Text) || x.FullName.Contains(dto.Text) || (x.Team != null && x.Team.Name.Contains(dto.Text)));
            }

            int totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModel>.Create(
                    Enumerable.Empty<UserModel>(),
                    0,
                    dto.PageSize,
                    0,
                    dto.PageIndex,
                    0
                );
            }

            if (dto.PageIndex == 0)
            {
                dto.PageIndex = 1;
            }

            if (dto.PageSize == 0)
            {
                dto.PageSize = totalCount;
            }

            int itemsPerPage = dto.PageSize;
            int totalPages = (totalCount / itemsPerPage) + (totalCount % itemsPerPage == 0 ? 0 : 1);

            if (dto.PageIndex > totalPages)
            {
                dto.PageIndex = totalPages;
            }

            query = query.ToPagedList(dto);

            var list = await query.ToListAsync();

            List<UserModel> models = list.Select(
                f => new UserModel().MapFromEntity(f))
                .ToList();

            var pagedResult = PagedResult<UserModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<UserModel> GetByIdAsync([Required] int id)
        {
            var existUser = await _userRepository.GetAll(x => x.Id == id && x.IsDeleted == 0)
                .Include(x => x.AssignedGoals)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            return new UserModel().MapFromEntity(existUser);
        }


        public async ValueTask<UserModel> GetByTokenAsync()
        {
            if (!int.TryParse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                throw new InvalidCredentialException();
            }

            var existUser = await _userRepository.GetAll(x => x.Id == userId && x.IsDeleted == 0)
                .Include(x => x.AssignedGoals)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            return new UserModel().MapFromEntity(existUser);
        }
    }
}
