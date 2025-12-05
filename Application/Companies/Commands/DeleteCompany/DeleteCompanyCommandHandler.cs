using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, OperationResult<bool>>
    {
        private readonly IGenericRepository<Company> _companyRepository;

        public DeleteCompanyCommandHandler(IGenericRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<OperationResult<bool>> Handle(
            DeleteCompanyCommand request,
            CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id);
            if (company is null)
            {
                return OperationResult<bool>.Failure(
                    $"Company with id '{request.Id}' was not found.");
            }

            await _companyRepository.DeleteAsync(company);

            return OperationResult<bool>.Success(true);
        }
    }
}
