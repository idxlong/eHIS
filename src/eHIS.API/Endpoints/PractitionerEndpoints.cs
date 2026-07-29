using MediatR;
using eHIS.Application.Practitioners;

namespace eHIS.API.Endpoints;

public static class PractitionerEndpoints
{
    public static void MapPractitionerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/practitioners")
            .WithTags("Practitioners");

        group.MapPost("/", async (CreatePractitionerCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/practitioners/{id}", id);
        });

        group.MapGet("/{id}", async (string id, ISender sender) =>
        {
            var practitioner = await sender.Send(new GetPractitionerByIdQuery(id));
            return practitioner is not null ? Results.Ok(practitioner) : Results.NotFound();
        });
    }
}
