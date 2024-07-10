using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Timespace.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PermissionPolicyDefinitionMustDefinePolicyNameAnalyzerCodeFixProvider)), Shared]
public class PermissionPolicyDefinitionMustDefinePolicyNameAnalyzerCodeFixProvider : CodeFixProvider
{
	// Specify the diagnostic IDs of analyzers that are expected to be linked.
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create(DiagnosticIds.TS0001MissingPolicyNameConstField);

	public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// We link only one diagnostic and assume there is only one diagnostic in the context.
		var diagnostic = context.Diagnostics.Single();

		// 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		if (root?.FindNode(diagnosticSpan) is ClassDeclarationSyntax classDeclarationSyntax &&
			root is CompilationUnitSyntax compilationUnitSyntax)
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "Add PolicyName const string field",
					createChangedDocument: c => AddPolicyNameConstField(context.Document, compilationUnitSyntax, classDeclarationSyntax, c),
					equivalenceKey: "Add Policyname const string field"),
				diagnostic);
		}
	}

	private static Task<Document> AddPolicyNameConstField(Document document, CompilationUnitSyntax root,
		ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
	{
		// Create the attribute syntax
		var fieldDeclSyntax = FieldDeclaration(
				VariableDeclaration(
						PredefinedType(
							Token(SyntaxKind.StringKeyword)))
					.WithVariables(
						SingletonSeparatedList(
							VariableDeclarator(
									Identifier("PolicyName"))
								.WithInitializer(
									EqualsValueClause(
										LiteralExpression(
											SyntaxKind.StringLiteralExpression,
											Literal("")))))))
			.WithModifiers(
				TokenList(
				[
					Token(SyntaxKind.PublicKeyword),
					Token(SyntaxKind.ConstKeyword)
				]));

		// Add the field to the class declaration
		var newClassDecl =
			classDeclarationSyntax.AddMembers([fieldDeclSyntax]);

		cancellationToken.ThrowIfCancellationRequested();

		// Replace the old class declaration with the new one
		var newRoot = root.ReplaceNode(classDeclarationSyntax, newClassDecl);

		cancellationToken.ThrowIfCancellationRequested();

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		cancellationToken.ThrowIfCancellationRequested();

		return Task.FromResult(newDocument);
	}
}
