using Kpi.Domain.Entities.Attachment;
using Kpi.Domain.Entities.Country;
using Kpi.Domain.Entities.MultilingualText;
using Kpi.Service.Interfaces.Attachment;
using Kpi.Service.Interfaces.Country;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.MultilingualText;
using Kpi.Service.Service.Attachment;
using Kpi.Service.Service.Country;
using Kpi.Service.Service.MultilingualText;
using Kpi.Service.Service.Repositories;
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
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddDependencies();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IMultilingualTextInterface, MultilingualTextService>();
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
