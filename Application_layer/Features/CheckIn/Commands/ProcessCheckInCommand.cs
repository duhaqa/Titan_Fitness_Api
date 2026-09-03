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

namespace Titan_Fitness.Application_layer.Features.CheckIn.Commands
{
    public record ProcessCheckInCommand(ProcessCheckInDto CheckInDto) : IRequest<bool>;

    public class ProcessCheckInCommandHandler : IRequestHandler<ProcessCheckInCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProcessCheckInCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> Handle(ProcessCheckInCommand request, CancellationToken cancellationToken)
        {
            if (request.CheckInDto == null)
                throw new ArgumentNullException(nameof(request.CheckInDto), "بيانات تسجيل الدخول مطلوبة.");

            var dto = request.CheckInDto;

            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
                throw new KeyNotFoundException($"العضو برقم {dto.MemberId} غير موجود.");

            var memberships = await _unitOfWork.Memberships.GetAllAsync(cancellationToken);
            var membership = memberships
                .Where(m => m.MemberId == dto.MemberId && m.Status != MembershipStatus.Cancelled)
                .OrderByDescending(m => m.StartDate)
                .FirstOrDefault();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Entry is granted only when all four hold: the membership has
            // started, has not ended, is not frozen, and its access scope
            // reaches this branch. A refusal names the one that failed.
            string? refusalReason = membership switch
            {
                null => "لا يوجد اشتراك مسجل لهذا العضو.",
                _ when today < membership.StartDate => "الاشتراك لم يبدأ بعد.",
                _ when today > membership.EndDate || membership.Status == MembershipStatus.Expired => "الاشتراك منتهي.",
                _ when membership.Status == MembershipStatus.Frozen => "الاشتراك مجمّد حالياً.",
                _ when membership.AgreedTerms.AccessScope == AccessScope.HomeBranchOnly
                       && member.HomeBranchId != dto.BranchId => "الاشتراك لا يغطي هذا الفرع.",
                _ => null
            };

            var checkIn = refusalReason == null
                ? Domain.Entites.CheckIn.CreateAdmitted(dto.MemberId, dto.BranchId)
                : Domain.Entites.CheckIn.CreateRefused(dto.MemberId, dto.BranchId, refusalReason);

            await _unitOfWork.CheckIns.AddAsync(checkIn, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return refusalReason == null;
        }
    }
}