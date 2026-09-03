using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class UpdateMemberDto
    {
        [Required]
        public int Id { get; init; }

        [Required]
        [StringLength(100)]
        public string FullName { get; init; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; init; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; init; }

        [StringLength(250)]
        public string? Address { get; init; }

        [Required]
        public DateTime JoinedDate { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int HomeBranchId { get; init; }
    }
}