using ErtugrulGokayDumanHesVenturesCaseStudy.Data;
using ErtugrulGokayDumanHesVenturesCaseStudy.DTOs;
using ErtugrulGokayDumanHesVenturesCaseStudy.Models;
using ErtugrulGokayDumanHesVenturesCaseStudy.Services.TrackingServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingService _trackingService;

        public TrackingController(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        [HttpPost]
        public async Task<ActionResult<TrackingInfo>> Create([FromBody] TrackingRequestDto request)
        {
            var tracking = await _trackingService.CreateTrackingAsync(request.TrackingNumber);
            return Ok(tracking);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackingResponseDto>>> GetAll()
        {
            var trackings = await _trackingService.GetAllTrackingsAsync();
            return Ok(trackings);
        }

        [HttpDelete("{trackingNumber}")]
        public async Task<ActionResult> Delete(string trackingNumber)
        {
            var result = await _trackingService.DeleteTrackingAsync(trackingNumber);
            if (!result)
                return NotFound();
            return Ok();
        }
    }
}
