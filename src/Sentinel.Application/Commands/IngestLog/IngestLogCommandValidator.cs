using FluentValidation;

namespace Sentinel.Application.Commands.IngestLog;

public class IngestLogCommandValidator : AbstractValidator<IngestLogCommand>
{
    public IngestLogCommandValidator()
    {
        RuleFor(x => x.AgentId)
            .NotEmpty().WithMessage("AgentId is required.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(4000).WithMessage("Message cannot exceed 4000 characters.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source is required.")
            .MaximumLength(500).WithMessage("Source cannot exceed 500 characters.");

        RuleFor(x => x.Level)
            .InclusiveBetween(0, 4).WithMessage("Level must be between 0 (Debug) and 4 (Critical).");

        RuleFor(x => x.Timestamp)
            .NotEmpty().WithMessage("Timestamp is required.");
    }
}
