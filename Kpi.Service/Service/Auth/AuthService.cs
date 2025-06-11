using Kpi.Domain.Enum;
using Kpi.Domain.Models.User;
using Kpi.Service.DTOs.User;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Auth;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.StringExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kpi.Service.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<Domain.Entities.User.User> _userRepository;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IGenericRepository<Domain.Entities.User.User> userRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AuthModel> LoginAsync(UserForLoginDTO @dto)
        {
            var existUser = await _userRepository.GetAll(x => x.UserName == dto.UserName && x.Password.Equals(dto.Password.Encrypt()) && x.IsDeleted == 0 && (x.Role == Domain.Enum.Role.Ceo || x.Role == Domain.Enum.Role.Director)).FirstOrDefaultAsync();

            if (existUser is null)
                throw new KpiException(400, "login_or_password_is_incorrect", false);

            var token = await GenerateToken(existUser.Id, existUser.Role, existUser.TeamId);

            return new AuthModel().MapFromEntity(token, existUser);
        }    
        
        public async ValueTask<AuthModel> LoginUserAsync(UserForLoginDTO @dto)
        {
            var existUser = await _userRepository.GetAll(x => x.UserName == dto.UserName && x.Password.Equals(dto.Password.Encrypt()) && x.IsDeleted == 0 && (x.Role == Domain.Enum.Role.TeamMember || x.Role == Domain.Enum.Role.TeamLeader)).FirstOrDefaultAsync();

            if (existUser is null)
                throw new KpiException(400, "login_or_password_is_incorrect", false);

            var token = await GenerateToken(existUser.Id, existUser.Role, existUser.TeamId);

            return new AuthModel().MapFromEntity(token, existUser);
        }

        public async ValueTask<bool> CheckUserName(UserForCheckUserNameDTO @dto)
        {
            var existUser = await _userRepository.GetAsync(x => x.UserName == dto.UserName);

            if (existUser == null) return true;
            return false;
        }


        private async ValueTask<string> GenerateToken(int userId, Role role, int? teamId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
            };

            if(teamId != null)
            {
                claims.Add(new Claim(ClaimTypes.Country, teamId.ToString()));
            }

            return await ValueTask.FromResult(TokenGenerator(claims));
        }

        public string TokenGenerator(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                expires: DateTime.Now.AddHours(int.Parse(configuration["JWT:Expire"])),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    key: authSigningKey,
                    algorithm: SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
