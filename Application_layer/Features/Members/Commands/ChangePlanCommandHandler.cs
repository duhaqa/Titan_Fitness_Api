using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Enums;
using Titan_Fitness.Domain.Interfaces;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    public record ChangePlanCommand(ChangePlanDto ChangePlanDto) : IRequest<bool>;

    public class ChangePlanCommandHandler : IRequestHandler<ChangePlanCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ChangePlanCommand request, CancellationToken cancellationToken)
        {
            if (request?.ChangePlanDto == null)
                throw new ArgumentNullException(nameof(request.ChangePlanDto), "Request payload cannot be null.");

            var dto = request.ChangePlanDto;

            if (dto.MemberId <= 0 || dto.NewPlanId <= 0)
                throw new ArgumentException("Invalid Member ID or Plan ID specified.");

            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
                throw new KeyNotFoundException($"Member with ID {dto.MemberId} was not found.");

            var newPlan = await _unitOfWork.Plans.GetByIdAsync(dto.NewPlanId, cancellationToken);
            if (newPlan == null || !newPlan.IsPublished)
                throw new InvalidOperationException("Selected plan is inactive or unavailable.");

            // Switching plans creates a new membership rather than editing the
            // current one — the current membership's own terms are never touched.
            var memberships = await _unitOfWork.Memberships.GetAllAsync(cancellationToken);
            var currentMembership = memberships
                .Where(m => m.MemberId == dto.MemberId && m.Status != MembershipStatus.Cancelled)
                .OrderByDescending(m => m.StartDate)
                .FirstOrDefault();

            if (currentMembership == null)
                throw new InvalidOperationException("This member has no membership to change the plan of.");

            if (currentMembership.Status == MembershipStatus.Cancelled)
                throw new InvalidOperationException("A cancelled membership cannot be moved onto a different plan.");

            var newStartDate = dto.Immediately
                ? DateOnly.FromDateTime(DateTime.UtcNow)
                : currentMembership.EndDate; // at renewal: no day is ever covered twice

            var agreedTerms = AgreedTerms.Create(
                newPlan.Price,
                newPlan.DurationInMonths,
                newPlan.MaxFreezeDays,
                newPlan.MaxNumberOfFreezes,
                newPlan.GuestPassQuota,
                newPlan.AccessScope);

            var newMembership = Membership.Create(dto.MemberId, dto.NewPlanId, newStartDate, agreedTerms);
            await _unitOfWork.Memberships.AddAsync(newMembership, cancellationToken);

            if (dto.Immediately)
            {
                currentMembership.ChangeStatus(MembershipStatus.Cancelled);
                _unitOfWork.Memberships.Update(currentMembership);
            }

            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}