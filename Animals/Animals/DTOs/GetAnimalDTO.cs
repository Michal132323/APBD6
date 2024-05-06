namespace Animals.DTOs;

public record GetAnimalResponse(int idAnimal, string Name, string Description, string Category, String Area);