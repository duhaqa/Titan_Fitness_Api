using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    // Command definition returning bool
    public record UpdateMemberCommand(UpdateMemberDto MemberDto) : IRequest<bool>;

    // Handler implementation within the same Vertical Slice file
    public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
        {
            // 1. Guard against null request payload
            if (request?.MemberDto == null)
            {
                throw new ArgumentNullException(nameof(request.MemberDto), "Update member payload cannot be null.");
            }

            var dto = request.MemberDto;

            // 2. Validate Member ID Existence & Input
            if (dto.Id <= 0)
            {
                throw new ArgumentException("A valid Member ID must be provided for update.");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException("Full Name cannot be empty.");
            }

            // 3. IDOR Protection: Check if member exists
            var member = await _unitOfWork.Members.GetByIdAsync(dto.Id, cancellationToken);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {dto.Id} was not found.");
            }

            // 4. Instantiate Value Objects using Factory Methods
            var phoneStr = string.IsNullOrWhiteSpace(dto.Phone) ? string.Empty : dto.Phone.Trim();
            var addressStr = string.IsNullOrWhiteSpace(dto.Address) ? string.Empty : dto.Address.Trim();

            Phone phone = Phone.Create(phoneStr);
            Address address = Address.Create(addressStr);

            // 5. Update Domain Entity state using encapsulation method UpdateProfile
            // Preserving existing PhotoPath from the entity
            member.UpdateProfile(
                fullName: dto.FullName.Trim(),
                email: string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLower(),
                phone: phone,
                address: address,
                photoPath: member.PhotoPath
            );

            // 6. Persist changes atomically
            _unitOfWork.Members.Update(member);
            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}