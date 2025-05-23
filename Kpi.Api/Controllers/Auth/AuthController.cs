using Kpi.Domain.Models.Response;
using Kpi.Service.DTOs.User;
using Kpi.Service.Exception;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Kpi.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
    public async ValueTask<IActionResult> LoginAsync(UserForLoginDTO @dto) => ResponseHandler.ReturnIActionResponse(await authService.LoginAsync(@dto));


    [HttpPost("check-userId")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
    public async ValueTask<IActionResult> CheckUserName(UserForCheckUserNameDTO @dto) => ResponseHandler.ReturnIActionResponse(await authService.CheckUserName(@dto));
}
