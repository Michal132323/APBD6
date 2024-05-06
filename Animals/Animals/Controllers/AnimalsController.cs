using Animals.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Animals.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAllAnimals()
    {
        var response = new List<GetAnimalsResponse>();
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand("SELECT * FROM Animals", sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                response.Add(new GetAnimalsResponse(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)
                    )
                );
            }
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetAnimals(int id)
    {
        using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        var sqlCommand = new SqlCommand("SELECT * FROM Animals WHERE ID = @1", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@1", id);
        sqlCommand.Connection.Open();

        var reader = sqlCommand.ExecuteReader();
        if (!reader.Read()) return NotFound();

        return Ok(new GetAnimalDetailsResponse(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)
            )
        );
    }

    [HttpPost]
    public IActionResult CreateAnimal(CreateAnimalRequest request)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "INSERT INTO Animals (FirstName, LastName, Phone, Birthdate) values (@1, @2, @3, @4); SELECT CAST(SCOPE_IDENTITY() as int)",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Connection.Open();

            var id = sqlCommand.ExecuteScalar();

            return Created($"students/{id}", new CreateAnimalResponse((int)id, request));
        }
    }

    [HttpPut("{id}")]
    public IActionResult ReplaceAnimal(int id, CreateAnimalRequest request)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "UPDATE Animals SET Name = @2, Description = @3, Category = @4, Area = @5, WHERE IdAnimal = @1",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.IdAnimal);
            sqlCommand.Parameters.AddWithValue("@2", request.Name);
            sqlCommand.Parameters.AddWithValue("@3", request.Description);
            sqlCommand.Parameters.AddWithValue("@4", request.Category);
            sqlCommand.Parameters.AddWithValue("@5", request.Area);
            sqlCommand.Connection.Open();

            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveAnimal(int id)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var command = new SqlCommand("DELETE FROM Animals WHERE ID = @1", sqlConnection);
            command.Parameters.AddWithValue("@1", id);
            command.Connection.Open();

            var affectedRows = command.ExecuteNonQuery();

            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }
}