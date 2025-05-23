using Kpi.Domain.Models.User;
using Kpi.Service.DTOs.User;

namespace Kpi.Service.Interfaces.Auth
{
    public interface IAuthService
    {
        ValueTask<bool> CheckUserName(UserForCheckUserNameDTO @dto);
        ValueTask<AuthModel> LoginAsync(UserForLoginDTO @dto);
    }
}
