using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class RenewMembershipDto
    {
        [Required]
        public int MemberId { get; init; }
    }
}