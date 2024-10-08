using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Timespace.SourceGenerators.Tests.CodeFixTests;

public static class CodeFixTestHelper
{
	private const string EditorConfig =
		"""
	    root = true

	    [*.cs]
	    charset = utf-8
	    indent_style = tab
	    insert_final_newline = true
	    indent_size = 4
	    """;

	public static CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> CreateCodeFixTest<TAnalyzer, TCodeFix>(
		string inputSource,
		string fixedSource,
		int codeActionIndex = 0
	)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
	{
		var csTest = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
		{
			CodeActionIndex = codeActionIndex,
			TestState =
			{
				Sources = { inputSource },
				AnalyzerConfigFiles = { { ("/.editorconfig", EditorConfig) } },
				ReferenceAssemblies = new ReferenceAssemblies(
					"net8.0",
					new PackageIdentity(
						"Microsoft.NETCore.App.Ref",
						"8.0.0"),
					Path.Combine("ref", "net8.0"))
			},
			FixedState = { MarkupHandling = MarkupMode.IgnoreFixable, Sources = { fixedSource } }
		};

		return csTest;
	}
}
