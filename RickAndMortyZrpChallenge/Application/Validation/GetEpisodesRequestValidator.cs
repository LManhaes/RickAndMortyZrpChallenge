using FluentValidation;
using RickAndMortyZrpChallenge.Application.Models.Requests;

namespace RickAndMortyZrpChallenge.Application.Validation
{
    public sealed class GetEpisodesRequestValidator : AbstractValidator<GetEpisodesRequest>
    {
        public GetEpisodesRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.Season)
                .InclusiveBetween(1, 9)
                .When(x => x.Season.HasValue)
                .WithMessage("Season must be between 1 and 9 when provided.");
        }
    }
}
