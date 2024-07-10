using System.Diagnostics.CodeAnalysis;
using Timespace.Analyzers;

namespace Timespace.SourceGenerators.Tests.CodeFixTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class PermissionPolicyDefinitionMustDefinePolicyNameAnalyzerCodeFixProviderTests
{
	[Fact]
	public async Task ValidDefinition_ShouldAddPolicyNameAttribute()
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<PermissionPolicyDefinitionMustDefinePolicyNameAnalyzer, PermissionPolicyDefinitionMustDefinePolicyNameAnalyzerCodeFixProvider>(
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
				  """,
				$$"""
				  namespace Timespace
				  {
				  public sealed class GeneratePermissionPolicyAttribute : System.Attribute;
				  }
				  
				  namespace Timespace.Features.Login {
				  [Timespace.GeneratePermissionPolicy]
				  public static partial class LoginEndpointPolicy {
				  		public const string PolicyName = "";
				  	}
				  }
				  """
			).RunAsync();
	}
}
