namespace Titan_Fitness.Domain.Entites;

using System;
using Titan_Fitness.Domain.Enums;

public class Booking
{
    public int Id { get; private set; } // Booking Id
    public int SessionId { get; private set; } // Session Id
    public int MemberId { get; private set; } // Member Id
    public DateTime BookedOn { get; private set; } // DateTime, required
    public BookingStatus Status { get; private set; } // Enum
    public int? WaitlistPosition { get; private set; } // nullable
    public string? NotesForTrainer { get; private set; } // max 500 char

    private Booking() { }

    public static Booking CreateBooked(int sessionId, int memberId, string? notesForTrainer)
    {
        ValidateIds(sessionId, memberId);
        ValidateNotes(notesForTrainer);

        return new Booking
        {
            SessionId = sessionId,
            MemberId = memberId,
            BookedOn = DateTime.UtcNow,
            Status = BookingStatus.Booked,
            WaitlistPosition = null,
            NotesForTrainer = notesForTrainer
        };
    }

    public static Booking CreateWaitlisted(int sessionId, int memberId, int waitlistPosition, string? notesForTrainer)
    {
        ValidateIds(sessionId, memberId);
        ValidateNotes(notesForTrainer);

        if (waitlistPosition <= 0)
            throw new ArgumentException("ترتيب قائمة الانتظار يجب أن يكون رقمًا موجبًا.");

        return new Booking
        {
            SessionId = sessionId,
            MemberId = memberId,
            BookedOn = DateTime.UtcNow,
            Status = BookingStatus.Waitlisted,
            WaitlistPosition = waitlistPosition,
            NotesForTrainer = notesForTrainer
        };
    }

    public void MarkAsAttended() => Status = BookingStatus.Attended;
    public void MarkAsNoShow() => Status = BookingStatus.NoShow;
    public void Cancel() => Status = BookingStatus.Cancelled;

    private static void ValidateIds(int sessionId, int memberId)
    {
        if (sessionId <= 0 || memberId <= 0)
            throw new ArgumentException("معرف الحصة والعضو يجب أن تكون أرقاماً صالحة.");
    }

    private static void ValidateNotes(string? notes)
    {
        if (notes?.Length > 500)
            throw new ArgumentException("ملاحظات المدرب يجب ألا تتجاوز 500 حرف.");
    }
}