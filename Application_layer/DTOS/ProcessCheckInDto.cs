using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class ProcessCheckInDto
    {
        [Required]
        public int MemberId { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BranchId { get; init; }
    }
}