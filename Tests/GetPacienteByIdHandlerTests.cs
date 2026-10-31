using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using GestionClinicaNutricional.Application.Paciente;
using GestionClinicaNutricional.Domain.Repositories;
using Joseco.DDD.Core.Results;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GetPacienteByIdHandlerTests
    {
        private IPacienteRepository pacienteRepository;

        [SetUp]
        public void Setup()
        {
            pacienteRepository = A.Fake<IPacienteRepository>();
        }

        [Test]
        public async Task GetPacienteById_ShouldReturnNotFound_WhenRepositoryThrowsNullReference()
        {
            var id = Guid.NewGuid();
            A.CallTo(() => pacienteRepository.GetByIdAsync(id, true))
                .Throws(new NullReferenceException("Paciente no encontrado."));

            var handler = new GetPacienteByIdHandler(pacienteRepository);

            var result = await handler.Handle(new GetPacienteByIdQuery { Id = id }, CancellationToken.None);

            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
        }
    }
}
