using FluentValidation;
using RickAndMortyZrpChallenge.Application.Models;

namespace RickAndMortyZrpChallenge.Application.Validation
{
    public sealed class GetCharacterRequestValidator : AbstractValidator<GetCharacterRequest>
    {
        public GetCharacterRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Character id must be greater than 0.");
        }
    }
}
