using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using Application.JobApplications.Mapping;
using Application.JobApplications.Queries.GetJobApplications;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using Domain.Models.Enums;
using FakeItEasy;
using NUnit.Framework;

namespace Test.JobApplications
{
    [TestFixture]
    public class GetJobApplicationsQueryHandlerTests
    {
        [Test]
        public async Task Handle_Should_Filter_By_Company_And_Set_CompanyName()
        {
            // Arrange
            var jobRepo = A.Fake<IGenericRepository<JobApplication>>();
            var companyRepo = A.Fake<IGenericRepository<Company>>();

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
                    Status = ApplicationStatus.Applied
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company2.Id,
                    PositionTitle = "Dev 2",
                    Status = ApplicationStatus.Applied
                }
            };

            A.CallTo(() => jobRepo.GetAllAsync()).Returns(applications);
            A.CallTo(() => companyRepo.GetAllAsync()).Returns(new List<Company> { company1, company2 });

            var handler = new GetJobApplicationsQueryHandler(jobRepo, companyRepo, mapper);

            var query = new GetJobApplicationsQuery(
                PageNumber: 1,
                PageSize: 10,
                CompanyId: company1.Id,
                Status: null,
                AppliedFrom: null,
                AppliedTo: null,
                Search: null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var paged = result.Data!;
            Assert.That(paged.Items.Count(), Is.EqualTo(1));

            var item = paged.Items.First();
            Assert.That(item.CompanyId, Is.EqualTo(company1.Id));
            Assert.That(item.CompanyName, Is.EqualTo("Company One"));
        }
    }
}
