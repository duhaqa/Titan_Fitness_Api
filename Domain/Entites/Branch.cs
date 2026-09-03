namespace Titan_Fitness.Domain.Entites;

using System;
using Titan_Fitness.Domain.Value_object;

public class Branch
{
    public int Id { get; private set; } // Branch Id[cite: 1]
    public string Name { get; private set; } = null!; // max 50 char, required[cite: 1]

    // Value Objects
    public Address Address { get; private set; } = null!; // max 200 char[cite: 1]
    public TimeRange WorkingHours { get; private set; } = null!; // Opening & closing time[cite: 1]

    private Branch() { }

    private Branch(string name, Address address, TimeRange workingHours)
    {
        Name = name;
        Address = address;
        WorkingHours = workingHours;
    }

    public static Branch Create(string name, Address address, TimeRange workingHours)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("اسم الفرع مطلوب."); 

        if (name.Length > 50)
            throw new ArgumentException("اسم الفرع يجب ألا يتجاوز 50 حرفاً."); 

        if (address == null)
            throw new ArgumentNullException(nameof(address), "عنوان الفرع مطلوب.");

        if (workingHours == null)
            throw new ArgumentNullException(nameof(workingHours), "ساعات العمل مطلوبة.");

        return new Branch(name, address, workingHours);
    }

    public void UpdateDetails(string name, Address address, TimeRange workingHours)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            throw new ArgumentException("اسم الفرع غير صالح."); 

        if (address == null)
            throw new ArgumentNullException(nameof(address), "عنوان الفرع غير صالح.");

        if (workingHours == null)
            throw new ArgumentNullException(nameof(workingHours), "ساعات العمل غير صالحة.");

        Name = name;
        Address = address;
        WorkingHours = workingHours;
    }
}