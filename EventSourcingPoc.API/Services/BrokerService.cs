using EventSourcingPoc.API.EFContext;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingPoc.API.Services
{
    public class BrokerInfo
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public interface IBrokerService
    {
        Task<BrokerInfo?> GetBrokerInfoAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<BrokerInfo>> GetBrokerInfoAsync(CancellationToken cancellationToken);
    }

    public class BrokerService(GuaranteeContext context) : IBrokerService
    {
        private readonly GuaranteeContext _context = context;
        public async Task<BrokerInfo?> GetBrokerInfoAsync(int id, CancellationToken cancellationToken)
        {
            var broker = await _context.Brokers.AsNoTracking()
                .Select(b => new BrokerInfo
                {
                    Id = b.Id,
                    Name = b.Name
                }).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            return broker;
        }

        public async Task<IReadOnlyList<BrokerInfo>> GetBrokerInfoAsync(CancellationToken cancellationToken)
        {
            var brokers = await _context.Brokers
                .AsNoTracking()
                .Select(b => new BrokerInfo
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToListAsync(cancellationToken);
            return brokers;
        }
    }
}
