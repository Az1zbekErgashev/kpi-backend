using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.Room;
using Kpi.Service.DTOs.Room;

namespace Kpi.Service.Interfaces.Room
{
    public interface IRoomService
    {
        ValueTask<List<RoomModel>> GetAsync();
        ValueTask<PagedResult<RoomModel>> GetAsync(RoomForFilterDTO dto);
        ValueTask<RoomModel> CreateAsync(RoomForCreateDTO @dto);
        ValueTask<bool> DeleteAsync(int id);
        ValueTask<RoomModel> GetByIdAsync(int id);
        ValueTask<RoomModel> UpdateAsync(RoomForUpdateDTO @dto);
    }
}
