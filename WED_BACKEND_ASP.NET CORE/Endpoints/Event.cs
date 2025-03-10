using Microsoft.AspNetCore.Mvc;
using WED_BACKEND_ASP.NET_CORE.Dtos;
using WED_BACKEND_ASP.NET_CORE.Infrastructure;
using Wed.Application.Common.Interfaces;
using Wed.Application.Event.Commands;
using Wed.Application.Event.Queries;

namespace WED_BACKEND_ASP.NET_CORE.Endpoints;

public class Event : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization("owner")
            .MapPost(CreateEvent, "/create-event")
            .MapPut(UpdateEvent, "/update-event")
            .MapDelete(DeleteEvent, "/delete-event/{id}")
            .MapGet(GetAllEventByOwner, "/get-event-by-owner")
            ;

        app.MapGroup(this)
            .MapGet(GetAllEvent, "/get-all-event")
            .MapGet(GetEventById, "/get-event-by-id/{id}")
            ;

    }
    
    public async Task<IResult> CreateEvent([FromBody] CreateEventDto request,
        [FromServices] ISender sender, [FromServices] IUser user)
    {
        var result = await sender.Send(new CreateEventCommand
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = user.Id!,
            Image = request.Image,
            EventType = request.EventType,
            StartDate = request.StartDate,
            EndDate = request.EndDate
            
        });
        return Results.Ok(new
        {
            status = result.Status,
            message = result.Message,
            data = result.Data
        });
    }
    
    public async Task<IResult> UpdateEvent([FromBody] UpdateEventDto request,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(new UpdateEventCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Image = request.Image,
            EventType = request.EventType,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        });
        return Results.Ok(new
        {
            status = result.Status,
            message = result.Message,
            data = result.Data
        });
    }
    
    public async Task<IResult> DeleteEvent([FromRoute] Guid id,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(new DeleteEventCommand
        {
            Id = id
        });
        return Results.Ok(new
        {
            status = result.Status,
            message = result.Message,
            data = result.Data
        });
    }
    
    public async Task<IResult> GetAllEvent([FromServices] ISender sender, int page, int pageSize, string? name, string? eventType)
    {
        var result = await sender.Send(new GetAllEventQuery(){Page = page, PageSize = pageSize, Name = name, EventType = eventType});
        return Results.Ok(new
        {
            status = result.Status,
            message = result.Message,
            data = result.Data
        });
    }
    
    public async Task<IResult> GetEventById([FromRoute] Guid id,
        [FromServices] ISender sender)
    {
        var result = await sender.Send(new GetEventByIdQuery
        {
            Id = id
        });
        return Results.Ok(new
        {
            status = result.Status,
            message = result.Message,
            data = result.Data
        });
    }
    
    public async Task<IResult> GetAllEventByOwner([FromServices] ISender sender, [FromServices] IUser user, int page, int pageSize, string? name, string? eventType)
    {
        var result = await sender.Send(new GetAllEventByOwnerQuery(){Page = page, PageSize = pageSize, OwnerId = user.Id!, Name = name, EventType = eventType});
        return Results.Ok(new
        {
            status = result.Status,
            message = result.Message,
            data = result.Data
        });
    }

}