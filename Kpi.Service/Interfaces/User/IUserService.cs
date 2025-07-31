using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.User;
using Kpi.Service.DTOs.User;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.Interfaces.User
{
    public interface IUserService
    {
        ValueTask<UserModel> CreateAsync(UserForCreateDTO @dto);
        ValueTask<UserModel> UpdateAsync(UserForUpdateDTO @dto);
        ValueTask<bool> DeleteAsync([Required] int id);
        ValueTask<PagedResult<UserModel>> GetAllAsync(UserForFilterDTO @dto);
        ValueTask<UserModel> GetByIdAsync([Required] int id);
        ValueTask<UserModel> GetByTokenAsync();
        ValueTask<PagedResult<UserModelForCEO>> GetUsersForCEO(UserForFilterCEOSideDTO @dto);
        ValueTask<List<PositionModel>> GetPositionAsync();
        ValueTask<PagedResult<UserModelForCEO>> GetUserListWithGoal(UserForFilterCEOSideDTO dto);
        ValueTask<PagedResult<UserModelForCEO>> GetTeamLeader(UserForFilterCEOSideDTO dto);
        ValueTask<UserModel> UpdateAsync(UserForUpdateByTokenDTO dto);
        ValueTask<bool> UploadExcelFileAsync(IFormFile file);
    }
}
