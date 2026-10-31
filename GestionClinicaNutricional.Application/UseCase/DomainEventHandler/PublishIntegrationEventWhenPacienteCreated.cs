using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using GestionClinicaNutricional.Domain.Events;
using Nur.Tricenter.Integration.Messages.Clinic;
using Microsoft.Extensions.Logging;

namespace GestionClinicaNutricional.Application.UseCase.DomainEventHandler
{
    /// <summary>
    /// Translates the outbox message that carries <see cref="PacienteCreated"/> into the
    /// kernel <see cref="PatientPublished"/> contract and publishes it to RabbitMQ.
    /// </summary>
    public class PublishIntegrationEventWhenPacienteCreated
        : INotificationHandler<OutboxMessage<PacienteCreated>>
    {
        public const string Destination = "paciente-published";
        public const string Source = "gestion-clinica-nutricional";

        private readonly ILogger<PublishIntegrationEventWhenPacienteCreated> _logger;
        private readonly IExternalPublisher _publisher;

        public PublishIntegrationEventWhenPacienteCreated(
            ILogger<PublishIntegrationEventWhenPacienteCreated> logger,
            IExternalPublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task Handle(OutboxMessage<PacienteCreated> notification, CancellationToken cancellationToken = default)
        {
            var pacienteCreated = notification.Content;

            var message = new PatientPublished(notification.CorrelationId, Source)
            {
                PatientId = pacienteCreated.PacienteId,
                DocumentNumber = pacienteCreated.CI,
                FirstName = pacienteCreated.Nombre,
                LastName = pacienteCreated.Apellido,
                Phone = pacienteCreated.Telefono
            };

            _logger.LogInformation(
                "Publishing {EventType} for paciente {PacienteId}: {Nombre} {Apellido}",
                notification.Content.GetType().Name, pacienteCreated.PacienteId, pacienteCreated.Nombre, pacienteCreated.Apellido);

            await _publisher.PublishAsync(message, Destination, declareDestination: true);
        }
    }
}