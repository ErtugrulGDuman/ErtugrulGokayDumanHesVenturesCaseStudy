using ErtugrulGokayDumanHesVenturesCaseStudy.DTOs;
using ErtugrulGokayDumanHesVenturesCaseStudy.Models;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.Services.TrackingServices
{
    public interface ITrackingService
    {
        Task<TrackingInfo> CreateTrackingAsync(string trackingNumber);
        Task<IEnumerable<TrackingResponseDto>> GetAllTrackingsAsync();
        Task<bool> DeleteTrackingAsync(string trackingNumber);
        Task UpdateStatusesAsync();
    }
}
