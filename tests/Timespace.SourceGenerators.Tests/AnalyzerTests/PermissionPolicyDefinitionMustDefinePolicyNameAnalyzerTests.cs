using System.Diagnostics.CodeAnalysis;
using Timespace.Analyzers;

namespace Timespace.SourceGenerators.Tests.AnalyzerTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class PermissionPolicyDefinitionMustDefinePolicyNameAnalyzerTests
{
	[Fact]
	public async Task InvalidDefinition_ShouldRaiseDiagnostic()
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<PermissionPolicyDefinitionMustDefinePolicyNameAnalyzer>(
			$$"""
			  namespace Timespace
			  {
			  public sealed class GeneratePermissionPolicyAttribute : System.Attribute;
			  }
			  
			  namespace Timespace.Features.Login {
			  [Timespace.GeneratePermissionPolicy]
			  public static partial class {|TS0001:LoginEndpointPolicy|} {
			  
			  }
			  }
			  """
		).RunAsync();
	}

	[Fact]
	public async Task ValidDefinition_ShouldNotRaiseDiagnostic()
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<PermissionPolicyDefinitionMustDefinePolicyNameAnalyzer>(
			$$"""
			  namespace Timespace
			  {
			  public sealed class GeneratePermissionPolicyAttribute : System.Attribute;
			  }

			  namespace Timespace.Features.Login {
			  [Timespace.GeneratePermissionPolicy]
			  public static partial class LoginEndpointPolicy {
			  public const string PolicyName = "timespace:login";
			  }
			  }
			  """
		).RunAsync();
	}
}
