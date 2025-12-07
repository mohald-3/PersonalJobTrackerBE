using Application.Common.Interfaces;
using Application.Companies.Dtos;
using Application.Companies.Queries.GetCompanies;
using AutoMapper;
using Domain.Models.Entities;
using FakeItEasy;

namespace Test.Companies
{
    [TestFixture]
    public class GetCompaniesQueryHandlerTests
    {
        [Test]
        public async Task Handle_Should_Return_Paged_And_Filtered_Companies()
        {
            // Arrange
            var repo = A.Fake<IGenericRepository<Company>>();
            var mapper = A.Fake<IMapper>();

            var companies = new List<Company>
            {
                new() { Id = Guid.NewGuid(), Name = "Gothenburg Dev AB", City = "Gothenburg" },
                new() { Id = Guid.NewGuid(), Name = "Stockholm Tech AB", City = "Stockholm" },
                new() { Id = Guid.NewGuid(), Name = "Gothenburg Cloud AB", City = "Gothenburg" },
                new() { Id = Guid.NewGuid(), Name = "Malmö Consult AB", City = "Malmö" },
            };

            A.CallTo(() => repo.GetAllAsync()).Returns(companies);

            // We don't care about DTO contents for this test, only paging metadata
            A.CallTo(() => mapper.Map<IEnumerable<CompanyDto>>(A<IEnumerable<Company>>._))
             .ReturnsLazily(call =>
             {
                 var src = call.GetArgument<IEnumerable<Company>>(0)!;
                 return src.Select(c => new CompanyDto
                 {
                     Id = c.Id,
                     Name = c.Name,
                     City = c.City
                 }).ToList();
             });

            var handler = new GetCompaniesQueryHandler(repo, mapper);

            // Only Gothenburg, page 1, size 1
            var query = new GetCompaniesQuery(
                PageNumber: 1,
                PageSize: 1,
                Search: "Gothenburg",
                City: null,
                Country: null,
                Industry: null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var paged = result.Data!;
            // There are 2 Gothenburg companies total
            Assert.That(paged.TotalCount, Is.EqualTo(2));
            Assert.That(paged.PageNumber, Is.EqualTo(1));
            Assert.That(paged.PageSize, Is.EqualTo(1));
            Assert.That(paged.Items.Count(), Is.EqualTo(1));
        }
    }
}
