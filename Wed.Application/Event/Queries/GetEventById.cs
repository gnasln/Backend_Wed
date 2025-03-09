using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Event.Queries;

public class GetEventByIdQuery : IRequest<ResultCustom<EventResponse>>
{
    public Guid Id { get; set; }
    
}

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, ResultCustom<EventResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEventByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultCustom<EventResponse>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Events.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            return new ResultCustom<EventResponse>
            {
                Status = StatusCode.NOTFOUND,
                Message = new[] { "Event not found" }
            };
        }

        return new ResultCustom<EventResponse>
        {
            Status = StatusCode.OK,
            Data = _mapper.Map<EventResponse>(entity)
        };
    }
}
public class GetEventByIdQueryValidator : AbstractValidator<GetEventByIdQuery>
{
    public GetEventByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}