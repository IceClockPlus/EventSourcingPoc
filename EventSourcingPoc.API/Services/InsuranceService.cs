using EventSourcingPoc.API.EFContext;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingPoc.API.Services
{
    public class InsuranceInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? LegacyId { get; set; }
        public long CurrentNumberCounter { get; set; }
    }

    public interface IInsuranceService
    {
        Task<InsuranceInfo?> GetInsurance(int id, CancellationToken cancellationToken);
        Task<InsuranceInfo?> GetInsuranceByLegacyId(int legacyId, CancellationToken cancellationToken);
        Task<long> GenerateNewCertificateNumber(int id, CancellationToken cancellationToken);
    }

    public class InsuranceService(GuaranteeContext context) : IInsuranceService
    {
        private readonly GuaranteeContext _context = context;
        public async Task<InsuranceInfo?> GetInsurance(int id, CancellationToken cancellationToken)
        {
            var insurance = await _context.Insurances
                .Select(i => new InsuranceInfo
                {
                    Id = i.Id,
                    Name = i.Name,
                    LegacyId = i.LegacyId,
                    CurrentNumberCounter = i.CertificateNumberCounter
                }).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            return insurance;            
        }

        public async Task<InsuranceInfo?> GetInsuranceByLegacyId(int legacyId, CancellationToken cancellationToken)
        {
            var insurance = await _context.Insurances
                .Select(i => new InsuranceInfo
                {
                    Id = i.Id,
                    Name = i.Name,
                    LegacyId = i.LegacyId,
                    CurrentNumberCounter = i.CertificateNumberCounter
                }).FirstOrDefaultAsync(i => i.LegacyId == legacyId, cancellationToken);
            return insurance;

        }

        /// <summary>
        /// Increment and get the guarantee certificate number correlative for a specific insurance
        /// </summary>
        /// <param name="id">Insurance Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>New certificate number correlative</returns>
        /// <remarks>
        /// KEEP THIS IMPLEMENTATION
        /// Make use of 'UPDATE .. RETURNING' to ensure atomicity of the operation.
        /// The database locks the row, increment and return the 'certificate_number_counter' in a single indivisible step.
        /// DOT NOT REFACTORING to either EF CORE (Read -> Update -> Save) nor 'ExecuteUpdateAsync' with a subsequent query,
        /// since both will leave a milliseconds gap prone to race conditions on high concurrent environment
        /// </remarks>
        public async Task<long> GenerateNewCertificateNumber(int id, CancellationToken cancellationToken)
        {
            var certificateNumber = await _context.Database
                .SqlQueryRaw<long>(
                    @"UPDATE insurances
                    SET certificate_number_counter = certificate_number_counter + 1
                    WHERE id = {0}
                    RETURNING certificate_number_counter",
                    id
                ).ToListAsync(cancellationToken);

            return certificateNumber.Single();
        }
    }
}
