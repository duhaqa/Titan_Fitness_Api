using System;
using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Domain.Entites
{
    public class ClassSession
    {
        public int Id { get; private set; } // Session Id[cite: 1]
        public string ClassName { get; private set; } = null!; // max 100 char, required[cite: 1]
        public int BranchId { get; private set; } // Branch Id[cite: 1]
        public int StudioId { get; private set; } // Studio Id[cite: 1]
        public int TrainerId { get; private set; } // Trainer Id[cite: 1]

        public DateOnly SessionDate { get; private set; } // required[cite: 1]
        public TimeSpan StartTime { get; private set; } // required[cite: 1]
        public int DurationInMinutes { get; private set; } // required (30, 45, 60)[cite: 1]
        public int CapacityLimit { get; private set; } // required[cite: 1]
        public SessionStatus Status { get; private set; } // Enum[cite: 1]
        public string? Description { get; private set; } // max 500 char[cite: 1]

        private ClassSession() { }

        public static ClassSession Create(
            string className,
            int branchId,
            int studioId,
            int trainerId,
            DateOnly sessionDate,
            TimeSpan startTime,
            int durationInMinutes,
            int capacityLimit,
            string? description)
        {
            if (string.IsNullOrWhiteSpace(className) || className.Length > 100)
                throw new ArgumentException("اسم الحصة مطلوب ومحدود بـ 100 حرف."); 

            if (branchId <= 0 || studioId <= 0 || trainerId <= 0)
                throw new ArgumentException("المعرفات الخاصة بالفرع والاستوديو والمدرب يجب أن تكون صالحة.");

            if (durationInMinutes != 30 && durationInMinutes != 45 && durationInMinutes != 60)
                throw new ArgumentException("مدة الحصة يجب أن تكون إما 30، 45، أو 60 دقيقة."); 

            if (capacityLimit <= 0)
                throw new ArgumentException("سعة الحصة يجب أن تكون أكبر من 0."); 

            if (description?.Length > 500)
                throw new ArgumentException("الوصف يجب ألا يتجاوز 500 حرف."); 

            return new ClassSession
            {
                ClassName = className,
                BranchId = branchId,
                StudioId = studioId,
                TrainerId = trainerId,
                SessionDate = sessionDate,
                StartTime = startTime,
                DurationInMinutes = durationInMinutes,
                CapacityLimit = capacityLimit,
                Status = SessionStatus.Open,
              
                Description = description
            };
        }

        public void ChangeStatus(SessionStatus newStatus)
        {
            Status = newStatus;
        }
    }
}