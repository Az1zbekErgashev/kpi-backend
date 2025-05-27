using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.Room;
using Kpi.Service.DTOs.Room;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.Room;
using Kpi.Service.StringExtensions;
using Microsoft.EntityFrameworkCore;

namespace Kpi.Service.Service.Room
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Domain.Entities.Room.Room> _roomRepository;

        public RoomService(IGenericRepository<Domain.Entities.Room.Room> roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async ValueTask<RoomModel> CreateAsync(RoomForCreateDTO @dto)
        {
            var room = new Domain.Entities.Room.Room()
            {
                Name = dto.Name,
            };

            await _roomRepository.CreateAsync(room);
            await _roomRepository.SaveChangesAsync();

            return new RoomModel().MapFromEntity(room);
        }
        public async ValueTask<RoomModel> UpdateAsync(RoomForUpdateDTO @dto)
        {
            var existRoom = await _roomRepository.GetAsync(x => x.Id == @dto.Id && x.IsDeleted == 0);

            if (existRoom == null) throw new KpiException(404, "room_not_found");

            existRoom.Name = dto.Name;
            existRoom.UpdatedAt = DateTime.UtcNow;

            _roomRepository.UpdateAsync(existRoom);
            await _roomRepository.SaveChangesAsync();

            return new RoomModel().MapFromEntity(existRoom);
        }

        public async ValueTask<bool> DeleteAsync(int id)
        {
            var existRoom = await _roomRepository.GetAsync(x => x.Id == id && x.IsDeleted == 0);

            if (existRoom == null) throw new KpiException(404, "room_not_found");

            existRoom.IsDeleted = 1;
            existRoom.UpdatedAt = DateTime.UtcNow;

            _roomRepository.UpdateAsync(existRoom);
            await _roomRepository.SaveChangesAsync();

            return true;
        }

        public async ValueTask<RoomModel> GetByIdAsync(int id)
        {
            var existRoom = await _roomRepository.GetAsync(x => x.Id == id && x.IsDeleted == 0);

            if (existRoom == null) throw new KpiException(404, "room_not_found");

            return new RoomModel().MapFromEntity(existRoom);
        }

        public async ValueTask<PagedResult<RoomModel>> GetAsync(RoomForFilterDTO dto)
        {
            var allRooms = _roomRepository.GetAll(x => x.IsDeleted == 0).Include(x => x.Users).OrderByDescending(x => x.UpdatedAt).AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name)) allRooms = allRooms.Where(x => x.Name.Contains(dto.Name));

            int totalCount = await allRooms.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<RoomModel>.Create(
                    Enumerable.Empty<RoomModel>(),
                    0,
                    dto.PageSize,
                    0,
                    dto.PageIndex,
                    0
                );
            }

            if (dto.PageIndex == 0)
            {
                dto.PageIndex = 1;
            }

            if (dto.PageSize == 0)
            {
                dto.PageSize = totalCount;
            }

            int itemsPerPage = dto.PageSize;
            int totalPages = (totalCount / itemsPerPage) + (totalCount % itemsPerPage == 0 ? 0 : 1);

            if (dto.PageIndex > totalPages)
            {
                dto.PageIndex = totalPages;
            }

            allRooms = allRooms.ToPagedList(dto);

            var list = await allRooms.ToListAsync();

            List<RoomModel> models = list.Select(
                f => new RoomModel().MapFromEntity(f))
                .ToList();

            var pagedResult = PagedResult<RoomModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<List<RoomModel>> GetAsync()
        {
            var allRooms = await _roomRepository.GetAll(x => x.IsDeleted == 0).ToListAsync();
            return allRooms.Select(x => new RoomModel().MapFromEntity(x)).ToList();
        }
    }
}
