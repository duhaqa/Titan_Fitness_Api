using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Data.REPO;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(DbContext context) : base(context)
    {
    }

    public async Task<bool> IsMembershipNumberExistsAsync(string membershipNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(m => m.MembershipNumber == membershipNumber, cancellationToken);
    }
}