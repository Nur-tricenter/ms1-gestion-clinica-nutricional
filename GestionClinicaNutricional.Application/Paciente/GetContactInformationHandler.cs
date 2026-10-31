using GestionClinicaNutricional.Domain.Repositories;
using Joseco.DDD.Core.Results;
using MediatR;

namespace GestionClinicaNutricional.Application.Paciente
{
    internal class GetContactInformationHandler(IPacienteRepository pacienteRepository)
        : IRequestHandler<GetContactInformationQuery, Result<IReadOnlyList<ContactInformation>>>
    {
        public async Task<Result<IReadOnlyList<ContactInformation>>> Handle(
            GetContactInformationQuery request, CancellationToken cancellationToken)
        {
            var pacientes = await pacienteRepository.GetByIdsAsync(request.PatientIds, cancellationToken);

            IReadOnlyList<ContactInformation> contacts = pacientes
                .Select(p => new ContactInformation(p.Id, p.Telefono, $"{p.Nombre} {p.Apellido}"))
                .ToList();

            return Result<IReadOnlyList<ContactInformation>>.Success(contacts);
        }
    }
}
