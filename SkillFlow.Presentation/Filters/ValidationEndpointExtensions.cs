namespace SkillFlow.Presentation.Filters
{
    public static class ValidationEndpointExtensions
    {
        public static RouteHandlerBuilder ValidateBody<T>(this RouteHandlerBuilder builder) =>
            builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}
