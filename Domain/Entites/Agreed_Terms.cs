using System;
using System.ComponentModel.DataAnnotations;
using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Domain.Entites;

public record Agreed_Terms
{
    [Key] // إضافة المفتاح الرئيسي لإنهاء خطأ EF Core
    public int Id { get; init; }

    // Foreign Key يربطه بالـ Membership
    public int MembershipId { get; init; }

    public decimal PricePaid { get; init; }
    public int DurationInMonths { get; init; }
    public int? MaxFreezeDays { get; init; }
    public int? MaxNumberOfFreezes { get; init; }
    public int? GuestPassQuota { get; init; }
    public AccessScope AccessScope { get; init; }

    public Agreed_Terms() { }

    private Agreed_Terms(
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

    public static Agreed_Terms Create(
        decimal pricePaid,
        int durationInMonths,
        int? maxFreezeDays,
        int? maxNumberOfFreezes,
        int? guestPassQuota,
        AccessScope accessScope)
    {
        if (pricePaid < 0)
            throw new ArgumentException("السعر المدفوع لا يمكن أن يكون بالسالب.");

        if (durationInMonths <= 0 || durationInMonths > 120)
            throw new ArgumentException("مدة الاشتراك بالشهور يجب أن تكون بين 1 و 120 شهراً.");

        if (maxFreezeDays < 0 || maxNumberOfFreezes < 0 || guestPassQuota < 0)
            throw new ArgumentException("حدود التجميد أو حصص الضيوف لا يمكن أن تكون بالسالب.");

        return new Agreed_Terms(
            Math.Round(pricePaid, 2),
            durationInMonths,
            maxFreezeDays,
            maxNumberOfFreezes,
            guestPassQuota,
            accessScope);
    }
}