using Application.Common.Interfaces;
using Application.Dashboard.Queries;
using Application.JobApplications.Mapping;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Models.Enums;
using FakeItEasy;
using NUnit.Framework;

namespace Test.Dashboard
{
    [TestFixture]
    public class GetDashboardOverviewQueryHandlerTests
    {
        [Test]
        public async Task Handle_Should_Calculate_Totals_And_StatusCounts()
        {
            // Arrange
            var companyRepo = A.Fake<IGenericRepository<Company>>();
            var jobRepo = A.Fake<IGenericRepository<JobApplication>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<JobApplicationProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var company1 = new Company { Id = Guid.NewGuid(), Name = "Company One" };
            var company2 = new Company { Id = Guid.NewGuid(), Name = "Company Two" };

            var applications = new List<JobApplication>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company1.Id,
                    PositionTitle = "Dev 1",
                    Status = ApplicationStatus.Applied,
                    AppliedDate = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company1.Id,
                    PositionTitle = "Dev 2",
                    Status = ApplicationStatus.Interview,
                    AppliedDate = DateTime.UtcNow.AddDays(-3)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company2.Id,
                    PositionTitle = "Dev 3",
                    Status = ApplicationStatus.Rejected,
                    AppliedDate = DateTime.UtcNow.AddDays(-1)
                }
            };

            A.CallTo(() => companyRepo.GetAllAsync())
                .Returns(new List<Company> { company1, company2 });
            A.CallTo(() => jobRepo.GetAllAsync())
                .Returns(applications);

            var handler = new GetDashboardOverviewQueryHandler(companyRepo, jobRepo, mapper);

            var query = new GetDashboardOverviewQuery(RecentDays: 30, TopCompaniesCount: 5);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var overview = result.Data!;
            Assert.That(overview.TotalCompanies, Is.EqualTo(2));
            Assert.That(overview.TotalApplications, Is.EqualTo(3));

            var appliedCount = overview.ApplicationsByStatus
                .First(x => x.Status == ApplicationStatus.Applied).Count;
            var rejectedCount = overview.ApplicationsByStatus
                .First(x => x.Status == ApplicationStatus.Rejected).Count;

            Assert.That(appliedCount, Is.EqualTo(1));
            Assert.That(rejectedCount, Is.EqualTo(1));

            // Top company should be company1 (2 apps vs 1)
            var top = overview.TopCompaniesByApplications.First();
            Assert.That(top.CompanyId, Is.EqualTo(company1.Id));
            Assert.That(top.ApplicationsCount, Is.EqualTo(2));
        }
    }
}
