namespace Animals.DTOs;

public record GetAnimalsResponse(int idAnimal, string Name, string Description, string Category, String Area);