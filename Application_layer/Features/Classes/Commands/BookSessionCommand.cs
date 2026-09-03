using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Classes.Commands
{
    // Command Definition
    public record BookSessionCommand(BookSessionDto BookingDto) : IRequest;

    // Handler Implementation
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
            {
                throw new ArgumentNullException(nameof(request.BookingDto), "بيانات الحجز مطلوبة.");
            }

            var dto = request.BookingDto;

            // 1. Validate Session existence
            var session = await _unitOfWork.ClassSessions.GetByIdAsync(dto.SessionId, cancellationToken);
            if (session == null)
            {
                throw new KeyNotFoundException($"الحصة المطلوب حجزها برقم {dto.SessionId} غير موجودة.");
            }

            // 2. Validate Member existence
            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
            {
                throw new KeyNotFoundException($"العضو برقم {dto.MemberId} غير موجود.");
            }

            // 3. Create Booking entity using Factory Method matching Booking.cs
            var booking = Booking.CreateBooked(
                sessionId: dto.SessionId,
                memberId: dto.MemberId,
                notesForTrainer: dto.SpecialRequirements
            );

            // 4. Save via Repository and UnitOfWork
            await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}