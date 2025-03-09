using AutoMapper;
using FluentValidation;
using MediatR;
using NetHelper.Common.Models;
using Wed.Application.Common.Interfaces;
using Wed.Application.Dtos;
using Wed.Domain.Entities;

namespace Wed.Application.Facility.Commands
{
    public record UpdateFacilityCommand : IRequest<ResultCustom<FacilityResponse>>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? OwnerId { get; set; }
        public string? Image { get; set; } = string.Empty;
        public string? FacilityType { get; set; } = string.Empty;
        public StatusFacility Status { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<UpdateFacilityCommand, Domain.Entities.Facility>();
                CreateMap<Domain.Entities.Facility, FacilityResponse>();
            }
        }
    }

    public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, ResultCustom<FacilityResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateFacilityCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResultCustom<FacilityResponse>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
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

            if (request.Name != null) entity.Name = request.Name;
            if (request.Address != null) entity.Address = request.Address;
            if (request.Description != null) entity.Description = request.Description;
            if (request.OwnerId != null) entity.OwnerId = request.OwnerId;
            if (request.FacilityType != null) entity.FacilityType = request.FacilityType;
            if (request.Status != null) entity.Status = request.Status;
            if (request.Image != null) entity.Image = request.Image;
            entity.UpdatedAt = DateTime.Now;

            _context.Facilities.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new ResultCustom<FacilityResponse>
            {
                Status = StatusCode.OK,
                Message = new[] { "Facility updated successfully" },
                Data = _mapper.Map<FacilityResponse>(entity)
            };
        }
    }

    public class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
    {
        public UpdateFacilityCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).MaximumLength(255);
            RuleFor(x => x.Address).MaximumLength(500);
        }
    }
}
