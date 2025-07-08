using Kpi.Domain.Entities.Attachment;
using Kpi.Domain.Entities.Comment;
using Kpi.Domain.Entities.Country;
using Kpi.Domain.Entities.Goal;
using Kpi.Domain.Entities.MultilingualText;
using Kpi.Domain.Entities.Room;
using Kpi.Domain.Entities.Team;
using Kpi.Domain.Entities.User;
using Kpi.Service.Interfaces.Attachment;
using Kpi.Service.Interfaces.Auth;
using Kpi.Service.Interfaces.Country;
using Kpi.Service.Interfaces.Evaluation;
using Kpi.Service.Interfaces.Goal;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.MonthlyTarget;
using Kpi.Service.Interfaces.MultilingualText;
using Kpi.Service.Interfaces.Room;
using Kpi.Service.Interfaces.Team;
using Kpi.Service.Interfaces.User;
using Kpi.Service.Service.Attachment;
using Kpi.Service.Service.Auth;
using Kpi.Service.Service.Country;
using Kpi.Service.Service.Evaluation;
using Kpi.Service.Service.Goal;
using Kpi.Service.Service.MonthlyTarget;
using Kpi.Service.Service.MultilingualText;
using Kpi.Service.Service.Repositories;
using Kpi.Service.Service.Room;
using Kpi.Service.Service.Team;
using Kpi.Service.Service.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace Kpi.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<IGenericRepository<Attachment>, GenericRepository<Attachment>>();
            services.AddScoped<IGenericRepository<Country>, GenericRepository<Country>>();
            services.AddScoped<IGenericRepository<MultilingualText>, GenericRepository<MultilingualText>>();
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<Room>, GenericRepository<Room>>();
            services.AddScoped<IGenericRepository<Team>, GenericRepository<Team>>();
            services.AddScoped<IGenericRepository<Goal>, GenericRepository<Goal>>();
            services.AddScoped<IGenericRepository<KpiGoal>, GenericRepository<KpiGoal>>();
            services.AddScoped<IGenericRepository<TargetValue>, GenericRepository<TargetValue>>();
            services.AddScoped<IGenericRepository<Comment>, GenericRepository<Comment>>();
            services.AddScoped<IGenericRepository<Division>, GenericRepository<Division>>();
            services.AddScoped<IGenericRepository<MonthlyTargetComment>, GenericRepository<MonthlyTargetComment>>();
            services.AddScoped<IGenericRepository<MonthlyPerformance>, GenericRepository<MonthlyPerformance>>();
            services.AddScoped<IGenericRepository<MonthlyTargetValue>, GenericRepository<MonthlyTargetValue>>();
            services.AddScoped<IGenericRepository<MonthlyTargetValue>, GenericRepository<MonthlyTargetValue>>();
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddDependencies();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IMultilingualTextInterface, MultilingualTextService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IGoalService, GoalService>();
            services.AddScoped<IMonthlyTargetService, MonthlyTargetService>();
            services.AddScoped<IEvaluationService, EvaluationService>();
        }
        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(p =>
            {
                p.ResolveConflictingActions(ad => ad.First());
                p.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });

                p.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "project-management",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3249f3f0-8b1e-4ebb-a8ee-1e40b0e90034dasfdq-asd23da"))
                };
            })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }
    }
}
