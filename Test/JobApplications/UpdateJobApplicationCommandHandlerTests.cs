using Application.Common.Interfaces;
using Application.JobApplications.Commands.UpdateJobApplication;
using Application.JobApplications.Dtos;
using Application.JobApplications.Mapping;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Models.Enums;
using FakeItEasy;

namespace Test.JobApplications
{
    [TestFixture]
    public class UpdateJobApplicationCommandHandlerTests
    {
        [Test]
        public async Task Handle_Should_Update_Fields_And_LastUpdated()
        {
            // Arrange
            var jobRepo = A.Fake<IGenericRepository<JobApplication>>();
            var companyRepo = A.Fake<IGenericRepository<Company>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<JobApplicationProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var id = Guid.NewGuid();
            var existing = new JobApplication
            {
                Id = id,
                CompanyId = Guid.NewGuid(),
                PositionTitle = "Old Title",
                Status = ApplicationStatus.Applied,
                Priority = 1,
                LastUpdated = DateTime.UtcNow.AddDays(-2)
            };

            var originalLastUpdated = existing.LastUpdated;

            A.CallTo(() => jobRepo.GetByIdAsync(id))
                .Returns(existing);

            // We don't change company in this test -> no companyRepo calls needed

            var dto = new UpdateJobApplicationDto
            {
                Id = id,
                CompanyId = existing.CompanyId,
                PositionTitle = "New Title",
                Status = ApplicationStatus.Interview,
                Priority = 5
            };

            var command = new UpdateJobApplicationCommand(dto);

            var handler = new UpdateJobApplicationCommandHandler(jobRepo, companyRepo, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.PositionTitle, Is.EqualTo("New Title"));
            Assert.That(result.Data!.Status, Is.EqualTo(ApplicationStatus.Interview));

            // LastUpdated should be updated to something >= original
            Assert.That(result.Data!.LastUpdated, Is.GreaterThan(originalLastUpdated));

            A.CallTo(() => jobRepo.UpdateAsync(existing))
                .MustHaveHappenedOnceExactly();
        }
    }
}
