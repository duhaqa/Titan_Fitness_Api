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

namespace Titan_Fitness.Application_layer.Features.Classes.Commands
{
    public record CreateClassSessionCommand(CreateClassSessionDto SessionDto) : IRequest<int>;

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
                throw new ArgumentNullException(nameof(request.SessionDto), "بيانات الجلسة مطلوبة.");

            var dto = request.SessionDto;

            var studio = await _unitOfWork.Studios.GetByIdAsync(dto.StudioId, cancellationToken);
            if (studio == null)
                throw new KeyNotFoundException($"الاستوديو برقم {dto.StudioId} غير موجود.");

            if (dto.CapacityLimit > studio.Capacity)
                throw new ArgumentException("سعة الحصة لا يمكن أن تتجاوز سعة الاستوديو.");

            var trainer = await _unitOfWork.Trainers.GetByIdAsync(dto.TrainerId, cancellationToken);
            if (trainer == null || !trainer.IsActive)
                throw new InvalidOperationException("المدرب غير موجود أو غير نشط، ولا يمكن جدولته.");

            var sessionDate = DateOnly.FromDateTime(dto.Date);
            var startTime = dto.StartTime;
            var endTime = startTime.Add(TimeSpan.FromMinutes(dto.DurationInMinutes));

            var sessionsOnDate = (await _unitOfWork.ClassSessions.GetAllAsync(cancellationToken))
                .Where(s => s.SessionDate == sessionDate && s.Status != SessionStatus.Cancelled)
                .ToList();

            var trainerConflict = sessionsOnDate.Any(s =>
                s.TrainerId == dto.TrainerId &&
                s.StartTime < endTime &&
                startTime < s.StartTime.Add(TimeSpan.FromMinutes(s.DurationInMinutes)));

            if (trainerConflict)
                throw new InvalidOperationException("المدرب لديه حصة أخرى تتداخل مع هذا الموعد.");

            var studioConflict = sessionsOnDate.Any(s =>
                s.StudioId == dto.StudioId &&
                s.StartTime < endTime &&
                startTime < s.StartTime.Add(TimeSpan.FromMinutes(s.DurationInMinutes)));

            if (studioConflict)
                throw new InvalidOperationException("الاستوديو محجوز لحصة أخرى تتداخل مع هذا الموعد.");

            var session = ClassSession.Create(
                className: dto.ClassName,
                branchId: dto.BranchId,
                studioId: dto.StudioId,
                trainerId: dto.TrainerId,
                sessionDate: sessionDate,
                startTime: startTime,
                durationInMinutes: dto.DurationInMinutes,
                capacityLimit: dto.CapacityLimit,
                description: dto.Description
            );

            await _unitOfWork.ClassSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}