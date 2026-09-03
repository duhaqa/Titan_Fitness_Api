using System;
using Titan_Fitness.Domain.Enums;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Domain.Entites
{
    public class Membership
    {
        public int Id { get; private set; } // Membership Id[cite: 1]
        public int MemberId { get; private set; }
          
        public int PlanId { get; private set; }
          
        public DateTime PurchaseDate { get; private set; }
          
        public DateOnly StartDate { get; private set; }
          
        public DateOnly EndDate { get; private set; }
          
        public MembershipStatus Status { get; private set; }
          

        // Value Object الشروط المتفق عليها عند الشراء[cite: 1]
        public AgreedTerms AgreedTerms { get; private set; } = null!; 

        private Membership() { }

        private Membership(int memberId, int planId, DateOnly startDate, AgreedTerms agreedTerms)
        {
            MemberId = memberId;
            PlanId = planId;
            PurchaseDate = DateTime.UtcNow;
            StartDate = startDate;
            // حساب تاريخ النهاية تلقائياً بناءً على أشهر الخطة المتفق عليها[cite: 1]
            EndDate = startDate.AddMonths(agreedTerms.DurationInMonths);
            AgreedTerms = agreedTerms;

            // إذا كان تاريخ البداية في المستقبل تكون الحالة Pending، وإلا Active[cite: 1]
            Status = startDate > DateOnly.FromDateTime(DateTime.UtcNow)
                ? MembershipStatus.Pending
                : MembershipStatus.Active;
        }

        public static Membership Create(int memberId, int planId, DateOnly startDate, AgreedTerms agreedTerms)
        {
            if (memberId <= 0 || planId <= 0)
                throw new ArgumentException("معرف العضو والخطة يجب أن تكون أرقاماً صالحة.");

            if (agreedTerms == null)
                throw new ArgumentNullException(nameof(agreedTerms), "شروط الاشتراك مطلوبة.");

            return new Membership(memberId, planId, startDate, agreedTerms);
        }

        // منطق عمل تمديد نهاية الاشتراك عند التجميد (Resuming Freeze)[cite: 1]
        public void ExtendEndDate(int freezeDays)
        {
            if (freezeDays <= 0)
                throw new ArgumentException("عدد أيام التجميد يجب أن يكون أكبر من 0.");

            EndDate = EndDate.AddDays(freezeDays);
        }

        public void ChangeStatus(MembershipStatus newStatus)
        {
            Status = newStatus;
        }
    }
}