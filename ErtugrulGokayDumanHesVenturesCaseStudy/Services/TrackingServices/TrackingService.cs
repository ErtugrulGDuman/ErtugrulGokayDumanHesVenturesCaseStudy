using ErtugrulGokayDumanHesVenturesCaseStudy.Data;
using ErtugrulGokayDumanHesVenturesCaseStudy.DTOs;
using ErtugrulGokayDumanHesVenturesCaseStudy.Models;
using ErtugrulGokayDumanHesVenturesCaseStudy.Services.PttScrapingServices;
using Microsoft.EntityFrameworkCore;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.Services.TrackingServices
{
    public class TrackingService : ITrackingService
    {
        private readonly AppDbContext _context;
        private readonly IPttScrapingService _pttScrapingService;
        private readonly ILogger<TrackingService> _logger;

        public TrackingService(
            AppDbContext context,
            IPttScrapingService pttScrapingService,
            ILogger<TrackingService> logger)
        {
            _context = context;
            _pttScrapingService = pttScrapingService;
            _logger = logger;
        }

        public async Task<TrackingInfo> CreateTrackingAsync(string trackingNumber)
        {
            // Aynı tracking number'ın daha önce eklenip eklenmediğini kontrol ettik
            var existingTracking = await _context.TrackingInfos
                .FirstOrDefaultAsync(t => t.TrackingNumber == trackingNumber);

            if (existingTracking != null)
            {
                throw new InvalidOperationException($"Tracking number {trackingNumber} already exists.");
            }

            var tracking = new TrackingInfo
            {
                TrackingNumber = trackingNumber,
                Status = "Pending",
                LastUpdated = DateTime.UtcNow
            };

            try
            {
                _context.TrackingInfos.Add(tracking);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new tracking for number: {TrackingNumber}", trackingNumber);
                return tracking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tracking for number: {TrackingNumber}", trackingNumber);
                throw;
            }
        }

        public async Task<IEnumerable<TrackingResponseDto>> GetAllTrackingsAsync()
        {
            try
            {
                var trackings = await _context.TrackingInfos
                    .Select(t => new TrackingResponseDto
                    {
                        TrackingNumber = t.TrackingNumber,
                        Status = t.Status
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} tracking records", trackings.Count);
                return trackings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tracking records");
                throw;
            }
        }

        public async Task<bool> DeleteTrackingAsync(string trackingNumber)
        {
            try
            {
                var tracking = await _context.TrackingInfos
                    .FirstOrDefaultAsync(t => t.TrackingNumber == trackingNumber);

                if (tracking == null)
                {
                    _logger.LogWarning("Tracking number not found: {TrackingNumber}", trackingNumber);
                    return false;
                }

                _context.TrackingInfos.Remove(tracking);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted tracking number: {TrackingNumber}", trackingNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tracking number: {TrackingNumber}", trackingNumber);
                throw;
            }
        }

        public async Task UpdateStatusesAsync()
        {
            try
            {
                var trackings = await _context.TrackingInfos.ToListAsync();
                foreach (var tracking in trackings)
                {
                    tracking.Status = await _pttScrapingService.GetTrackingStatus(tracking.TrackingNumber);
                    tracking.LastUpdated = DateTime.UtcNow;
                    _logger.LogInformation(
                        "Updated status for tracking number {TrackingNumber}: {Status}",
                        tracking.TrackingNumber,
                        tracking.Status);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tracking statuses");
                throw;
            }
        }
    }
}