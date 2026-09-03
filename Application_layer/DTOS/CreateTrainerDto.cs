using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class CreateTrainerDto

    {
        [Required(ErrorMessage = "اسم المدرب مطلوب.")]
        [StringLength(100)]
        public string FullName { get; init; } = string.Empty;

        [StringLength(100)]
        public string Specialty { get; init; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int BranchId { get; init; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; init; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; init; }

        public bool IsActive { get; init; } = true;
    }
}