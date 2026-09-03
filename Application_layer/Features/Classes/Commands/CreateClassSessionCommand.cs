using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Classes.Commands
{
    // Command Definition
    public record CreateClassSessionCommand(CreateClassSessionDto SessionDto) : IRequest<int>;

    // Handler Implementation
    public class CreateClassSessionCommandHandler : IRequestHandler<CreateClassSessionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateClassSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<int> Handle(CreateClassSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.SessionDto == null)
            {
                throw new ArgumentNullException(nameof(request.SessionDto), "بيانات الجلسة مطلوبة.");
            }

            var dto = request.SessionDto;

            // 1. Create ClassSession Entity using domain factory method
            var session = ClassSession.Create(
                className: dto.ClassName,
                branchId: dto.BranchId,
                studioId: dto.StudioId,
                trainerId: dto.TrainerId,
                sessionDate: DateOnly.FromDateTime(dto.Date),
                startTime: dto.StartTime,
                durationInMinutes: dto.DurationInMinutes,
                capacityLimit: dto.CapacityLimit,
                description: dto.Description
            );

            // 2. Add via Generic Repository and persist
            await _unitOfWork.ClassSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Return created session ID
            return session.Id;
        }
    }
}