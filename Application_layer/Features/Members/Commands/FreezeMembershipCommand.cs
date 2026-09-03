using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Enums;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    public record FreezeMembershipCommand(FreezeMembershipDto FreezeDto) : IRequest<bool>;

    public class FreezeMembershipCommandHandler : IRequestHandler<FreezeMembershipCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FreezeMembershipCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(FreezeMembershipCommand request, CancellationToken cancellationToken)
        {
            if (request?.FreezeDto == null)
                throw new ArgumentNullException(nameof(request.FreezeDto), "Freeze request payload cannot be null.");

            var dto = request.FreezeDto;

            if (dto.MemberId <= 0)
                throw new ArgumentException("A valid Member ID must be provided.");

            if (dto.DurationInMonths <= 0 || dto.DurationInMonths > 12)
                throw new ArgumentOutOfRangeException(nameof(dto.DurationInMonths), "Freeze duration must be between 1 and 12 months.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                throw new ArgumentException("Reason for freezing membership is required.");

            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {dto.MemberId} was not found.");

            // Locate the member's currently active membership — only an active
            // membership can be frozen.
            var memberships = await _unitOfWork.Memberships.GetAllAsync(cancellationToken);
            var membership = memberships
                .Where(m => m.MemberId == dto.MemberId && m.Status == MembershipStatus.Active)
                .OrderByDescending(m => m.StartDate)
                .FirstOrDefault();

            if (membership == null)
                throw new InvalidOperationException("This member has no active membership that can be frozen.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var startDate = dto.StartDate == default ? today : DateOnly.FromDateTime(dto.StartDate);

            if (startDate < today)
                throw new ArgumentException("A freeze cannot begin in the past.");

            var endDate = startDate.AddMonths(dto.DurationInMonths);

            if (endDate > membership.EndDate)
                throw new ArgumentException("A freeze cannot run past the end of the membership.");

            // Enforce the agreed terms' freeze allowance (fixed at purchase, not
            // whatever the plan currently offers).
            var freezes = await _unitOfWork.Freezes.GetAllAsync(cancellationToken);
            var existingFreezes = freezes.Where(f => f.MembershipId == membership.Id).ToList();

            var maxFreezes = membership.AgreedTerms.MaxNumberOfFreezes ?? 0;
            if (existingFreezes.Count >= maxFreezes)
                throw new InvalidOperationException("This membership has used all the freezes its agreed terms allow.");

            var freezeDays = endDate.DayNumber - startDate.DayNumber;
            var usedFreezeDays = existingFreezes.Sum(f => f.EndDate.DayNumber - f.StartDate.DayNumber);
            var maxFreezeDays = membership.AgreedTerms.MaxFreezeDays ?? 0;

            if (usedFreezeDays + freezeDays > maxFreezeDays)
                throw new InvalidOperationException("This freeze would exceed the maximum freeze days allowed by the agreed terms.");

            var reason = ParseReason(dto.Reason);

            var freeze = Freeze.Create(membership.Id, startDate, endDate, dto.DurationInMonths, reason, dto.AdditionalNotes);
            await _unitOfWork.Freezes.AddAsync(freeze, cancellationToken);

            // The end date moves forward by exactly the days spent frozen, so
            // nothing is lost — projected up front, as the screen shows it
            // before the freeze is confirmed.
            membership.ExtendEndDate(freezeDays);
            membership.ChangeStatus(MembershipStatus.Frozen);
            _unitOfWork.Memberships.Update(membership);

            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }

        private static FreezeReason ParseReason(string reason) => reason.Trim().ToLowerInvariant() switch
        {
            "extended travel" => FreezeReason.ExtendedTravel,
            "injury" => FreezeReason.Injury,
            _ => FreezeReason.Other
        };
    }
}