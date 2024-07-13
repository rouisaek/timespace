using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Infrastructure.Startup;

public static class StartupExtensions
{
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
			o.AccessDeniedPath = PathString.Empty;
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
			o.Events.OnRedirectToAccessDenied = _ => throw new ForbiddenException();
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

}
