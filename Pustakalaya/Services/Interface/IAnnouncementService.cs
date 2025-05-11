using Pustakalaya.DTOs;

namespace Pustakalaya.Services.Interface
{
    public interface IAnnouncementService
    {
        Task<IEnumerable<AnnouncementDto>> GetAllAnnouncementsAsync();
        Task<AnnouncementDto> GetAnnouncementByIdAsync(int id);
        Task<IEnumerable<AnnouncementDto>> GetActiveAnnouncementsAsync();
        Task<AnnouncementDto> CreateAnnouncementAsync(CreateAnnouncementDto announcementDto);
        Task<AnnouncementDto> UpdateAnnouncementAsync(int id, UpdateAnnouncementDto announcementDto);
        Task<bool> DeleteAnnouncementAsync(int id);
    }
}