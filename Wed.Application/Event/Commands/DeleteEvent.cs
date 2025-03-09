using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;

namespace Wed.Application.Event.Commands;

public record DeleteEventCommand : IRequest<ResultCustom<string>>
{
    public required Guid Id { get; set; }
}

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, ResultCustom<string>>
{
    private readonly IApplicationDbContext _context;
    public DeleteEventCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ResultCustom<string>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Events.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            return new ResultCustom<string>
            {
                Status = StatusCode.NOTFOUND,
                Message = new[] { "Event not found" }
            };
        }
        _context.Events.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new ResultCustom<string>
        {
            Status = StatusCode.OK,
            Message = new[] { "Event deleted successfully" }
        };
    }
}