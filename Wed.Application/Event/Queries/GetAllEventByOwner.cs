using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Event.Queries;

public record GetAllEventByOwnerQuery : IRequest<ResultCustomPaginate<IEnumerable<EventResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OwnerId { get; set; } = string.Empty;
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

public class
    GetAllEventByOwnerQueryHandle : IRequestHandler<GetAllEventByOwnerQuery,
    ResultCustomPaginate<IEnumerable<EventResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllEventByOwnerQueryHandle(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultCustomPaginate<IEnumerable<EventResponse>>> Handle(GetAllEventByOwnerQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Events.AsQueryable();
        if (!string.IsNullOrEmpty(request.OwnerId))
        {
            query = query.Where(x => x.OwnerId == request.OwnerId);
        }

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
            Message = new[] { "Get all facility successfully" },
            Data = response,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
            PageNumber = request.Page,
            PageSize = request.PageSize
        };
    }

    public class GetAllEventByOwnerQueryValidator : AbstractValidator<GetAllEventByOwnerQuery>
    {
        public GetAllEventByOwnerQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}
