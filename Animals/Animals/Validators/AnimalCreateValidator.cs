using System.Text.RegularExpressions;
using FluentValidation;
using SqlClientExample.DTOs;

namespace Animals.Validators;

public class AnimalCreateValidator : AbstractValidator<CreateAnimalRequest>
{
    public AnimalCreateValidator()
    {
        RuleFor(s => s.Name).MaximumLength(200).NotNull();
        RuleFor(s => s.Description).MaximumLength(200).NotNull();
        RuleFor(s => s.Category).MaximumLength(200).NotNull();
        RuleFor(s => s.Area).MaximumLength(200).NotNull();
    }
}