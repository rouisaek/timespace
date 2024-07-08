using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Timespace.Api.Infrastructure.Logging;

[assembly: Behaviors(
	typeof(LoggingBehavior<,>),
	typeof(ValidationBehavior<,>)
)]
