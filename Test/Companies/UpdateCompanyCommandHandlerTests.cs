using Application.Common.Interfaces;
using Application.Companies.Commands.UpdateCompany;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Entities;
using FakeItEasy;
using NUnit.Framework;

namespace Test.Companies
{
    [TestFixture]
    public class UpdateCompanyCommandHandlerTests
    {
        [Test]
        public async Task Handle_Should_Return_Failure_When_Company_Not_Found()
        {
            // Arrange
            var repo = A.Fake<IGenericRepository<Company>>();
            var mapper = A.Fake<IMapper>();

            var dto = new UpdateCompanyDto
            {
                Id = Guid.NewGuid(),
                Name = "Updated Name"
            };

            var command = new UpdateCompanyCommand(dto);

            // No company found
            A.CallTo(() => repo.GetByIdAsync(dto.Id))
                .Returns((Company?)null);

            var handler = new UpdateCompanyCommandHandler(repo, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Null);
            Assert.That(result.Errors.Count, Is.GreaterThan(0));
            Assert.That(result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)), Is.True);

            A.CallTo(() => repo.UpdateAsync(A<Company>._))
                .MustNotHaveHappened();
        }
    }
}
