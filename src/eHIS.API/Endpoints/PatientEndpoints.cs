using MediatR;
using eHIS.Application.Patients;
using eHIS.Application.DTOs;

namespace eHIS.API.Endpoints;

public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients")
            .WithTags("Patients");

        group.MapPost("/", async (CreatePatientCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/patients/{id}", id);
        });

        group.MapGet("/{id}", async (string id, ISender sender) =>
        {
            var patient = await sender.Send(new GetPatientByIdQuery(id));
            return patient is not null ? Results.Ok(patient) : Results.NotFound();
        });
    }
}
