using Application.Common.Interfaces;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.Companies.Queries.GetCompanies
{
    public class GetCompaniesQueryHandler
        : IRequestHandler<GetCompaniesQuery, OperationResult<IEnumerable<CompanyDto>>>
    {
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public GetCompaniesQueryHandler(
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<IEnumerable<CompanyDto>>> Handle(
            GetCompaniesQuery request,
            CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllAsync();

            var dtoList = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return OperationResult<IEnumerable<CompanyDto>>.Success(dtoList);
        }
    }
}
