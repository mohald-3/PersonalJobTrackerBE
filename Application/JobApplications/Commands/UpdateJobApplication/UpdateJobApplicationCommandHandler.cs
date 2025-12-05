using Application.Common.Interfaces;
using Application.JobApplications.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.JobApplications.Commands.UpdateJobApplication
{
    public class UpdateJobApplicationCommandHandler
        : IRequestHandler<UpdateJobApplicationCommand, OperationResult<JobApplicationDto>>
    {
        private readonly IGenericRepository<JobApplication> _jobApplicationRepository;
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly IMapper _mapper;

        public UpdateJobApplicationCommandHandler(
            IGenericRepository<JobApplication> jobApplicationRepository,
            IGenericRepository<Company> companyRepository,
            IMapper mapper)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<JobApplicationDto>> Handle(
            UpdateJobApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var entity = await _jobApplicationRepository.GetByIdAsync(dto.Id);
            if (entity is null)
            {
                return OperationResult<JobApplicationDto>.Failure(
                    $"JobApplication with id '{dto.Id}' was not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.PositionTitle))
                return OperationResult<JobApplicationDto>.Failure("PositionTitle is required.");

            // Optional: validate company
            //if (dto.CompanyId != Guid.Empty && dto.CompanyId != entity.CompanyId)
            //{
            //    var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
            //    if (company is null)
            //    {
            //        return OperationResult<JobApplicationDto>.Failure(
            //            $"Company with id '{dto.CompanyId}' was not found.");
            //    }
            //}

            // Map dto onto existing entity
            _mapper.Map(dto, entity);

            await _jobApplicationRepository.UpdateAsync(entity);

            var resultDto = _mapper.Map<JobApplicationDto>(entity);
            return OperationResult<JobApplicationDto>.Success(resultDto);
        }
    }
}
