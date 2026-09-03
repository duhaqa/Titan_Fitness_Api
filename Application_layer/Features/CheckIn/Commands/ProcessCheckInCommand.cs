using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.CheckIn.Commands
{
    // Command Definition
    public record ProcessCheckInCommand(ProcessCheckInDto CheckInDto) : IRequest<bool>;

    // Handler Implementation
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
            {
                throw new ArgumentNullException(nameof(request.CheckInDto), "بيانات تسجيل الدخول مطلوبة.");
            }

            var dto = request.CheckInDto;

            // 1. Validate Member existence
            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
            {
                throw new KeyNotFoundException($"العضو برقم {dto.MemberId} غير موجود.");
            }

            // 2. Create Admitted CheckIn Entity
            var checkIn = Domain.Entites.CheckIn.CreateAdmitted(dto.MemberId, dto.BranchId);

            // 3. Persist CheckIn via Repository and UnitOfWork
            await _unitOfWork.CheckIns.AddAsync(checkIn, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}