using Application.Common.Interfaces;
using Application.Companies.Commands.CreateCompany;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Entities;
using FakeItEasy;
using NUnit.Framework;

namespace Test.Companies
{
    [TestFixture]
    public class CreateCompanyCommandHandlerTests
    {
        [Test]
        public async Task Handle_Should_Create_Company_And_Return_Success()
        {
            // Arrange
            var repo = A.Fake<IGenericRepository<Company>>();
            var mapper = A.Fake<IMapper>();

            var dto = new CreateCompanyDto
            {
                Name = "Test Company",
                City = "Gothenburg"
            };

            var command = new CreateCompanyCommand(dto);

            var companyEntity = new Company
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                City = dto.City
            };

            var companyDto = new CompanyDto
            {
                Id = companyEntity.Id,
                Name = companyEntity.Name,
                City = companyEntity.City
            };

            // mapping setup
            A.CallTo(() => mapper.Map<Company>(dto))
                .Returns(companyEntity);

            A.CallTo(() => mapper.Map<CompanyDto>(companyEntity))
                .Returns(companyDto);

            var handler = new CreateCompanyCommandHandler(repo, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo("Test Company"));

            A.CallTo(() => repo.AddAsync(A<Company>.That.Matches(c =>
                    c.Name == "Test Company" && c.City == "Gothenburg")))
                .MustHaveHappenedOnceExactly();
        }
    }
}
