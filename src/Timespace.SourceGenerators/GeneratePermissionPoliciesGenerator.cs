using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Timespace.SourceGenerators;

[Generator]
public sealed class GeneratePermissionPoliciesGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var permissionStrings = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Timespace.GeneratePermissionAttribute",
				predicate: static (_, _) => true,
				transform: static (ctx, token) =>
				{
					var @class = (ClassDeclarationSyntax)ctx.TargetNode;
					var name = ctx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

					var attr = ctx.Attributes[0];
					var permissionString = (string?)attr.ConstructorArguments
										  .FirstOrDefault()
										  .Value
									  ?? @class.Identifier.Text;

					var symbolInfo = (INamedTypeSymbol)ctx.SemanticModel.GetDeclaredSymbol(@class, token)!;

					var @namespace = symbolInfo.ContainingNamespace.ToString();

					return new { Fqn = ctx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), Name = name, PermissionString = permissionString, Namespace = @namespace == "<global namespace>" ? "Timespace" : @namespace };
				})
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

		var classTemplate = Utility.GetScribanTemplate("GeneratePermissionPolicyClass");
		context.RegisterSourceOutput(
			permissionStrings,
			action: (spc, c) =>
			{
				foreach (var permissionPolicy in c)
				{
					var output = classTemplate
						.Render(permissionPolicy);
					spc.AddSource($"PermissionPolicyClasses.{permissionPolicy.Namespace}.{permissionPolicy.Name}.g.cs", SourceText.From(output, Encoding.UTF8));
				}
			});
	}
}
