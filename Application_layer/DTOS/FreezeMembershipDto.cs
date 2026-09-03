using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class FreezeMembershipDto
    {
        [Required]
        public int MemberId { get; init; }

        [Required]
        public DateTime StartDate { get; init; }

        [Required]
        [Range(1, 12, ErrorMessage = "مدة التجميد بالشهور غير صالحة.")]
        public int DurationInMonths { get; init; }

        [Required(ErrorMessage = "سبب التجميد مطلوب.")]
        [StringLength(100)]
        public string Reason { get; init; } = string.Empty;

        [StringLength(500, ErrorMessage = "الملاحظات الإضافية يجب ألا تتجاوز 500 حرف.")]
        public string? AdditionalNotes { get; init; }
    }
}
