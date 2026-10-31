using GestionClinicaNutricional.Application.Paciente;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionClinicaNutricionalService.WebApi.Controllers
{
    /// <summary>
    /// The contract MS5 (logistics) uses to get a patient's phone and full name. Path and body are fixed
    /// by that consumer: <c>POST /api/v1/patients/contact-information</c> with an array of patient ids.
    /// </summary>
    [ApiController]
    [Route("api/v1/patients")]
    public class PatientsContactController(IMediator mediator) : ControllerBase
    {
        [HttpPost("contact-information")]
        public async Task<IActionResult> ContactInformation([FromBody] Guid[] patientIds)
        {
            var result = await mediator.Send(new GetContactInformationQuery { PatientIds = patientIds });

            return Ok(result.Value);
        }
    }
}
