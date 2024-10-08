using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Timespace.SourceGenerators;

[Generator]
public sealed class ConfigureOptionsGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var classes = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Timespace.ConfigureOptionsAttribute",
				predicate: static (_, _) => true,
				transform: static (ctx, _) =>
				{
					var @class = (ClassDeclarationSyntax)ctx.TargetNode;
					var name = ctx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

					var attr = ctx.Attributes[0];
					var sectionPath = (string?)attr.NamedArguments
										  .FirstOrDefault(a => a.Key == "SectionName")
										  .Value.Value
									  ?? @class.Identifier.Text;

					return new { Name = name, Path = sectionPath };
				})
			.Collect();

		var template = Utility.GetScribanTemplate("ConfigureOptions");
		context.RegisterSourceOutput(
			classes,
			action: (spc, c) =>
			{
				var output = template
					.Render(new { classes = c });
				spc.AddSource($"ConfigureOptions.g.cs", SourceText.From(output, Encoding.UTF8));
			});
	}
}
