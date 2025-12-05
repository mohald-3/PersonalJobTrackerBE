using FluentValidation;

namespace Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must be 200 characters or fewer.");

            RuleFor(x => x.Dto.OrgNumber)
                .MaximumLength(50).WithMessage("OrgNumber must be 50 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.OrgNumber));

            RuleFor(x => x.Dto.City)
                .MaximumLength(100).WithMessage("City must be 100 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.City));

            RuleFor(x => x.Dto.Country)
                .MaximumLength(100).WithMessage("Country must be 100 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Country));

            RuleFor(x => x.Dto.Industry)
                .MaximumLength(150).WithMessage("Industry must be 150 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Industry));

            RuleFor(x => x.Dto.WebsiteUrl)
                .MaximumLength(300).WithMessage("WebsiteUrl must be 300 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.WebsiteUrl));

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(2000).WithMessage("Notes must be 2000 characters or fewer.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
        }
    }
}
