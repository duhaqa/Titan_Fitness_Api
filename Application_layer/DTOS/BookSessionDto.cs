using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class BookSessionDto
    {
        [Required]
        public int SessionId { get; init; }

        [Required]
        public int MemberId { get; init; }

        [StringLength(500, ErrorMessage = "الملاحظات الموجهة للمدرب لا تتجاوز 500 حرف.")]
        public string? SpecialRequirements { get; init; }
    }
}
