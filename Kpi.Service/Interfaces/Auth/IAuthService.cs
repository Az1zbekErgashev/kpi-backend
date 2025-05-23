using Kpi.Service.DTOs.User;

namespace Kpi.Service.Interfaces.Auth
{
    public interface IAuthService
    {
        ValueTask<bool> CheckUserName(UserForCheckUserNameDTO @dto);
        ValueTask<string> LoginAsync(UserForLoginDTO @dto);
    }
}
