namespace Titan_Fitness.Domain.Enums
{
    // 1. نطاق الوصول المسموح بالاشتراك
    public enum AccessScope
    {
        HomeBranchOnly = 1,
        AllBranches = 2
    }

    // 2. حالة اشتراك العضو
    public enum MembershipStatus
    {
        Pending = 1,
        Active = 2,
        Frozen = 3,
        Expired = 4,
        Cancelled = 5
    }

    // 3. نتيجة تسجيل الدخول (Check-In)
    public enum CheckInResult
    {
        Admitted = 1,
        Refused = 2
    }

    // 4. سبب طلب تجميد الاشتراك
    public enum FreezeReason
    {
        ExtendedTravel = 1,
        Injury = 2,
        Other = 3
    }

    // 5. حالة الحصة التدريبية (Class Session)
    public enum SessionStatus
    {
        Open = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    // 6. حالة حجز العضو في الحصة
    public enum BookingStatus
    {
        Booked = 1,
        Waitlisted = 2,
        Attended = 3,
        NoShow = 4,
        Cancelled = 5
    }
}