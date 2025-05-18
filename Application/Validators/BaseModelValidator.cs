using FluentValidation;
using BookKeepAPI.Application.Models;

namespace BookKeepAPI.Application.Validators;

/// <summary>
/// Validator for the <see cref="BaseModel"/> class, ensuring common base properties are valid.
/// Ensures that common base properties like Id and Guid are valid.
/// </summary>
public class BaseModelValidator : AbstractValidator<BaseModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseModelValidator"/> class.
    /// </summary>
    public BaseModelValidator()
    {
        RuleFor(bm => bm.Id)
            .GreaterThanOrEqualTo(0UL).WithMessage("Id must be greater than or equal to 0.");

        RuleFor(bm => bm.Guid)
            .NotEmpty().WithMessage("Guid cannot be empty.")
            .NotEqual(Guid.Empty).WithMessage("Guid must be a valid GUID.");

        RuleFor(bm => bm.IsActive)
            .NotNull().WithMessage("IsActive status must be specified.");

        // no validation for CreatedOn and UpdatedOn as they are managed by the api/database
    }
}
