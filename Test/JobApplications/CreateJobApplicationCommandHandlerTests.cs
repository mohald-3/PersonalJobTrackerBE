using Application.Common.Interfaces;
using Application.JobApplications.Commands.CreateJobApplication;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Models.Enums;
using FakeItEasy;

namespace Test.JobApplications
{
    [TestFixture]
    public class CreateJobApplicationCommandHandlerTests
    {
        [Test]
        public async Task Handle_Should_Return_Failure_When_Company_Does_Not_Exist()
        {
            // Arrange
            var jobRepo = A.Fake<IGenericRepository<JobApplication>>();
            var companyRepo = A.Fake<IGenericRepository<Company>>();
            var mapper = A.Fake<IMapper>();

            var dto = new CreateJobApplicationDto
            {
                CompanyId = Guid.NewGuid(),
                PositionTitle = "Backend Developer",
                Status = ApplicationStatus.Applied
            };

            var command = new CreateJobApplicationCommand(dto);

            A.CallTo(() => companyRepo.GetByIdAsync(dto.CompanyId))
                .Returns((Company?)null);

            var handler = new CreateJobApplicationCommandHandler(jobRepo, companyRepo, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Null);
            Assert.That(result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)), Is.True);

            A.CallTo(() => jobRepo.AddAsync(A<JobApplication>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_Create_JobApplication_And_Return_Success()
        {
            // Arrange
            var jobRepo = A.Fake<IGenericRepository<JobApplication>>();
            var companyRepo = A.Fake<IGenericRepository<Company>>();
            var mapper = A.Fake<IMapper>();

            var companyId = Guid.NewGuid();
            var company = new Company
            {
                Id = companyId,
                Name = "Test Company"
            };

            var dto = new CreateJobApplicationDto
            {
                CompanyId = companyId,
                PositionTitle = "Backend Developer",
                Status = ApplicationStatus.Applied,
                Priority = 3
            };

            var command = new CreateJobApplicationCommand(dto);

            var entity = new JobApplication
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PositionTitle = dto.PositionTitle,
                Status = dto.Status,
                Priority = dto.Priority
            };

            var resultDto = new JobApplicationDto
            {
                Id = entity.Id,
                CompanyId = companyId,
                CompanyName = company.Name,
                PositionTitle = entity.PositionTitle,
                Status = entity.Status,
                Priority = entity.Priority
            };

            A.CallTo(() => companyRepo.GetByIdAsync(companyId))
                .Returns(company);

            A.CallTo(() => mapper.Map<JobApplication>(dto))
                .Returns(entity);

            A.CallTo(() => mapper.Map<JobApplicationDto>(entity))
                .Returns(resultDto);

            var handler = new CreateJobApplicationCommandHandler(jobRepo, companyRepo, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.CompanyId, Is.EqualTo(companyId));
            Assert.That(result.Data!.PositionTitle, Is.EqualTo("Backend Developer"));

            A.CallTo(() => jobRepo.AddAsync(A<JobApplication>.That.Matches(a =>
                    a.CompanyId == companyId &&
                    a.PositionTitle == "Backend Developer")))
                .MustHaveHappenedOnceExactly();
        }
    }
}
