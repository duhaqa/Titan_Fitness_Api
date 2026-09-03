using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Enums;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Classes.Commands
{
    public record BookSessionCommand(BookSessionDto BookingDto) : IRequest;

    public class BookSessionCommandHandler : IRequestHandler<BookSessionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(BookSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.BookingDto == null)
                throw new ArgumentNullException(nameof(request.BookingDto), "بيانات الحجز مطلوبة.");

            var dto = request.BookingDto;

            var session = await _unitOfWork.ClassSessions.GetByIdAsync(dto.SessionId, cancellationToken);
            if (session == null)
                throw new KeyNotFoundException($"الحصة المطلوب حجزها برقم {dto.SessionId} غير موجودة.");

            if (session.Status != SessionStatus.Open)
                throw new InvalidOperationException("لا يمكن الحجز في حصة غير مفتوحة للحجز.");

            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
                throw new KeyNotFoundException($"العضو برقم {dto.MemberId} غير موجود.");

            var memberships = await _unitOfWork.Memberships.GetAllAsync(cancellationToken);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var hasActiveMembership = memberships.Any(m =>
                m.MemberId == dto.MemberId &&
                m.Status == MembershipStatus.Active &&
                m.StartDate <= today && m.EndDate >= today);

            if (!hasActiveMembership)
                throw new InvalidOperationException("لا يمكن حجز عضو ليس لديه اشتراك فعّال.");

            var allBookings = (await _unitOfWork.Bookings.GetAllAsync(cancellationToken)).ToList();

            var alreadyBooked = allBookings.Any(b =>
                b.SessionId == dto.SessionId &&
                b.MemberId == dto.MemberId &&
                (b.Status == BookingStatus.Booked || b.Status == BookingStatus.Waitlisted));

            if (alreadyBooked)
                throw new InvalidOperationException("العضو محجوز مسبقاً على هذه الحصة.");

            var sessionEnd = session.StartTime.Add(TimeSpan.FromMinutes(session.DurationInMinutes));
            var memberActiveSessionIds = allBookings
                .Where(b => b.MemberId == dto.MemberId &&
                            (b.Status == BookingStatus.Booked || b.Status == BookingStatus.Waitlisted))
                .Select(b => b.SessionId)
                .ToHashSet();

            if (memberActiveSessionIds.Count > 0)
            {
                var allSessions = await _unitOfWork.ClassSessions.GetAllAsync(cancellationToken);
                var overlaps = allSessions.Any(s =>
                    memberActiveSessionIds.Contains(s.Id) &&
                    s.SessionDate == session.SessionDate &&
                    s.StartTime < sessionEnd &&
                    session.StartTime < s.StartTime.Add(TimeSpan.FromMinutes(s.DurationInMinutes)));

                if (overlaps)
                    throw new InvalidOperationException("العضو محجوز على حصة أخرى تتداخل مع هذا الموعد.");
            }

            var bookedCount = allBookings.Count(b => b.SessionId == dto.SessionId && b.Status == BookingStatus.Booked);

            Booking booking;
            if (bookedCount < session.CapacityLimit)
            {
                booking = Booking.CreateBooked(dto.SessionId, dto.MemberId, dto.SpecialRequirements);
            }
            else
            {
                var waitlistPosition = allBookings.Count(b => b.SessionId == dto.SessionId && b.Status == BookingStatus.Waitlisted) + 1;
                booking = Booking.CreateWaitlisted(dto.SessionId, dto.MemberId, waitlistPosition, dto.SpecialRequirements);
            }

            await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}