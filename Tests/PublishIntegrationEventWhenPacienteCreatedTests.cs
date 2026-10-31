using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using GestionClinicaNutricional.Application.UseCase.DomainEventHandler;
using GestionClinicaNutricional.Domain;
using GestionClinicaNutricional.Domain.Events;
using Joseco.Communication.External.Contracts.Services;
using Joseco.Outbox.Contracts.Model;
using Microsoft.Extensions.Logging;
using Nur.Tricenter.Integration.Messages.Clinic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PublishIntegrationEventWhenPacienteCreatedTests
    {
        private IExternalPublisher publisher;
        private PublishIntegrationEventWhenPacienteCreated handler;

        [SetUp]
        public void Setup()
        {
            publisher = A.Fake<IExternalPublisher>();
            handler = new PublishIntegrationEventWhenPacienteCreated(
                A.Fake<ILogger<PublishIntegrationEventWhenPacienteCreated>>(),
                publisher);
        }

        [Test]
        public async Task Handle_PacienteCreated_PublishesTheKernelPatientSnapshotWithPhone()
        {
            // Arrange
            var paciente = Paciente.Crear("12345678", "Juan", "Perez", "70012345");
            var created = (PacienteCreated)paciente.DomainEvents.Single();
            var notification = new OutboxMessage<PacienteCreated>(created, "corr-1", "trace-1", "span-1");
            PatientPublished? published = null;
            A.CallTo(() => publisher.PublishAsync(A<PatientPublished>._, A<string?>._, A<bool>._))
                .Invokes((PatientPublished message, string? _, bool _) => published = message)
                .Returns(Task.CompletedTask);

            // Act
            await handler.Handle(notification, CancellationToken.None);

            // Assert
            Assert.That(published, Is.Not.Null);
            Assert.That(published!.PatientId, Is.EqualTo(paciente.Id));
            Assert.That(published.DocumentNumber, Is.EqualTo("12345678"));
            Assert.That(published.FirstName, Is.EqualTo("Juan"));
            Assert.That(published.LastName, Is.EqualTo("Perez"));
            Assert.That(published.Phone, Is.EqualTo("70012345"));
            Assert.That(published.CorrelationId, Is.EqualTo("corr-1"));
            A.CallTo(() => publisher.PublishAsync(A<PatientPublished>._, "paciente-published", true))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_PacienteWithoutPhone_PublishesNullPhone()
        {
            // Arrange
            var created = (PacienteCreated)Paciente.Crear("1", "Ana", "Gomez").DomainEvents.Single();
            PatientPublished? published = null;
            A.CallTo(() => publisher.PublishAsync(A<PatientPublished>._, A<string?>._, A<bool>._))
                .Invokes((PatientPublished message, string? _, bool _) => published = message)
                .Returns(Task.CompletedTask);

            // Act
            await handler.Handle(new OutboxMessage<PacienteCreated>(created), CancellationToken.None);

            // Assert
            Assert.That(published!.Phone, Is.Null);
        }
    }
}
