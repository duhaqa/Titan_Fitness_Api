using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class ChangePlanDto
    {
        [Required]
        public int MemberId { get; init; }

        [Required]
        public int NewPlanId { get; init; }

        [Required]
        public bool Immediately { get; init; }  // True = فوراً، False = عند الانتهاء (At renewal)
        public string Notes { get; set; } = string.Empty;
    }
}
