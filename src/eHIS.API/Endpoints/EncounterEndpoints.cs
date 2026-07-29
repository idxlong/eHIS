using MediatR;
using eHIS.Application.Encounters;
using eHIS.Application.DTOs;

namespace eHIS.API.Endpoints;

public static class EncounterEndpoints
{
    public static void MapEncounterEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/encounters")
            .WithTags("Encounters");

        group.MapPost("/", async (StartEncounterCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/encounters/{id}", id);
        });

        group.MapPost("/{id}/diagnoses", async (string id, AddEncounterDiagnosisInput input, ISender sender) =>
        {
            var command = new AddEncounterDiagnosisCommand(id, input.Condition, input.Use, input.Rank);
            await sender.Send(command);
            return Results.Ok();
        });

        group.MapPost("/{id}/complete", async (string id, CompleteEncounterInput input, ISender sender) =>
        {
            var command = new CompleteEncounterCommand(id, input.EndTime);
            await sender.Send(command);
            return Results.Ok();
        });

        group.MapGet("/{id}", async (string id, ISender sender) =>
        {
            var encounter = await sender.Send(new GetEncounterByIdQuery(id));
            return encounter is not null ? Results.Ok(encounter) : Results.NotFound();
        });
    }
}

public record AddEncounterDiagnosisInput(CodeableConceptDto Condition, CodingDto? Use, int? Rank);
public record CompleteEncounterInput(DateTime EndTime);
