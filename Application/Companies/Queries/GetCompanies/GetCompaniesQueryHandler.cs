using Application.Common.Interfaces;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.Companies.Queries.GetCompanies
{
    public class GetCompaniesQueryHandler
        : IRequestHandler<GetCompaniesQuery, OperationResult<PagedResult<CompanyDto>>>
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

        public async Task<OperationResult<PagedResult<CompanyDto>>> Handle(
            GetCompaniesQuery request,
            CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            const int maxPageSize = 100;
            if (pageSize > maxPageSize) pageSize = maxPageSize;

            var allCompanies = await _companyRepository.GetAllAsync();
            var query = allCompanies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(c =>
                       c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (c.City != null && c.City.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (c.Country != null && c.Country.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (c.Industry != null && c.Industry.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var city = request.City.Trim();
                query = query.Where(c => c.City != null &&
                                         c.City.Contains(city, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                var country = request.Country.Trim();
                query = query.Where(c => c.Country != null &&
                                         c.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Industry))
            {
                var industry = request.Industry.Trim();
                query = query.Where(c => c.Industry != null &&
                                         c.Industry.Contains(industry, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = query.Count();

            var pagedCompanies = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = _mapper.Map<IEnumerable<CompanyDto>>(pagedCompanies);

            var pagedResult = PagedResult<CompanyDto>.Create(dtoList, totalCount, pageNumber, pageSize);

            return OperationResult<PagedResult<CompanyDto>>.Success(pagedResult);
        }
    }
}
