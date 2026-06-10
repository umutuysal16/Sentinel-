using FluentValidation;

namespace Sentinel.Application.Commands.RegisterAgent;

public class RegisterAgentCommandValidator : AbstractValidator<RegisterAgentCommand>
{
    public RegisterAgentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Agent name is required.")
            .MaximumLength(200).WithMessage("Agent name cannot exceed 200 characters.");

        RuleFor(x => x.DeviceType)
            .NotEmpty().WithMessage("Device type is required.")
            .MaximumLength(200).WithMessage("Device type cannot exceed 200 characters.");
    }
}
