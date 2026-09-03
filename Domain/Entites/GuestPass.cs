using System;

namespace Titan_Fitness.Domain.Entites
{
    public class GuestPass
    {
        public int Id { get; private set; } // Guest pass Id[cite: 1]
        public int MembershipId { get; private set; } // Membership Id[cite: 1]
        public DateOnly IssuedOn { get; private set; } // Issued on (date, required)[cite: 1]
        public DateOnly? UsedOn { get; private set; } // Used on (date, nullable)[cite: 1]
        public string? GuestName { get; private set; } // Guest name (max 100 char, nullable)[cite: 1]

        private GuestPass() { }

        public static GuestPass Issue(int membershipId)
        {
            if (membershipId <= 0)
                throw new ArgumentException("معرف الاشتراك غير صالح.");

            return new GuestPass
            {
                MembershipId = membershipId,
                IssuedOn = DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }

        public void MarkAsUsed(string guestName)
        {
            if (UsedOn.HasValue)
                throw new InvalidOperationException("تذكرة الزائر تم استخدامها سابقاً."); 

            if (string.IsNullOrWhiteSpace(guestName) || guestName.Length > 100)
                throw new ArgumentException("اسم الزائر مطلوب ويجب ألا يتجاوز 100 حرف."); 

            GuestName = guestName;
            UsedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }
}