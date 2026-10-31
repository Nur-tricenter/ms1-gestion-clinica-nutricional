using Joseco.DDD.Core.Results;
using MediatR;

namespace GestionClinicaNutricional.Application.Paciente
{
    /// <summary>
    /// Contact data MS5 needs to reach a patient at delivery time. Patients that do not exist are
    /// omitted from the answer rather than failing the whole request.
    /// </summary>
    public class GetContactInformationQuery : IRequest<Result<IReadOnlyList<ContactInformation>>>
    {
        public IReadOnlyList<Guid> PatientIds { get; init; } = [];
    }

    public record ContactInformation(Guid PatientId, string? PhoneNumber, string FullName);
}
