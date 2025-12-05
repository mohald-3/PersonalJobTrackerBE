using Application.Common.Interfaces;
using Application.Companies.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, OperationResult<CompanyDto>>
    {
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public GetCompanyByIdQueryHandler(
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<CompanyDto>> Handle(
            GetCompanyByIdQuery request,
            CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id);
            if (company is null)
            {
                return OperationResult<CompanyDto>.Failure(
                    $"Company with id '{request.Id}' was not found.");
            }

            var dto = _mapper.Map<CompanyDto>(company);
            return OperationResult<CompanyDto>.Success(dto);
        }
    }
}
