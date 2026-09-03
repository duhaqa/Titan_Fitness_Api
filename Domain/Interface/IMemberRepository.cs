using System.Threading;
using System.Threading.Tasks;
using Titan_Fitness.Domain.Entites;

namespace Titan_Fitness.Domain.Interfaces;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<bool> IsMembershipNumberExistsAsync(string membershipNumber, CancellationToken cancellationToken = default);
}