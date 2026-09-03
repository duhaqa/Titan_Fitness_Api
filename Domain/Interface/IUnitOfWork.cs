using System.Threading;
using System.Threading.Tasks;
using Titan_Fitness.Domain.Entites;

namespace Titan_Fitness.Domain.Interfaces;

public interface IUnitOfWork
{
    IMemberRepository Members { get; }
    IGenericRepository<Plan> Plans { get; }
    IGenericRepository<Membership> Memberships { get; }
    IGenericRepository<Branch> Branches { get; }
    IGenericRepository<Studio> Studios { get; }
    IGenericRepository<Trainer> Trainers { get; }
    IGenericRepository<ClassSession> ClassSessions { get; }
    IGenericRepository<Booking> Bookings { get; }
    IGenericRepository<CheckIn> CheckIns { get; }
    IGenericRepository<Freeze> Freezes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}