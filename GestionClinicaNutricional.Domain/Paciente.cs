using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Joseco.DDD.Core.Abstractions;
using GestionClinicaNutricional.Domain.Events;

namespace GestionClinicaNutricional.Domain
{
    public class Paciente: AggregateRoot
    {
        [Key]
        public Guid Id { get; set; }
        public string CI { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Telefono { get; set; }
        
        public virtual ICollection<ConsultaInicial> ConsultaIniciales { get; set; } = new List<ConsultaInicial>();
        
        [JsonIgnore] public virtual ICollection<PlanAlimenticio> PlanAlimenticios { get; set; } = new List<PlanAlimenticio>();

        public static Paciente Crear(string ci, string nombre, string apellido, string? telefono = null)
        {
            // The id is assigned here, not by EF on AddAsync, because the event below is built in
            // this method: an unassigned id would travel as Guid.Empty.
            var paciente = new Paciente
            {
                Id = Guid.NewGuid(),
                Telefono = telefono,
                CI = ci,
                Nombre = nombre,
                Apellido = apellido
            };

            paciente.AddDomainEvent(new PacienteCreated(paciente.Id, ci, nombre, apellido, telefono));

            return paciente;
        }
    }
}