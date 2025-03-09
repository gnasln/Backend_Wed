using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Event.Commands;

public record CreateEventCommand : IRequest<ResultCustom<EventResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateEventCommand, Domain.Entities.Event>();
            CreateMap<Domain.Entities.Event, EventResponse>();
        }
    }
}

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, ResultCustom<EventResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateEventCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultCustom<EventResponse>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Domain.Entities.Event>(request);
        await _context.Events.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new ResultCustom<EventResponse>
        {
            Status = StatusCode.CREATED,
            Message = new[] { "Event created successfully" },
            Data = _mapper.Map<EventResponse>(entity)
        };
    }
}

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.EventType).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}