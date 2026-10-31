using GestionClinicaNutricional.Domain.Repositories;
using Joseco.DDD.Core.Results;
using MediatR;

namespace GestionClinicaNutricional.Application.Paciente
{
    public class GetPacienteByIdHandler(IPacienteRepository pacienteRepository)
        : IRequestHandler<GetPacienteByIdQuery, Result<Domain.Paciente>>
    {
        public async Task<Result<Domain.Paciente>> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await pacienteRepository.GetByIdAsync(request.Id, true);
            }
            catch (NullReferenceException)
            {
                // The repository throws when the patient does not exist; other services (MS3) rely on a 404, not a 500.
                return Result.Failure<Domain.Paciente>(new Error("404", "Paciente no encontrado.", ErrorType.NotFound));
            }
        }
    }
}
