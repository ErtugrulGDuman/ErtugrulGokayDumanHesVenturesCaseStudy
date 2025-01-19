﻿using ErtugrulGokayDumanHesVenturesCaseStudy.Data;
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
        private readonly ILogger<TrackingController> _logger;

        public TrackingController(ITrackingService trackingService, ILogger<TrackingController> logger)
        {
            _trackingService = trackingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TrackingInfo>> Create([FromBody] TrackingRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for tracking request.");
                return BadRequest(ModelState); // Doğrulama hatalarını döndür
            }

            if (request == null)
            {
                _logger.LogWarning("Received null tracking request.");
                return BadRequest("Tracking request cannot be null.");
            }

            try
            {
                _logger.LogInformation("Creating tracking for number: {TrackingNumber}", request.TrackingNumber);
                var result = await _trackingService.CreateTrackingAsync(request.TrackingNumber);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Duplicate tracking number: {TrackingNumber}", request.TrackingNumber);
                return Conflict(new { Message = ex.Message }); // Çift kayıt için 409 hatası
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tracking");
                return StatusCode(500, "An error occurred while creating the tracking");
            }
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
