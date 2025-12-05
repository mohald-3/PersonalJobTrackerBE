using Application.Companies.Dtos;
using Domain.Models.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Companies.Commands.CreateCompany
{

    public record CreateCompanyCommand(CreateCompanyDto Dto) : IRequest<OperationResult<CompanyDto>>;
}
