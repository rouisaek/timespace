using System.Text.Json;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;
using ProblemDetailsOptions = Microsoft.AspNetCore.Http.ProblemDetailsOptions;

namespace Timespace.Api.Infrastructure.Startup;

public static class StartupExtensions
{
	public static void AddConfiguration(this IServiceCollection services, ConfigurationManager configurationManager)
	{

	}

	public static void ConfigureImmediatePlatform(this IServiceCollection services)
	{
		_ = services.AddHandlers();
		_ = services.AddBehaviors();
	}

	public static void AddIdentity(this IServiceCollection services)
	{
		_ = services.AddIdentityCore<ApplicationUser>(config =>
			{
				config.SignIn.RequireConfirmedEmail = true;
				config.User.RequireUniqueEmail = true;
				config.Password.RequiredLength = 6;
				config.Password.RequiredUniqueChars = 0;
				config.Password.RequireLowercase = false;
				config.Password.RequireUppercase = false;
				config.Password.RequireDigit = false;
				config.Password.RequireNonAlphanumeric = false;
			})
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();

		_ = services.Configure<CookieAuthenticationOptions>(o =>
		{
			o.LoginPath = PathString.Empty;
		});

		_ = services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
			options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
			options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
		}).AddCookie(IdentityConstants.ApplicationScheme, o =>
		{
			o.Cookie.Name = "timespace";
			o.Cookie.HttpOnly = true;
			o.ExpireTimeSpan = TimeSpan.FromDays(7);
			o.Events.OnRedirectToLogin = _ => throw new UnauthorizedException();
		});

		_ = services.AddAuthorization(options =>
		{
			options.DefaultPolicy = new AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
				.RequireAuthenticatedUser()
				.Build();

			options.FallbackPolicy = new AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
				.RequireAuthenticatedUser()
				.Build();
		});

		services.TryAddScoped<IUserValidator<ApplicationUser>, UserValidator<ApplicationUser>>();
		services.TryAddScoped<IPasswordValidator<ApplicationUser>, PasswordValidator<ApplicationUser>>();
		services.TryAddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
		services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
		services.TryAddScoped<IdentityErrorDescriber>();
		services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();
		services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<ApplicationUser>>();
		services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser>>();
		services.TryAddScoped<IUserConfirmation<ApplicationUser>, DefaultUserConfirmation<ApplicationUser>>();
		services.TryAddScoped<UserManager<ApplicationUser>>();
		services.TryAddScoped<SignInManager<ApplicationUser>>();
	}

	public static IServiceCollection AddSwagger(this IServiceCollection services) =>
		services.AddSwaggerGen(o =>
		{
			o.CustomSchemaIds(t => t.FullName?.Replace('+', '.'));

			o.DocInclusionPredicate((_, api) =>
				api.ActionDescriptor
						.EndpointMetadata
						.OfType<IRouteDiagnosticsMetadata>()
						.FirstOrDefault()
					is { Route: var route }
				&& route.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
			);

			o.TagActionsBy(api =>
			{
				var routeMetadata = api.ActionDescriptor
					.EndpointMetadata
					.OfType<IRouteDiagnosticsMetadata>()
					.FirstOrDefault();

				if (routeMetadata is not { Route: var route })
					throw new InvalidOperationException("Unable to determine tag for endpoint.");

				var splits = route["/api/".Length..].Split('/');
				if (splits is not [{ } tag, ..]
					|| string.IsNullOrWhiteSpace(tag))
				{
					throw new InvalidOperationException("Unable to determine tag for endpoint.");
				}

				return [tag[..1].ToUpperInvariant() + tag[1..]];
			});
		});

	public static void ConfigureProblemDetails(ProblemDetailsOptions options) =>
		options.CustomizeProblemDetails = c =>
		{
			if (c.Exception is null)
				return;

			c.HttpContext.Response.Headers.Append("RequestId", c.HttpContext.TraceIdentifier);

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
					Status = StatusCodes.Status400BadRequest,
				},

				BadHttpRequestException { InnerException: JsonException ex }
					when ex.Message.StartsWith("JSON deserialization for type", StringComparison.InvariantCultureIgnoreCase) => new ProblemDetails
					{
						Detail = $"Missing the following properties: " + ex.Message.Split("following: ")[1],
						Status = StatusCodes.Status400BadRequest
					},

				BadHttpRequestException { InnerException: JsonException ex } => new ProblemDetails()
				{
					Detail = ex.Message,
					Status = StatusCodes.Status400BadRequest
				},

				TimespaceException ex => new ProblemDetails
				{
					Detail = ex.Message,
					Status = ex.StatusCode
				},

				_ => new ProblemDetails
				{
					Detail = "An error has occurred. Please contact us with the value of the 'RequestId' header",
					Status = StatusCodes.Status500InternalServerError,
				},
			};

			c.HttpContext.Response.StatusCode =
				c.ProblemDetails.Status
				?? StatusCodes.Status500InternalServerError;
		};

}
