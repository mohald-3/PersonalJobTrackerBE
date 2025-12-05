using Application.Common.Interfaces;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler
        : IRequestHandler<CreateCompanyCommand, OperationResult<CompanyDto>>
    {
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public CreateCompanyCommandHandler(
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<CompanyDto>> Handle(
            CreateCompanyCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (string.IsNullOrWhiteSpace(dto.Name))
                return OperationResult<CompanyDto>.Failure("Company name is required.");

            var entity = _mapper.Map<Company>(dto);

            await _companyRepository.AddAsync(entity);

            var resultDto = _mapper.Map<CompanyDto>(entity);

            return OperationResult<CompanyDto>.Success(resultDto);
        }
    }
}
