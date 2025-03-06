using Microsoft.AspNetCore.Identity;
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
                
                ;

            app.MapGroup(this)
                .MapGet(GetAllFacility, "/get-all-facility")
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
            var result = await sender.Send(new GetAllFacilityQuery(){ PageSize = pageSize, Page = page, Name = name });
            return Results.Ok(new
            {
                status = result.Status,
                message = result.Message,
                data = result.Data
            });
        }

    }
}
