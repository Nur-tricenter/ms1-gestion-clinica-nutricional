using System.Linq;
using GestionClinicaNutricional.Domain;
using GestionClinicaNutricional.Domain.Events;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PacienteCrearTests
    {
        [Test]
        public void Crear_AssignsIdBeforeRaisingTheEvent()
        {
            var paciente = Paciente.Crear("1234567", "Ana", "Rojas");

            Assert.That(paciente.Id, Is.Not.EqualTo(System.Guid.Empty));
        }

        [Test]
        public void Crear_RaisesPacienteCreatedWithEachFieldInItsOwnSlot()
        {
            var paciente = Paciente.Crear("1234567", "Ana", "Rojas", "70012345");

            var evento = paciente.DomainEvents.OfType<PacienteCreated>().Single();

            Assert.Multiple(() =>
            {
                Assert.That(evento.PacienteId, Is.EqualTo(paciente.Id));
                Assert.That(evento.CI, Is.EqualTo("1234567"));
                Assert.That(evento.Nombre, Is.EqualTo("Ana"));
                Assert.That(evento.Apellido, Is.EqualTo("Rojas"));
                Assert.That(paciente.Telefono, Is.EqualTo("70012345"));
            });
        }
    }
}
