﻿using Kpi.Domain.Models.Response;
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
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] UserForFilterDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetAllAsync(@dto));

        [HttpGet("position")]
        [ProducesResponseType(typeof(ResponseModel<PositionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetPositionAsync() => ResponseHandler.ReturnIActionResponse(await _userRepository.GetPositionAsync());

        [HttpGet("filter-ceo")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetUsersForCEO([FromQuery] UserForFilterCEOSideDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetUsersForCEO(@dto));

        [HttpGet("filter-teams")]
        [Authorize(Roles = "TeamLeader,TeamMember")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetUserListWithGoal([FromQuery] UserForFilterCEOSideDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetUserListWithGoal(@dto));

        [HttpGet("team-leader")]
        [Authorize(Roles = "TeamLeader")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetTeamLeader([FromQuery] UserForFilterCEOSideDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetTeamLeader(@dto));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByIdAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _userRepository.GetByIdAsync(id));

        [HttpGet("profile")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByTokenAsync() => ResponseHandler.ReturnIActionResponse(await _userRepository.GetByTokenAsync());

        [HttpDelete("{id}")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> DeleteAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _userRepository.DeleteAsync(id));

        [HttpPost("create")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateAsync(UserForCreateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.CreateAsync(@dto));

        [HttpPut("update")]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> UpdateAsync(UserForUpdateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.UpdateAsync(@dto));

        [HttpPut]
        [ProducesResponseType(typeof(ResponseModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> UpdateAsync(UserForUpdateByTokenDTO @dto) => ResponseHandler.ReturnIActionResponse(await _userRepository.UpdateAsync(@dto));

        [HttpPost]
        public async ValueTask<IActionResult> UploadExcelFile(IFormFile file) => ResponseHandler.ReturnIActionResponse(await _userRepository.UploadExcelFileAsync(file));

    }
}
