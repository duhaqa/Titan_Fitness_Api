using System;
using Titan_Fitness.Domain.Enums;

//using static titan_fitness.Enums.Enums;

namespace Titan_Fitness.Domain.Value_object
{
    // 1. Value Object لـ العنوان
    public record Address
    {
        public string Value { get; init; } = string.Empty;

        private Address() { } // مطلوب لـ EF Core
        private Address(string value) => Value = value;

        public static Address Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("العنوان مطلوب.");

            if (value.Length > 200)
                throw new ArgumentException("العنوان يجب ألا يتجاوز 200 حرف.");

            return new Address(value);
        }
    }

    // 2. Value Object لـ رقم الهاتف
    public record Phone
    {
        public string Value { get; init; } = string.Empty;

        private Phone() { } // مطلوب لـ EF Core
        private Phone(string value) => Value = value;

        public static Phone Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("رقم الهاتف مطلوب.");

            if (value.Length > 20)
                throw new ArgumentException("رقم الهاتف يجب ألا يتجاوز 20 حرفاً.");

            return new Phone(value);
        }
    }

    // 3. Value Object لـ السعر/المبلغ المالي
    public record Price
    {
        public decimal Value { get; init; }

        private Price() { } // مطلوب لـ EF Core
        private Price(decimal value) => Value = value;

        public static Price Create(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("السعر لا يمكن أن يكون بالسالب.");

            decimal roundedValue = Math.Round(value, 2);

            return new Price(roundedValue);
        }
    }

    // 4. Value Object لـ أوقات الافتتاح والإغلاق
    public record TimeRange
    {
        public TimeSpan OpeningTime { get; init; }
        public TimeSpan ClosingTime { get; init; }

        private TimeRange() { } // مطلوب لـ EF Core
        private TimeRange(TimeSpan openingTime, TimeSpan closingTime)
        {
            OpeningTime = openingTime;
            ClosingTime = closingTime;
        }

        public static TimeRange Create(TimeSpan openingTime, TimeSpan closingTime)
        {
            if (closingTime <= openingTime)
                throw new ArgumentException("وقت الإغلاق يجب أن يكون بعد وقت الافتتاح.");

            return new TimeRange(openingTime, closingTime);
        }
    }

    // 5. Value Object لـ شروط الاشتراك المتفق عليها عند الشراء
    public record AgreedTerms
    {
        public decimal PricePaid { get; init; }
        public int DurationInMonths { get; init; }
        public int? MaxFreezeDays { get; init; }
        public int? MaxNumberOfFreezes { get; init; }
        public int? GuestPassQuota { get; init; }
        public AccessScope AccessScope { get; init; }

        private AgreedTerms() { } // مطلوب لـ EF Core

        private AgreedTerms(
            decimal pricePaid,
            int durationInMonths,
            int? maxFreezeDays,
            int? maxNumberOfFreezes,
            int? guestPassQuota,
            AccessScope accessScope)
        {
            PricePaid = pricePaid;
            DurationInMonths = durationInMonths;
            MaxFreezeDays = maxFreezeDays;
            MaxNumberOfFreezes = maxNumberOfFreezes;
            GuestPassQuota = guestPassQuota;
            AccessScope = accessScope;
        }

        public static AgreedTerms Create(
            decimal pricePaid,
            int durationInMonths,
            int? maxFreezeDays,
            int? maxNumberOfFreezes,
            int? guestPassQuota,
            AccessScope accessScope)
        {
            if (pricePaid < 0)
                throw new ArgumentException("السعر المدفوع غير صالح.");

            if (durationInMonths <= 0)
                throw new ArgumentException("المدة الشهرية يجب أن تكون أكبر من صفر.");

            return new AgreedTerms(pricePaid, durationInMonths, maxFreezeDays, maxNumberOfFreezes, guestPassQuota, accessScope);
        }
    }
}