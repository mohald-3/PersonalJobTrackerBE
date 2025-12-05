using Application.Common.Interfaces;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, OperationResult<CompanyDto>>
    {
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public UpdateCompanyCommandHandler(
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<CompanyDto>> Handle(
            UpdateCompanyCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var company = await _companyRepository.GetByIdAsync(dto.Id);
            if (company is null)
            {
                return OperationResult<CompanyDto>.Failure(
                    $"Company with id '{dto.Id}' was not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
                return OperationResult<CompanyDto>.Failure("Company name is required.");

            // Map changes from DTO onto the existing entity
            _mapper.Map(dto, company);

            await _companyRepository.UpdateAsync(company);

            var resultDto = _mapper.Map<CompanyDto>(company);
            return OperationResult<CompanyDto>.Success(resultDto);
        }
    }
}
