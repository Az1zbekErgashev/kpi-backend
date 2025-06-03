using Kpi.Domain.Models.Response;
using Kpi.Domain.Models.User;
using Kpi.Service.DTOs.User;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Api.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userRepository;

        public UserController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] UserForFilterDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetAllAsync(@dto));

        [HttpGet("filter-ceo")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetUsersForCEO([FromQuery] UserForFilterCEOSideDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetUsersForCEO(@dto));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByIdAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetByIdAsync(id));

        [HttpGet("profile")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByTokenAsync() => ResponseHandler.ReturnIActionResponse(await _userRepository.GetByTokenAsync());

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> DeleteAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _userRepository.DeleteAsync(id));

        [HttpPost("create")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateAsync(UserForCreateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.CreateAsync(@dto));

        [HttpPut("update")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> UpdateAsync(UserForUpdateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.UpdateAsync(@dto));
    }
}
