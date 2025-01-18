using ErtugrulGokayDumanHesVenturesCaseStudy.Data;
using HtmlAgilityPack;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.Hangfire
{
    public class StatusUpdaterJob
    {
        private readonly AppDbContext _context;

        public StatusUpdaterJob(AppDbContext context)
        {
            _context = context;
        }

        public void UpdateStatuses()
        {
            foreach (var tracking in _context.TrackingInfos.ToList())
            {
                var status = ScrapeTrackingStatus(tracking.TrackingNumber);
                if (!string.IsNullOrEmpty(status))
                {
                    tracking.Status = status;
                    _context.SaveChanges();
                }
            }
        }

        private string ScrapeTrackingStatus(string trackingNumber)
        {
            try
            {
                var url = $"https://gonderitakip.ptt.gov.tr/";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                return "In Transit"; // Test verisi.
            }
            catch
            {
                return null;
            }
        }
    }
}
