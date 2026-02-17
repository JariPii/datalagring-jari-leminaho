using FluentValidation;

namespace SkillFlow.Presentation.Filters
{
    public sealed class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dto = context.Arguments.OfType<T>().FirstOrDefault();

           if (dto is null)
            {
                return Results.Problem(
                    title: "Invalid request body",
                    detail: "Request body is missin or could not be parsed",
                    statusCode: StatusCodes.Status400BadRequest
                    );
            }

            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            if (validator is null)
                return await next(context);

            var result = await validator.ValidateAsync(dto, context.HttpContext.RequestAborted);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary(), title: "Validation failed");

            return await next(context);
        }
    }
}
