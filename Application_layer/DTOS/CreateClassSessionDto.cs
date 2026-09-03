using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class CreateClassSessionDto
    {

        [Required(ErrorMessage = "اسم الحصة مطلوب.")]
        [StringLength(100)]
        public string ClassName { get; init; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int BranchId { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TrainerId { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int StudioId { get; init; }

        [Required]
        public DateTime Date { get; init; }

        [Required]
        public TimeSpan StartTime { get; init; }

        [Required]
        [Range(30, 60, ErrorMessage = "المدة يجب أن تكون 30، 45، أو 60 دقيقة.")]
        public int DurationInMinutes { get; init; }

        [Required]
        [Range(1, 500, ErrorMessage = "السعة ويجب أن تكون رقمًا موجبًا.")]
        public int CapacityLimit { get; init; }

        [StringLength(500)]
        public string? Description { get; init; }
    }
}