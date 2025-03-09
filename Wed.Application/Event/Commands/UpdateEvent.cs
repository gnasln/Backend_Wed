using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Event.Commands;

public record UpdateEventCommand : IRequest<ResultCustom<EventResponse>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateEventCommand, Domain.Entities.Event>();
            CreateMap<Domain.Entities.Event, EventResponse>();
        }
    }
}

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ResultCustom<EventResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateEventCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultCustom<EventResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
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
        
        if (!string.IsNullOrWhiteSpace(request.Name)) entity.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Description)) entity.Description = request.Description;
        if (!string.IsNullOrWhiteSpace(request.Image)) entity.Image = request.Image;
        if (request.StartDate != null) entity.StartDate = request.StartDate;
        if (request.EndDate != null) entity.EndDate = request.EndDate;
        if (!string.IsNullOrWhiteSpace(request.EventType)) entity.EventType = request.EventType;
        
        _context.Events.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new ResultCustom<EventResponse>
        {
            Status = StatusCode.CREATED,
            Message = new[] { "Event created successfully" },
            Data = _mapper.Map<EventResponse>(entity)
        };
    }
}