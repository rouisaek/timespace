using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Timespace.SourceGenerators.Tests;

public static class GeneratorTestHelper
{
	public static GeneratorDriver GetDriver(string source, IIncrementalGenerator generator)
	{
		// Parse the provided string into a C# syntax tree
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		// Create a Roslyn compilation for the syntax tree.
		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.Net80.References.All,
				// MetadataReference.CreateFromFile("./Timespace.Api.dll")
			]
		);

		// The GeneratorDriver is used to run our generator against a compilation
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

		// Run the source generator!
		return driver.RunGenerators(compilation);
	}
}
