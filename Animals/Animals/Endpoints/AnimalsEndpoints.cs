using Animals.DTOs;

namespace Animals.Endpoints;

using System.Data;
using System.Data.SqlClient;
using FluentValidation;
using SqlClientExample.DTOs;
using Dapper;
/*
 * SqlClient with Dapper example
 * https://www.learndapper.com/
 */

public static class AnimalsDapperEndpoints
{
    public static void RegisterAnimalsDapperEndpoints(this WebApplication app)
    {
        var Animals = app.MapGroup("minimal-students-dapper");

        Animals.MapGet("/", GetAnimals);
        Animals.MapGet("{id:int}", GetAnimal);
        Animals.MapPost("/", CreateAnimal);
        Animals.MapDelete("{id:int}", RemoveAnimal);
        Animals.MapPut("{id:int}", ReplaceAnimal);
    }

    private static IResult ReplaceAnimal(IConfiguration configuration, IValidator<ReplaceAnimalRequest> validator, int id, ReplaceAnimalRequest request)
    {
        
        var validation = validator.Validate(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }
        
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var affectedRows = sqlConnection.Execute(
                "UPDATE Animals SET Name = @2, Description = @3, Category = @4, Area = @5, WHERE IdAnimal = @Id",
                new
                {
                    Name=request.Name,
                    Description=request.Description,
                    Category=request.Category,
                    Area=request.Area
                }
            );
            
            if (affectedRows == 0) return Results.NotFound();
        }

        return Results.NoContent();
    }

    private static IResult RemoveAnimal(IConfiguration configuration, int id)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var affectedRows = sqlConnection.Execute(
                "DELETE FROM Students WHERE ID = @Id",
                new { Id = id }
            );
            return affectedRows == 0 ? Results.NotFound() : Results.NoContent();
        }
    }

    private static IResult CreateAnimal(IConfiguration configuration, IValidator<CreateStudentRequest> validator, CreateStudentRequest request)
    {
        var validation = validator.Validate(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var id = sqlConnection.ExecuteScalar<int>(
                "INSERT INTO Animals (Name, Description, Category, Area) values (@Name, @Description, @Category, @Area); SELECT CAST(SCOPE_IDENTITY() as int)",
                new
                {
                    Name=request.Name,
                    Description=request.Description,
                    Category=request.Category,
                    Area=request.Area
                }
            );

            return Results.Created($"/students-dapper/{id}", new CreateAnimalResponse(id, request));
        }
    }

    private static IResult GetAnimals(IConfiguration configuration)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var Animals = sqlConnection.Query<GetAnimalsResponse>("SELECT * FROM Animals");
            return Results.Ok(Animals);
        }
    }

    private static IResult GetAnimal(IConfiguration configuration, int id)
    {
        using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
        {
            var Animal = sqlConnection.QuerySingleOrDefault<GetAnimalResponse>(
                "SELECT * FROM Animals WHERE ID = @IdAnimal",
                new { idAnimal = id }
            );

            if (Animal is null) return Results.NotFound();
            return Results.Ok(Animal);
        }
    }
}