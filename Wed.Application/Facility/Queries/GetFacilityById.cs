using AutoMapper;
using FluentValidation;
using MediatR;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;

namespace Wed.Application.Facility.Queries
{
    public record GetFacilityByIdQuery : IRequest<ResultCustom<FacilityResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetFacilityByIdQueryHandler : IRequestHandler<GetFacilityByIdQuery, ResultCustom<FacilityResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetFacilityByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResultCustom<FacilityResponse>> Handle(GetFacilityByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Facilities.FindAsync(request.Id);
            if (entity == null)
            {
                return new ResultCustom<FacilityResponse>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Facility not found" }
                };
            }

            return new ResultCustom<FacilityResponse>
            {
                Status = StatusCode.OK,
                Data = _mapper.Map<FacilityResponse>(entity)
            };
        }
    }

    public class GetFacilityByIdQueryValidator : AbstractValidator<GetFacilityByIdQuery>
    {
        public GetFacilityByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}