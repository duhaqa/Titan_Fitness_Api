using System.ComponentModel.DataAnnotations;

namespace Titan_Fitness.Application_layer.DTOS
{
    public class CreateMemberDto

    {
        [Required(ErrorMessage = "اسم العضو مطلوب.")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف.")]
        public string FullName { get; init; } = string.Empty;

        [StringLength(10, ErrorMessage = "رقم العضوية لا يتجاوز 10 خانات.")]
        public string? MembershipNumber { get; init; } // يترك فارغاً للتوليد التلقائي

        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        [StringLength(150)]
        public string? Email { get; init; }

        [Phone(ErrorMessage = "رقم الهاتف غير صالح.")]
        [StringLength(20)]
        public string? Phone { get; init; }

        [StringLength(250)]
        public string? Address { get; init; }

        [Required]
        public DateTime JoinedDate { get; init; } = DateTime.UtcNow;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار فرع صالح.")]
        public int HomeBranchId { get; init; }
    }
}