using System.Transactions;
using Joseco.DDD.Core.Abstractions;

namespace GestionClinicaNutricional.Domain.Repositories
{
    public interface IPacienteRepository : IRepository<Paciente>
    {
        Task UpdateAsync(Paciente paciente);

        /// <summary>Read-only lookup of several patients at once; ids that do not exist are skipped.</summary>
        Task<IReadOnlyList<Paciente>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    }
}