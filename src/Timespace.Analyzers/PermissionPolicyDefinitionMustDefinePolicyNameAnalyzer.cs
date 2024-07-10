using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Timespace.Analyzers;

/// <summary>
/// A sample analyzer that reports the company name being used in class declarations.
/// Traverses through the Syntax Tree and checks the name (identifier) of each class node.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PermissionPolicyDefinitionMustDefinePolicyNameAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = DiagnosticIds.TS0001MissingPolicyNameConstField;

	// The category of the diagnostic (Design, Naming etc.).
	private const string Category = "Design";

	private static readonly DiagnosticDescriptor PermissionPolicyMustDefinePolicyName = new(DiagnosticId, "PermissionPolicy definitions must define a PolicyName const field", "Missing PolicyName field in the class", Category,
		DiagnosticSeverity.Error, isEnabledByDefault: true, description: "PermissionPolicy definitions must define a PolicyName const field.");

	// Keep in mind: you have to list your rules here.
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(PermissionPolicyMustDefinePolicyName);

	public override void Initialize(AnalysisContext context)
	{
		// You must call this method to avoid analyzing generated code.
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

		// You must call this method to enable the Concurrent Execution.
		context.EnableConcurrentExecution();

		// Subscribe to the Syntax Node with the appropriate 'SyntaxKind' (ClassDeclaration) action.
		// To figure out which Syntax Nodes you should choose, consider installing the Roslyn syntax tree viewer plugin Rossynt: https://plugins.jetbrains.com/plugin/16902-rossynt/
		// context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);

		context.RegisterSymbolAction(AnalyzeSyntax, SymbolKind.NamedType);

		// Check other 'context.Register...' methods that might be helpful for your purposes.
	}

	/// <summary>
	/// Executed for each Syntax Node with 'SyntaxKind' is 'ClassDeclaration'.
	/// </summary>
	/// <param name="context">Operation context.</param>
	private void AnalyzeSyntax(SymbolAnalysisContext context)
	{
		// The Roslyn architecture is based on inheritance.
		// To get the required metadata, we should match the 'Node' object to the particular type: 'ClassDeclarationSyntax'.
		if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
			return;

		if (!HasAttribute(namedTypeSymbol, "GeneratePermissionPolicyAttribute"))
			return;

		var hasCorrectField = namedTypeSymbol.GetMembers()
			.OfType<IFieldSymbol>()
			.Any(x => x.IsConst &&
						x.DeclaredAccessibility == Accessibility.Public &&
						x.Type.ToDisplayString() == "string" &&
						x.Name == "PolicyName");

		if (!hasCorrectField)
			context.ReportDiagnostic(Diagnostic.Create(PermissionPolicyMustDefinePolicyName, namedTypeSymbol.Locations[0]));
	}

	private static bool HasAttribute(INamedTypeSymbol classSymbol, string attributeName)
	{
		return classSymbol.GetAttributes()
			.Any(attr => attr.AttributeClass?.Name == attributeName);
	}
}
