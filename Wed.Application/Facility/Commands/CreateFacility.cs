using AutoMapper;
using FluentValidation;
using MediatR;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;
using Wed.Domain.Entities;

namespace Wed.Application.Facility.Commands
{
    public record CreateFacilityCommand : IRequest<ResultCustom<FacilityResponse>>
    {
        public string? Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? OwnerId { get; set; }
        public string? FacilityType { get; set; } = string.Empty;
        public StatusFacility Status { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<CreateFacilityCommand, Domain.Entities.Facility>();
                CreateMap<Domain.Entities.Facility, FacilityResponse>();
            }
        }
    }

    public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, ResultCustom<FacilityResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CreateFacilityCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ResultCustom<FacilityResponse>> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entities.Facility>(request);
            await _context.Facilities.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new ResultCustom<FacilityResponse>
            {
                Status = StatusCode.CREATED,
                Message = new[] { "Facility created successfully" },
                Data = _mapper.Map<FacilityResponse>(entity)
            };
        }
    }

    public class CreateFacilityCommandValidator : AbstractValidator<CreateFacilityCommand>
    {
        public CreateFacilityCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.FacilityType).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}