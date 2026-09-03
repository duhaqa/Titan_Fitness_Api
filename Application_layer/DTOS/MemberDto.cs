namespace Titan_Fitness.Application_layer.DTOS
{
    public class MemberDto
    {
        public int Id { get; init; }
        public string MembershipNumber { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public DateTime JoinedDate { get; init; }
        public int HomeBranchId { get; init; }
        public string HomeBranchName { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty; // Active, Frozen, Expired
        public DateTime? LastVisit { get; init; }
    }
}