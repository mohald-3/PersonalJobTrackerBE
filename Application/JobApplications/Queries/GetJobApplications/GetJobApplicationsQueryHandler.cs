using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;

namespace Application.JobApplications.Queries.GetJobApplications
{
    public class GetJobApplicationsQueryHandler
        : IRequestHandler<GetJobApplicationsQuery, OperationResult<PagedResult<JobApplicationDto>>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public GetJobApplicationsQueryHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<PagedResult<JobApplicationDto>>> Handle(
            GetJobApplicationsQuery request,
            CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            const int maxPageSize = 100;
            if (pageSize > maxPageSize) pageSize = maxPageSize;

            var allApplications = await _jobApplicationRepository.GetAllAsync();
            var query = allApplications.AsQueryable();

            if (request.CompanyId.HasValue && request.CompanyId.Value != Guid.Empty)
            {
                query = query.Where(a => a.CompanyId == request.CompanyId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(a => a.Status == request.Status.Value);
            }

            if (request.AppliedFrom.HasValue)
            {
                query = query.Where(a => a.AppliedDate.HasValue &&
                                         a.AppliedDate.Value.Date >= request.AppliedFrom.Value.Date);
            }

            if (request.AppliedTo.HasValue)
            {
                query = query.Where(a => a.AppliedDate.HasValue &&
                                         a.AppliedDate.Value.Date <= request.AppliedTo.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(a =>
                       a.PositionTitle.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (a.Source != null && a.Source.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (a.Notes != null && a.Notes.Contains(search, StringComparison.OrdinalIgnoreCase))
                    || (a.ContactEmail != null && a.ContactEmail.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var totalCount = query.Count();

            var pagedApplications = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = _mapper.Map<IEnumerable<JobApplicationDto>>(pagedApplications);

            var companies = await _companyRepository.GetAllAsync();
            var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

            foreach (var dto in dtoList)
            {
                if (companyDict.TryGetValue(dto.CompanyId, out var companyName))
                {
                    dto.CompanyName = companyName;
                }
            }

            var pagedResult = PagedResult<JobApplicationDto>.Create(dtoList, totalCount, pageNumber, pageSize);

            return OperationResult<PagedResult<JobApplicationDto>>.Success(pagedResult);
        }
    }
}
