using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;


namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    // Command definition returning bool
    public record FreezeMembershipCommand(FreezeMembershipDto FreezeDto) : IRequest<bool>;

    // Handler implementation within the same feature file
    public class FreezeMembershipCommandHandler : IRequestHandler<FreezeMembershipCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FreezeMembershipCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(FreezeMembershipCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate request payload existence
            if (request?.FreezeDto == null)
            {
                throw new ArgumentNullException(nameof(request.FreezeDto), "Freeze request payload cannot be null.");
            }

            var dto = request.FreezeDto;

            // 2. Validate essential identifiers & constraints
            if (dto.MemberId <= 0)
            {
                throw new ArgumentException("A valid Member ID must be provided.");
            }

            if (dto.DurationInMonths <= 0 || dto.DurationInMonths > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(dto.DurationInMonths), "Freeze duration must be between 1 and 12 months.");
            }

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                throw new ArgumentException("Reason for freezing membership is required.");
            }

            // 3. IDOR Protection: Verify Member existence
            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {dto.MemberId} was not found.");
            }

            // 4. Calculate freeze end date based on DurationInMonths
            DateTime calculatedStartDate = dto.StartDate == default ? DateTime.UtcNow.Date : dto.StartDate.Date;
            DateTime calculatedEndDate = calculatedStartDate.AddMonths(dto.DurationInMonths);

            // 5. Atomic state persistence
            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}