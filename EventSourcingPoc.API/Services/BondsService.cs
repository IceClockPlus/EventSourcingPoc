using EventSourcingPoc.API.EFContext;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingPoc.API.Services
{
    public class BondData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? LegacyId { get; set; }
    }

    public interface IBondsService
    {
        Task<IReadOnlyList<BondData>> GetAllBonds(CancellationToken cancellationToken);
        Task<BondData?> GetById(int id, CancellationToken cancellationToken);
        Task<BondData?> GetByLegacyId(int legacyId, CancellationToken cancellationToken);
    }

    public class BondsService : IBondsService
    {
        private readonly GuaranteeContext _context;
        public BondsService(GuaranteeContext guaranteeContext)
        {
            _context = guaranteeContext;
        }
        public async Task<IReadOnlyList<BondData>> GetAllBonds(CancellationToken cancellationToken)
        {
            var bonds = await _context.Bonds.AsNoTracking().Select(b => new BondData
            {
                Id = b.Id,
                Name = b.Name,
                LegacyId = b.LegacyId
            }).ToListAsync(cancellationToken);
            return bonds;

        }

        public async Task<BondData?> GetById(int id, CancellationToken cancellationToken)
        {
            var bond = await _context.Bonds.AsNoTracking()
                .Select(b => new BondData
                {
                    Id = b.Id,
                    Name = b.Name,
                    LegacyId = b.LegacyId
                }).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            return bond;
        }

        public async Task<BondData?> GetByLegacyId(int legacyId, CancellationToken cancellationToken)
        {
            var bond = await _context.Bonds.AsNoTracking()
            .Select(b => new BondData
            {
                Id = b.Id,
                Name = b.Name,
                LegacyId = b.LegacyId
            }).FirstOrDefaultAsync(b => b.LegacyId == legacyId, cancellationToken);
            return bond;
        }
    }
}
