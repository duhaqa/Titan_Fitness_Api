using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Titan_Fitness.Data.DB; // تأكد من استيراد ה-Namespace الخاص بالـ DB_context
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Data.REPO;

public class UnitOfWork : IUnitOfWork
{
    private readonly DB_context _context; // B كبيرة

    public IMemberRepository Members { get; }
    public IGenericRepository<Plan> Plans { get; }
    public IGenericRepository<Membership> Memberships { get; }
    public IGenericRepository<Branch> Branches { get; }
    public IGenericRepository<Trainer> Trainers { get; }
    public IGenericRepository<ClassSession> ClassSessions { get; }
    public IGenericRepository<Booking> Bookings { get; }
    public IGenericRepository<CheckIn> CheckIns { get; }
    public IGenericRepository<Freeze> Freezes { get; }

    public UnitOfWork(DB_context context) // B كبيرة
    {
        _context = context;
        Members = new MemberRepository(_context);
        Plans = new GenericRepository<Plan>(_context);
        Memberships = new GenericRepository<Membership>(_context);
        Branches = new GenericRepository<Branch>(_context);
        Trainers = new GenericRepository<Trainer>(_context);
        ClassSessions = new GenericRepository<ClassSession>(_context);
        Bookings = new GenericRepository<Booking>(_context);
        CheckIns = new GenericRepository<CheckIn>(_context);
        Freezes = new GenericRepository<Freeze>(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}