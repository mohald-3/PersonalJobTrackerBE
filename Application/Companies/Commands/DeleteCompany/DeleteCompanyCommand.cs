using Domain.Models.Common;
using MediatR;

namespace Application.Companies.Commands.DeleteCompany
{
    public record DeleteCompanyCommand(Guid Id) : IRequest<OperationResult<bool>>;
}
