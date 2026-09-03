using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Domain.Entites
{
    public class Plan
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int DurationInMonths { get; private set; }
        public int? MaxFreezeDays { get; private set; }
        public int? MaxNumberOfFreezes { get; private set; }
        public int? GuestPassQuota { get; private set; }
        public AccessScope AccessScope { get; private set; }
        public bool IsPublished { get; private set; }

        private Plan() { }

        public static Plan Create(
            string name,
            decimal price,
            int durationInMonths,
            int? maxFreezeDays,
            int? maxNumberOfFreezes,
            int? guestPassQuota,
            AccessScope accessScope,
            bool isPublished = false)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
                throw new ArgumentException("اسم الخطة مطلوب ويجب ألا يتجاوز 50 حرفاً.");

            if (price < 0)
                throw new ArgumentException("السعر لا يمكن أن يكون بالسالب."); 

            if (durationInMonths <= 0)
                throw new ArgumentException("المدة بالشهور يجب أن تكون أكبر من 0."); 

            if (maxFreezeDays < 0 || maxNumberOfFreezes < 0 || guestPassQuota < 0)
                throw new ArgumentException("قيم التجميد أو حصص الضيوف لا يمكن أن تكون بالسالب.");

            return new Plan
            {
                Name = name,
                Price = Math.Round(price, 2),
                DurationInMonths = durationInMonths,
                MaxFreezeDays = maxFreezeDays,
                MaxNumberOfFreezes = maxNumberOfFreezes,
                GuestPassQuota = guestPassQuota,
                AccessScope = accessScope,
                IsPublished = isPublished
            };
        }

        public void Publish() => IsPublished = true;
        public void Unpublish() => IsPublished = false;
    }
}