using ErtugrulGokayDumanHesVenturesCaseStudy.Models;
using Microsoft.EntityFrameworkCore;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<TrackingInfo> TrackingInfos { get; set; }
    }
}
