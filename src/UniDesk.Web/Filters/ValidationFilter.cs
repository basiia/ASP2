using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.Filters
{
    public class ValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            foreach (var argument in context.Arguments)
            {
                if (argument == null)
                {
                    continue;
                }

                var argumentType = argument.GetType();

                if (argumentType.IsPrimitive ||
                    argument is string ||
                    argumentType.IsEnum)
                {
                    continue;
                }

                var validationContext = new ValidationContext(argument);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(
                    argument,
                    validationContext,
                    validationResults,
                    validateAllProperties: true);

                if (!isValid)
                {
                    var errors = validationResults
                        .GroupBy(result => result.MemberNames.FirstOrDefault() ?? string.Empty)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(result => result.ErrorMessage ?? "Nieprawidłowa wartość.")
                                .ToArray());

                    return Results.ValidationProblem(errors);
                }
            }

            return await next(context);
        }
    }
}