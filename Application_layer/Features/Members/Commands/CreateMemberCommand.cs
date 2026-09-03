using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    // Command definition
    public record CreateMemberCommand(CreateMemberDto MemberDto) : IRequest<int>;

    // Handler implementation within the same Vertical Slice file
    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            // 1. Guard against null request payload
            if (request?.MemberDto == null)
            {
                throw new ArgumentNullException(nameof(request.MemberDto), "Member details payload cannot be null.");
            }

            var dto = request.MemberDto;

            // 2. Validate input properties
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException("Full Name is required.");
            }

            if (dto.HomeBranchId <= 0)
            {
                throw new ArgumentException("A valid Home Branch ID must be assigned.");
            }

            // 3. Auto-generate Unique Membership Number if left empty (e.g., TF-8932)
            string membershipNumber = string.IsNullOrWhiteSpace(dto.MembershipNumber)
                ? $"TF-{Random.Shared.Next(1000, 9999)}"
                : dto.MembershipNumber.Trim().ToUpper();

            // 4. Ensure uniqueness of the membership number
            bool isNumberExists = await _unitOfWork.Members.IsMembershipNumberExistsAsync(membershipNumber, cancellationToken);
            if (isNumberExists)
            {
                throw new InvalidOperationException($"Membership number '{membershipNumber}' is already in use.");
            }

            // 5. Instantiate Value Objects safely via Factory Method Create(...)
            var phoneStr = string.IsNullOrWhiteSpace(dto.Phone) ? string.Empty : dto.Phone.Trim();
            var addressStr = string.IsNullOrWhiteSpace(dto.Address) ? string.Empty : dto.Address.Trim();

            Phone phone = Phone.Create(phoneStr);
            Address address = Address.Create(addressStr);

            // 6. Instantiate Domain Entity via static Factory Method Member.Create(...)
            Member memberEntity = Member.Create(
                membershipNumber: membershipNumber,
                fullName: dto.FullName.Trim(),
                email: string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLower(),
                phone: phone,
                address: address,
                homeBranchId: dto.HomeBranchId,
                photoPath: null
            );

            // 7. Pass the Domain Entity (Member) directly to Repository
            await _unitOfWork.Members.AddAsync(memberEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return memberEntity.Id;
        }
    }
}