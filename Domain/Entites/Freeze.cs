using System;
using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Domain.Entites
{
    public class Freeze
    {
        public int Id { get; private set; } // Freeze Id[cite: 1]
        public int MembershipId { get; private set; } // Membership Id[cite: 1]
        public DateOnly StartDate { get; private set; } // Start date (date only, required)[cite: 1]
        public DateOnly EndDate { get; private set; } // End date (date only, required)[cite: 1]
        public int DurationInMonths { get; private set; } // Duration in months (int, required)[cite: 1]
        public FreezeReason Reason { get; private set; } // Reason (enum)[cite: 1]
        public string? AdditionalNotes { get; private set; } // Additional notes (max 200 char)[cite: 1]
        public DateTime RequestedOn { get; private set; } // Requested on (date time, required)[cite: 1]

        private Freeze() { }

        private Freeze(
            int membershipId,
            DateOnly startDate,
            DateOnly endDate,
            int durationInMonths,
            FreezeReason reason,
            string? additionalNotes)
        {
            MembershipId = membershipId;
            StartDate = startDate;
            EndDate = endDate;
            DurationInMonths = durationInMonths;
            Reason = reason;
            AdditionalNotes = additionalNotes;
            RequestedOn = DateTime.UtcNow;
        }

        public static Freeze Create(
            int membershipId,
            DateOnly startDate,
            DateOnly endDate,
            int durationInMonths,
            FreezeReason reason,
            string? additionalNotes)
        {
            if (membershipId <= 0)
                throw new ArgumentException("معرف الاشتراك غير صالح.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (startDate < today)
                throw new ArgumentException("لا يمكن بدء التجميد بتاريخ سابق لليوم."); 

            if (endDate <= startDate)
                throw new ArgumentException("تاريخ نهاية التجميد يجب أن يكون بعد تاريخ البداية.");

            if (durationInMonths <= 0)
                throw new ArgumentException("مدة التجميد يجب أن تكون شهر على الأقل."); 

            if (additionalNotes?.Length > 200)
                throw new ArgumentException("الملاحظات الإضافية يجب ألا تتجاوز 200 حرف."); 

            return new Freeze(membershipId, startDate, endDate, durationInMonths, reason, additionalNotes);
        }
    }
}