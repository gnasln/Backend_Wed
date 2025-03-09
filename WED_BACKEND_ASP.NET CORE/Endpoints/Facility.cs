using Microsoft.AspNetCore.Mvc;
using Wed.Application.Common.Interfaces;
using Wed.Application.Facility.Commands;
using Wed.Application.Facility.Queries;
using Wed.Domain.Entities;
using WED_BACKEND_ASP.NET_CORE.Dtos;
using WED_BACKEND_ASP.NET_CORE.Infrastructure;

namespace WED_BACKEND_ASP.NET_CORE.Endpoints
{
    public class Facility : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(CreateFacility, "/create-facility")
                .MapGet(GetAllFacilityByOwner, "/get-all-facility-by-owner")
                .MapDelete(DeleteFacility, "/delete-facility/{FacilityId}")
                .MapPut(UpdateFacility, "/update-facility")
                ;

            app.MapGroup(this)
                .MapGet(GetAllFacility, "/get-all-facility")
                .MapGet(GetFacilityById, "/get-facility-by-id/{FacilityId}")
                ;

        }

        public async Task<IResult> CreateFacility([FromBody] CreateFacilityDto request,
            [FromServices] ISender sender, [FromServices] IUser user)
        {
            var result = await sender.Send(new CreateFacilityCommand
            {
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                OwnerId = user.Id,
                Image = request.Image,
                FacilityType = request.FacilityType,
                Status = (StatusFacility)request.Status
            });
            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }

        public async Task<IResult> GetAllFacility([FromServices] ISender sender, int page, int pageSize, string? name)
        {
            var result = await sender.Send(new GetAllFacilityQuery() { PageSize = pageSize, Page = page, Name = name });
            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }

        public async Task<IResult> GetAllFacilityByOwner([FromServices] ISender sender, int page, int pageSize, string? name, [FromServices] IUser user)
        {
            var result = await sender.Send(new GetAllFacilityByOwnerQuery() { PageSize = pageSize, Page = page, Name = name, OwnerId = user.Id });
            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }

        public async Task<IResult> DeleteFacility([FromRoute] Guid FacilityId,
            [FromServices] ISender sender)
        {
            var result = await sender.Send(new DeleteFacilitycommand
            {
                Id = FacilityId
            });
            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }

        public async Task<IResult> UpdateFacility([FromBody] UpdateFacilityDto request,
            [FromServices] ISender sender)
        {
            var result = await sender.Send(new UpdateFacilityCommand
            {
                Id = request.Id,
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                OwnerId = request.OwnerId,
                Image = request.Image,
                FacilityType = request.FacilityType,
                Status = (StatusFacility)request.Status
            });
            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }

        public async Task<IResult> GetFacilityById([FromRoute] Guid FacilityId,
            [FromServices] ISender sender)
        {
            var result = await sender.Send(new GetFacilityByIdQuery
            {
                Id = FacilityId
            });

            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }
    }
}