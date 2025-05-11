using Microsoft.EntityFrameworkCore;
using Pustakalaya.Data;
using Pustakalaya.DTOs;
using Pustakalaya.Entities;
using Pustakalaya.Services.Interface;

namespace Pustakalaya.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AppDbContext _context;

        public AnnouncementService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnnouncementDto>> GetAllAnnouncementsAsync()
        {
            var announcements = await _context.Announcements.ToListAsync();
            return announcements.Select(MapToDto);
        }

        public async Task<AnnouncementDto> GetAnnouncementByIdAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return null;
            }

            return MapToDto(announcement);
        }

        public async Task<IEnumerable<AnnouncementDto>> GetActiveAnnouncementsAsync()
        {
            var currentDate = DateTime.UtcNow;
            var announcements = await _context.Announcements
                .Where(a => a.IsActive && a.StartDate <= currentDate && a.EndDate >= currentDate)
                .ToListAsync();

            return announcements.Select(MapToDto);
        }

        public async Task<AnnouncementDto> CreateAnnouncementAsync(CreateAnnouncementDto announcementDto)
        {
            var announcement = new Announcement
            {
                Title = announcementDto.Title,
                Content = announcementDto.Content,
                StartDate = announcementDto.StartDate,
                EndDate = announcementDto.EndDate,
                IsActive = true
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return MapToDto(announcement);
        }

        public async Task<AnnouncementDto> UpdateAnnouncementAsync(int id, UpdateAnnouncementDto announcementDto)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return null;
            }

            announcement.Title = announcementDto.Title;
            announcement.Content = announcementDto.Content;
            announcement.StartDate = announcementDto.StartDate;
            announcement.EndDate = announcementDto.EndDate;
            announcement.IsActive = announcementDto.IsActive;

            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();

            return MapToDto(announcement);
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return false;
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return true;
        }

        private AnnouncementDto MapToDto(Announcement announcement)
        {
            return new AnnouncementDto
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate,
                IsActive = announcement.IsActive
            };
        }
    }
}