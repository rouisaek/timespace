using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Timespace.SourceGenerators;

[Generator]
public sealed class GeneratePermissionPoliciesGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var permissionStrings = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Timespace.GeneratePermissionPolicyAttribute",
				predicate: static (_, _) => true,
				transform: static (ctx, _) => new { Fqn = ctx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) })
			.Collect();

		var template = Utility.GetScribanTemplate("GeneratePermissionPolicies");
		context.RegisterSourceOutput(
			permissionStrings,
			action: (spc, c) =>
			{
				var output = template
					.Render(new { classes = c });
				spc.AddSource($"AddPermissionPolicies.g.cs", SourceText.From(output, Encoding.UTF8));
			});
	}
}
