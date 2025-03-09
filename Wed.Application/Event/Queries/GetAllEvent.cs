using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Event.Queries;

public record GetAllEventQuery : IRequest<ResultCustomPaginate<IEnumerable<EventResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; } = string.Empty;
    public string? EventType { get; set; } = string.Empty;
    
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<EventResponse, Domain.Entities.Event>();
        }
    }
}

public class GetAllEventQueryHandler : IRequestHandler<GetAllEventQuery, ResultCustomPaginate<IEnumerable<EventResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllEventQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultCustomPaginate<IEnumerable<EventResponse>>> Handle(GetAllEventQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Events.AsQueryable();
        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.Name.Contains(request.Name));
        }
        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(x => x.EventType.Contains(request.EventType));
        }
        // Lấy tổng số bản ghi
        var totalItems = await query.CountAsync(cancellationToken);

        // Lấy danh sách bản ghi theo phân trang
        var entity = await query
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<IEnumerable<EventResponse>>(entity);

        return new ResultCustomPaginate<IEnumerable<EventResponse>>
        {
            Status = StatusCode.OK,
            Message = new[] { "Get all event successfully" },
            Data = response,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
            PageNumber = request.Page,
            PageSize = request.PageSize
        };
    }
}

public class GetAllEventQueryValidator : AbstractValidator<GetAllEventQuery>
{
    public GetAllEventQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}