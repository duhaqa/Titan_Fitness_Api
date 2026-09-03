namespace Titan_Fitness.Domain.Entites;

using System;
using Titan_Fitness.Domain.Value_object;

public class Member
{
    public int Id { get; private set; } // Member Id        
    public string MembershipNumber { get; private set; } = null!; // max 10 char, required, unique        
    public string FullName { get; private set; } = null!; // max 100 char, required        
    public string? Email { get; private set; } // max 100 char        
    public Phone Phone { get; private set; } = null!; // max 20 char (Value Object)        
    public Address Address { get; private set; } = null!; // max 200 char (Value Object)        
    public DateTime JoinedDate { get; private set; } // date, required        
    public string? PhotoPath { get; private set; } // Photo        
    public int HomeBranchId { get; private set; } // Home branch Id        

    private Member() { }

    private Member(
        string membershipNumber,
        string fullName,
        string? email,
        Phone phone,
        Address address,
        int homeBranchId,
        string? photoPath)
    {
        MembershipNumber = membershipNumber;
        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        JoinedDate = DateTime.UtcNow;
        HomeBranchId = homeBranchId;
        PhotoPath = photoPath;
    }

    public static Member Create(
        string membershipNumber,
        string fullName,
        string? email,
        Phone phone,
        Address address,
        int homeBranchId,
        string? photoPath = null)
    {
        if (string.IsNullOrWhiteSpace(membershipNumber) || membershipNumber.Length > 10)
            throw new ArgumentException("رقم العضوية مطلوب ويجب ألا يتجاوز 10 أحرف.");         

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 100)
            throw new ArgumentException("اسم العضو مطلوب ويجب ألا يتجاوز 100 حرف.");         

        if (email?.Length > 100)
            throw new ArgumentException("البريد الإلكتروني يجب ألا يتجاوز 100 حرف.");         

        if (homeBranchId <= 0)
            throw new ArgumentException("معرف الفرع الرئيسي غير صالح.");

        return new Member(membershipNumber, fullName, email, phone, address, homeBranchId, photoPath);
    }

    public void UpdateProfile(string fullName, string? email, Phone phone, Address address, string? photoPath)
    {
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 100)
            throw new ArgumentException("اسم العضو غير صالح.");         

        if (email?.Length > 100)
            throw new ArgumentException("البريد الإلكتروني غير صالح.");         

        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        PhotoPath = photoPath;
    }
}