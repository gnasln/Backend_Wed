using FluentValidation;
using MediatR;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;

namespace Wed.Application.Facility.Commands
{
    public record DeleteFacilitycommand : IRequest<ResultCustom<string>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilitycommand, ResultCustom<string>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteFacilityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResultCustom<string>> Handle(DeleteFacilitycommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Facilities.FindAsync(request.Id);
            if (entity == null)
            {
                return new ResultCustom<string>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Facility not found" }
                };
            }
            _context.Facilities.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new ResultCustom<string>
            {
                Status = StatusCode.OK,
                Message = new[] { "Facility deleted successfully" }
            };
        }
    }

    public class DeleteFacilityCommandValidator : AbstractValidator<DeleteFacilitycommand>
    {
        public DeleteFacilityCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}