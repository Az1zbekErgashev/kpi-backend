using Kpi.Domain.Models.Response;
using Kpi.Domain.Models.Room;
using Kpi.Service.DTOs.Room;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Api.Controllers.Room
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Ceo,Director")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(ResponseModel<RoomModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] RoomForFilterDTO @dto) => ResponseHandler.ReturnIActionResponse(await _roomService.GetAsync(@dto));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<RoomModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByIdAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _roomService.GetByIdAsync(id));

        [HttpGet("list")]
        [ProducesResponseType(typeof(ResponseModel<List<RoomModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAsync() => ResponseHandler.ReturnIActionResponse(await _roomService.GetAsync());

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> DeleteAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _roomService.DeleteAsync(id));

        [HttpPost("create")]
        [ProducesResponseType(typeof(ResponseModel<RoomModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateAsync(RoomForCreateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _roomService.CreateAsync(@dto));

        [HttpPut("update")]
        [ProducesResponseType(typeof(ResponseModel<RoomModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> UpdateAsync(RoomForUpdateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _roomService.UpdateAsync(@dto));
    }
}
