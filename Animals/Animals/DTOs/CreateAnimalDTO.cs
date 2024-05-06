using System.ComponentModel.DataAnnotations;

namespace Animals.DTOs;

public record CreateStudentRequest(
    [Required] [MaxLength(200)] string Name,
    [MaxLength(200)] string Description,
    [Required] [MaxLength(200)] string Category,
    [Required] [MaxLength(200)] string Area
);

public record CreateAnimalResponse(int idAnimal, string Name, string Description, string Category, String Area)
{
    public CreateAnimalResponse(int idAnimal, CreateStudentRequest request): this(idAnimal, request.Name, request.Description, request.Category, request.Area){}
};