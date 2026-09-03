using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Application_layer.Features.Trainers.Commands
{
    // Command Definition
    public record CreateTrainerCommand(CreateTrainerDto TrainerDto) : IRequest<int>;

    // Handler Implementation
    public class CreateTrainerCommandHandler : IRequestHandler<CreateTrainerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTrainerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<int> Handle(CreateTrainerCommand request, CancellationToken cancellationToken)
        {
            if (request.TrainerDto == null)
            {
                throw new ArgumentNullException(nameof(request.TrainerDto), "بيانات المدرب مطلوبة.");
            }

            var dto = request.TrainerDto;

            // 1. Create Phone Value Object safely
            var phone = Phone.Create(dto.Phone ?? string.Empty);

            // 2. Map DTO FullName to Entity Name parameter
            var trainer = Trainer.Create(
                name: dto.FullName,
                email: dto.Email,
                phone: phone,
                isActive: dto.IsActive
            );

            // 3. Save via UnitOfWork
            await _unitOfWork.Trainers.AddAsync(trainer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return trainer.Id;
        }
    }
}