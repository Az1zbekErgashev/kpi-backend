using Kpi.Domain.Enum;
using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.User;
using Kpi.Service.DTOs.User;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.User;
using Kpi.Service.StringExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Kpi.Service.Service.User
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<Domain.Entities.User.User> _userRepository;
        private readonly IGenericRepository<Domain.Entities.User.Position> _positionRepository;
        private readonly IGenericRepository<Domain.Entities.Team.Team> _teamRepository;
        private readonly IGenericRepository<Domain.Entities.Room.Room> _roomRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.Goal> _goalRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserService(IGenericRepository<Domain.Entities.User.User> userRepository, IHttpContextAccessor httpContextAccessor, IGenericRepository<Domain.Entities.User.Position> positionRepository, IGenericRepository<Domain.Entities.Team.Team> teamRepository, IGenericRepository<Domain.Entities.Goal.Goal> goalRepository, IGenericRepository<Domain.Entities.Room.Room> roomRepository)
        {
            _userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
            _positionRepository = positionRepository;
            _teamRepository = teamRepository;
            _goalRepository = goalRepository;
            _roomRepository = roomRepository;
        }
        public async ValueTask<UserModel> CreateAsync(UserForCreateDTO @dto)
        {
            var existUser = await _userRepository.GetAsync(x => x.UserName == dto.UserName);

            if (existUser != null) throw new KpiException(400, "this_user_already_exist");

            if (dto.Role == Role.TeamLeader)
            {
                var isExistTeamLeader = await _teamRepository.GetAll(
                    x => x.Id == dto.TeamId &&
                         x.Users.Any(u => u.Role == Role.TeamLeader)
                ).FirstOrDefaultAsync();

                if (isExistTeamLeader != null)
                {
                    throw new KpiException(400, "team_already_has_team_leader");
                }
            }

            var user = new Domain.Entities.User.User()
            {
                TeamId = dto.TeamId,
                FullName = dto.FullName,
                Role = dto.Role,
                UserName = dto.UserName,
                Password = dto.Password.Encrypt(),
                RoomId = dto.RoomId,
                PositionId = dto.PositionId,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(user);
        }

        public async ValueTask<UserModel> UpdateAsync(UserForUpdateDTO @dto)
        {
            var existUser = await _userRepository.GetAll(x => x.Id == dto.Id).Include(x => x.CreatedGoals).ThenInclude(x => x.MonthlyPerformance).FirstOrDefaultAsync();

            var existuserName = await _userRepository.GetAsync(x => x.UserName == dto.UserName && x.Id != dto.Id);

            if (existUser == null) throw new KpiException(404, "user_not_found");

            if (existuserName != null) throw new KpiException(400, "this_user_already_exist");

            if (dto.Role == Role.TeamLeader)
            {
                var isExistTeamLeader = await _teamRepository.GetAll(
                    x => x.Id == dto.TeamId &&
                         x.Users.Any(u => u.Role == Role.TeamLeader)
                ).FirstOrDefaultAsync();

                if (isExistTeamLeader != null)
                {
                    throw new KpiException(400, "team_already_has_team_leader");
                }
            }

            existUser.Role = dto.Role;
            existUser.FullName = dto.FullName;
            existUser.UserName = dto.UserName;
            existUser.UpdatedAt = DateTime.UtcNow;
            existUser.RoomId = dto.RoomId;
            existUser.PositionId = dto.PositionId;

            if(existUser.TeamId != dto.TeamId)
            {
                foreach (var item in existUser.CreatedGoals)
                {
                    await _goalRepository.DeleteAsync(item.Id);
                }

                await _goalRepository.SaveChangesAsync();
                existUser.TeamId = dto.TeamId;
            }


            _userRepository.UpdateAsync(existUser);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(existUser);
        }

        public async ValueTask<bool> DeleteAsync([Required] int id)
        {
            var existUser = await _userRepository.DeleteAsync(id);

            if (!existUser) throw new KpiException(404, "user_not_found");

            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async ValueTask<PagedResult<UserModel>> GetAllAsync(UserForFilterDTO @dto)
        {
            var query = _userRepository.GetAll(x => x.Id != 1)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.Text))
            {
                query = query.Where(x => x.UserName.Contains(dto.Text) || x.FullName.Contains(dto.Text) || (x.Team != null && x.Team.Name.toLower().Contains(dto.Text.toLower())));
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
            var existUser = await _userRepository.GetAll(x => x.Id == id)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
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

            var existUser = await _userRepository.GetAll(x => x.Id == userId)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            return new UserModel().MapFromEntity(existUser);
        }

        public async ValueTask<PagedResult<UserModelForCEO>> GetUsersForCEO(UserForFilterCEOSideDTO dto)
        {
            var query = _userRepository.GetAll(x => x.Id != 1 && x.Role == Domain.Enum.Role.TeamLeader && x.TeamId != null && x.RoomId != null)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.UserName))
            {
                query = query.Where(x => x.UserName.Contains(dto.UserName));
            }

            if (dto.RoomId != null)
            {
                query = query.Where(x => x.RoomId == dto.RoomId);
            }

            if (dto.TeamId != null)
            {
                query = query.Where(x => x.TeamId == dto.TeamId);
            }

            int totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModelForCEO>.Create(
                    Enumerable.Empty<UserModelForCEO>(),
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

            string filterYear = dto?.Year?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<UserModelForCEO> models = list.Select(
                f => new UserModelForCEO().MapFromEntity(f, filterYear))
                .ToList();

            var pagedResult = PagedResult<UserModelForCEO>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<List<PositionModel>> GetPositionAsync()
        {
            var position = await _positionRepository.GetAll().ToListAsync();
            return position.Select(x => new PositionModel().MapFromEntity(x)).ToList();
        }

        public async ValueTask<PagedResult<UserModelForCEO>> GetUserListWithGoal(UserForFilterCEOSideDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
            ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new KpiException(400, "please_check_your_team");
            }

            var users = _userRepository.GetAll()
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (role == Role.TeamLeader)
            {
                users = users.Where(x => x.TeamId == teamId && x.Id != userId);
            }
            else
            {
                users = users.Where(x => teamId == x.TeamId && x.Id == userId);
            }

            int totalCount = await users.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModelForCEO>.Create(
                    Enumerable.Empty<UserModelForCEO>(),
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

            users = users.ToPagedList(dto);

            var list = await users.ToListAsync();

            string filterYear = dto?.Year?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<UserModelForCEO> models = list.Select(
                f => new UserModelForCEO().MapFromEntity(f, filterYear))
                .ToList();

            var pagedResult = PagedResult<UserModelForCEO>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<PagedResult<UserModelForCEO>> GetTeamLeader(UserForFilterCEOSideDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
           ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new KpiException(400, "please_check_your_team");
            }

            if (role != Role.TeamLeader) throw new KpiException(400, "inccorect_role");

            var users = _userRepository.GetAll(x => x.Id == userId && x.TeamId == teamId)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();


            int totalCount = await users.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModelForCEO>.Create(
                    Enumerable.Empty<UserModelForCEO>(),
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

            users = users.ToPagedList(dto);

            var list = await users.ToListAsync();

            string filterYear = dto?.Year?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<UserModelForCEO> models = list.Select(
                f => new UserModelForCEO().MapFromEntity(f, filterYear))
                .ToList();

            var pagedResult = PagedResult<UserModelForCEO>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<UserModel> UpdateAsync(UserForUpdateByTokenDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
             ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var existUser = await _userRepository.GetAsync(x => x.Id == userId);

            if (existUser == null) throw new KpiException(404, "user_not_found");

            existUser.PositionId = dto.PositionId;
            existUser.FullName = dto.FullName;
            existUser.UpdatedAt = DateTime.UtcNow;

            if (dto.UpdatePassword)
            {
                if (existUser.Password.Equals(dto.CurrentPassword.Encrypt()))
                {
                    if (dto.NewPassword != dto.ConfirmPassword) throw new KpiException(404, "new_password_dont_match");
                    else existUser.Password = dto.NewPassword.Encrypt();
                }
                else throw new KpiException(404, "old_password_not_correct");
            }

            var model = _userRepository.UpdateAsync(existUser);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(model);
        }

        public async ValueTask<bool> UploadExcelFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new KpiException(400, "please_upload_file");

            using var stream = file.OpenReadStream();
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);
            int rowCount = sheet.LastRowNum;

            var dataFormatter = new DataFormatter();
            var columnIndexes = new Dictionary<string, int>();
            var columnMapping = new Dictionary<string, string>
            {
                { "id", "ID" },
                { "password", "Password" },
                { "user name", "User name" },
                { "room", "ROOM" },
                { "team", "Team" },
                { "role", "Role" },
                { "position", "Position" },
            };

            string Clean(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;

                return new string(input
                    .Where(c => !char.IsControl(c))
                    .ToArray())
                    .Replace("\u00A0", "")
                    .Replace("\u200B", "")
                    .Replace("\uFEFF", "")
                    .ToLowerInvariant()
                    .Trim();
            }

            int startRow = -1;
            for (int i = 0; i < 10; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                for (int j = 0; j < row.LastCellNum; j++)
                {
                    string value = Clean(dataFormatter.FormatCellValue(row.GetCell(j)));
                    if (value.Contains("id"))
                    {
                        startRow = i + 1;
                        for (int col = 0; col < row.LastCellNum; col++)
                        {
                            string headerText = Clean(dataFormatter.FormatCellValue(row.GetCell(col)));
                            if (columnMapping.ContainsKey(headerText))
                            {
                                columnIndexes[columnMapping[headerText]] = col;
                            }
                        }
                        break;
                    }
                }

                if (startRow != -1)
                    break;
            }

            if (startRow == -1)
                throw new KpiException(400, "format_not_found");

            string GetCellValue(IRow row, string columnKey)
            {
                if (!columnIndexes.TryGetValue(columnKey, out int colIndex))
                    return string.Empty;

                return dataFormatter.FormatCellValue(row.GetCell(colIndex)).Trim();
            }

            var users = await _userRepository.GetAll().ToListAsync();
            var teams = await _teamRepository.GetAll().ToListAsync();
            var rooms = await _roomRepository.GetAll().ToListAsync();

            foreach (var user in users)
                await _userRepository.DeleteAsync(user.Id);
            await _userRepository.SaveChangesAsync();

            foreach (var room in rooms)
                await _roomRepository.DeleteAsync(room.Id);
            await _roomRepository.SaveChangesAsync();

            foreach (var team in teams)
                await _teamRepository.DeleteAsync(team.Id);
            await _teamRepository.SaveChangesAsync();

            var positions = await _positionRepository.GetAll().ToListAsync();

            for (int row = startRow; row <= rowCount; row++)
            {
                var currentRow = sheet.GetRow(row);
                if (currentRow == null) continue;

                string teamName = GetCellValue(currentRow, "Team");
                string roomName = GetCellValue(currentRow, "ROOM");
                string positionName = GetCellValue(currentRow, "Position");


                Domain.Entities.Team.Team team = null;

                if (!string.IsNullOrWhiteSpace(teamName)) {
                    var exisTeam = await _teamRepository.GetAsync(x => x.Name == teamName);
                    if(exisTeam is null)
                    {
                        team = await _teamRepository.CreateAsync(new Domain.Entities.Team.Team { Name = teamName });
                        await _teamRepository.SaveChangesAsync();
                    }
                    else
                    {
                        team = exisTeam;
                    }
                }


                Domain.Entities.Room.Room room = null;
                if (!string.IsNullOrWhiteSpace(roomName))
                {
                    var existRoom = await _roomRepository.GetAsync(x => x.Name == roomName);
                    if (existRoom is null)
                    {
                        room = await _roomRepository.CreateAsync(new Domain.Entities.Room.Room { Name = roomName });
                        await _roomRepository.SaveChangesAsync();
                    }
                    else room = existRoom;
                }

                var position = positions.FirstOrDefault(p => p.Name.Trim().ToLower() == positionName.Trim().ToLower());

                var user = new Domain.Entities.User.User
                {
                    TeamId = team?.Id,
                    RoomId = room?.Id,
                    PositionId = position.Id,
                    Password = GetCellValue(currentRow, "Password").Encrypt(),
                    FullName = GetCellValue(currentRow, "User name"),
                    CreatedAt = DateTime.UtcNow,
                    Role = Enum.Parse<Role>(GetCellValue(currentRow, "Role"), ignoreCase: true),
                    UserName = GetCellValue(currentRow, "ID")
                };

                await _userRepository.CreateAsync(user);
            }

            await _userRepository.SaveChangesAsync();
            return true;
        }
    }
}
