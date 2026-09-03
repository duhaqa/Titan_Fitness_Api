using System.ComponentModel.DataAnnotations;
using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Application_layer.DTOS

{
    public class CreatePlanDto
    {
        [Required]
        [StringLength(100)]
        public string PlanName { get; init; } = string.Empty;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "السعر يجب أن يكون مبلغًا موجبًا.")]
        public decimal Price { get; init; }

        [Required]
        [Range(1, 120, ErrorMessage = "المدة بالشهور يجب أن تكون بين 1 و 120.")]
        public int DurationInMonths { get; init; }

        public bool IsPublished { get; init; } = true;

        [Range(0, 365)]
        public int? MaxFreezeDays { get; init; }

        [Range(0, 50)]
        public int? MaxNumberOfFreezes { get; init; }

        [Range(0, 100)]
        public int? GuestPassQuota { get; init; }

        [Required]
        public AccessScope AccessScope { get; init; }
    }
}