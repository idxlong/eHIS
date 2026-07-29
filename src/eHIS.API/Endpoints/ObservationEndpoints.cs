using MediatR;
using eHIS.Application.Observations;

namespace eHIS.API.Endpoints;

public static class ObservationEndpoints
{
    public static void MapObservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/observations")
            .WithTags("Observations");

        group.MapPost("/", async (RecordObservationCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/observations/{id}", id);
        });

        group.MapGet("/patient/{patientId}", async (string patientId, ISender sender) =>
        {
            var observations = await sender.Send(new GetObservationsByPatientQuery(patientId));
            return Results.Ok(observations);
        });
    }
}
