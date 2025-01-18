namespace ErtugrulGokayDumanHesVenturesCaseStudy.Services.PttScrapingServices
{
    public interface IPttScrapingService
    {
        Task<string> GetTrackingStatus(string trackingNumber);
    }
}
