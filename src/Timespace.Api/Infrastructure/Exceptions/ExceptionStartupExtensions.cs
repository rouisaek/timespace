using System.Text.Json;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Timespace.Api.Infrastructure.Exceptions;

public static partial class ExceptionStartupExtensions
{
	public static void ConfigureProblemDetails(ProblemDetailsOptions options) =>
		options.CustomizeProblemDetails = c =>
		{
			if (c.Exception is null)
				return;

			var logger = c.HttpContext.RequestServices.GetRequiredService<ILogger<ProblemDetails>>();

			c.ProblemDetails = c.Exception switch
			{
				ValidationException ex => new ValidationProblemDetails(
					ex
						.Errors
						.GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
						.ToDictionary(
							x => x.Key,
							x => x.Select(y => y.ErrorMessage).ToArray(),
							StringComparer.OrdinalIgnoreCase
						)
				)
				{
					Type = "validation",
					Status = StatusCodes.Status400BadRequest,
				},

				BadHttpRequestException { InnerException: JsonException ex }
					when ex.Message.StartsWith("JSON deserialization for type", StringComparison.InvariantCultureIgnoreCase) => new ProblemDetails
					{
						Type = "json-validation",
						Detail = $"Missing the following properties: " + ex.Message.Split("following: ")[1],
						Status = StatusCodes.Status400BadRequest
					},

				BadHttpRequestException { InnerException: JsonException ex } => new ProblemDetails()
				{
					Type = "json-validation",
					Detail = ex.Message,
					Status = StatusCodes.Status400BadRequest
				},

				TimespaceException ex => new ProblemDetails
				{
					Type = ex.ErrorType,
					Detail = ex.Message,
					Status = ex.StatusCode
				},

				_ => new ProblemDetails
				{
					Type = "internal-server-error",
					Detail = "An error has occurred. Please contact us with the value of the 'RequestId' header",
					Status = StatusCodes.Status500InternalServerError,
				},
			};

			if (c.ProblemDetails.Status == StatusCodes.Status500InternalServerError)
			{
				logger.LogUnhandledException(c.Exception);
			}

			c.HttpContext.Response.Headers.Append("RequestId", c.HttpContext.TraceIdentifier);

			c.HttpContext.Response.StatusCode =
				c.ProblemDetails.Status
				?? StatusCodes.Status500InternalServerError;
		};

	[LoggerMessage(LogLevel.Error, "Unhandled exception. {Ex}")]
	private static partial void LogUnhandledException(this ILogger logger, Exception ex);
}
