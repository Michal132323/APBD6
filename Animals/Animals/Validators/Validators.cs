namespace Animals.Validators;

using FluentValidation;


public static class Validators
{
    public static void RegisterValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AnimalUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<AnimalCreateValidator>();
    }
}