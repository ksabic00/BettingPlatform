using FluentValidation;
using FluentValidation.Results;

namespace BettingPlatform.Application.Common.Validation;

public static class AppValidation
{
    public static ValidationException Error(string property, string message, string? code = null) =>
        new ValidationException(new[]
        {
            new ValidationFailure(property, message) { ErrorCode = code }
        });

    public static ValidationException Errors(params (string Property, string Message, string? Code)[] items) =>
        new ValidationException(items.Select(x =>
            new ValidationFailure(x.Property, x.Message) { ErrorCode = x.Code }));
}
