using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Facility.Queries;

public class GetAllFacilityByOwnerQuery : IRequest<ResultCustomPaginate<IEnumerable<FacilityResponse>>>
{
    public string OwnerId { get; set; } = string.Empty;
    public int PageSize { get; set; }
    public int Page { get; set; }
    public string? Name { get; set; } = string.Empty;
    
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<FacilityResponse, Domain.Entities.Facility>();
        }
    }
}

public class GetAllFacilityByOwnerQueryHandle : IRequestHandler<GetAllFacilityByOwnerQuery, ResultCustomPaginate<IEnumerable<FacilityResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllFacilityByOwnerQueryHandle(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ResultCustomPaginate<IEnumerable<FacilityResponse>>> Handle(GetAllFacilityByOwnerQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Facilities.AsQueryable();
        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.Name.Contains(request.Name));
        }
        if (!string.IsNullOrEmpty(request.OwnerId))
        {
            query = query.Where(x => x.OwnerId == request.OwnerId);
        }
        // Lấy tổng số bản ghi
        var totalItems = await query.CountAsync(cancellationToken);

        // Lấy danh sách bản ghi theo phân trang
        var entity = await query
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<IEnumerable<FacilityResponse>>(entity);

        return new ResultCustomPaginate<IEnumerable<FacilityResponse>>
        {
            Status = StatusCode.OK,
            Message = new[] { "Get all facility successfully" },
            Data = response,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
            PageNumber = request.Page,
            PageSize = request.PageSize
        };
    }
    
    public class GetAllFacilityByOwnerQueryValidator : AbstractValidator<GetAllFacilityQuery>
    {
        public GetAllFacilityByOwnerQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
        }
    }
}

