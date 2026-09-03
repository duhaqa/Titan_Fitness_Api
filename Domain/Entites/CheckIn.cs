using System;
using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Domain.Entites
{
    public class CheckIn
    {
        public int Id { get; private set; } // Check-in Id        
        public int MemberId { get; private set; } // Member Id        
        public int BranchId { get; private set; } // Branch Id        
        public DateTime CheckInDateTime { get; private set; } // Check-in date time (date time, required)        
        public CheckInResult Result { get; private set; } // Result (enum)        
        public string? RefusalReason { get; private set; } // Refusal reason (max 100 char)        

        private CheckIn() { }

        private CheckIn(int memberId, int branchId, CheckInResult result, string? refusalReason)
        {
            MemberId = memberId;
            BranchId = branchId;
            CheckInDateTime = DateTime.UtcNow;
            Result = result;
            RefusalReason = refusalReason;
        }

        // Factory Method عند السماح بالدخول
        public static CheckIn CreateAdmitted(int memberId, int branchId)
        {
            if (memberId <= 0 || branchId <= 0)
                throw new ArgumentException("معرف العضو والفرع يجب أن تكون أرقاماً صالحة.");

            return new CheckIn(memberId, branchId, CheckInResult.Admitted, null);         
        }

        // Factory Method عند رفض الدخول وتسجيل السبب
        public static CheckIn CreateRefused(int memberId, int branchId, string refusalReason)
        {
            if (memberId <= 0 || branchId <= 0)
                throw new ArgumentException("معرف العضو والفرع يجب أن تكون أرقاماً صالحة.");

            if (string.IsNullOrWhiteSpace(refusalReason))
                throw new ArgumentException("يجب كتابة سبب الرفض.");         

            if (refusalReason.Length > 100)
                throw new ArgumentException("سبب الرفض يجب ألا يتجاوز 100 حرف.");         

            return new CheckIn(memberId, branchId, CheckInResult.Refused, refusalReason);         
        }
    }
}