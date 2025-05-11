using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustakalaya.DTOs;
using Pustakalaya.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pustakalaya.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetAllAnnouncements()
        {
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            return Ok(announcements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementDto>> GetAnnouncement(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }

            return Ok(announcement);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetActiveAnnouncements()
        {
            var announcements = await _announcementService.GetActiveAnnouncementsAsync();
            return Ok(announcements);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AnnouncementDto>> CreateAnnouncement(CreateAnnouncementDto announcementDto)
        {
            var announcement = await _announcementService.CreateAnnouncementAsync(announcementDto);
            return CreatedAtAction(nameof(GetAnnouncement), new { id = announcement.Id }, announcement);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AnnouncementDto>> UpdateAnnouncement(int id, UpdateAnnouncementDto announcementDto)
        {
            var announcement = await _announcementService.UpdateAnnouncementAsync(id, announcementDto);
            if (announcement == null)
            {
                return NotFound();
            }

            return Ok(announcement);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAnnouncement(int id)
        {
            var result = await _announcementService.DeleteAnnouncementAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}