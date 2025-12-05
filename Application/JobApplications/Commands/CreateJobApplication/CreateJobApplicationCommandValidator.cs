using FluentValidation;

namespace Application.JobApplications.Commands.CreateJobApplication
{
    public class CreateJobApplicationCommandValidator
        : AbstractValidator<CreateJobApplicationCommand>
    {
        public CreateJobApplicationCommandValidator()
        {
            RuleFor(x => x.Dto.CompanyId)
                .NotEmpty().WithMessage("CompanyId is required.");

            RuleFor(x => x.Dto.PositionTitle)
                .NotEmpty().WithMessage("PositionTitle is required.")
                .MaximumLength(200).WithMessage("PositionTitle must be 200 characters or fewer.");

            RuleFor(x => x.Dto.ContactEmail)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactEmail))
                .WithMessage("ContactEmail must be a valid email address.");

            RuleFor(x => x.Dto.Priority)
                .InclusiveBetween(1, 5)
                .When(x => x.Dto.Priority.HasValue)
                .WithMessage("Priority must be between 1 and 5 when provided.");
        }
    }
}
