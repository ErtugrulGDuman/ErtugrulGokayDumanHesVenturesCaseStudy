using System.ComponentModel.DataAnnotations;

namespace ErtugrulGokayDumanHesVenturesCaseStudy.DTOs
{
    public class TrackingRequestDto
    {
        [Required(ErrorMessage = "Tracking number is required.")]
        [StringLength(50, ErrorMessage = "Tracking number cannot exceed 50 characters.")]
        public string TrackingNumber { get; set; }
    }
}
