namespace Timespace.SourceGenerators.Tests;

public class GeneratePermissionPolicyTests
{
	[Fact]
	public void Test1()
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			  namespace Timespace
			  {
			  
			  [AttributeUsage(AttributeTargets.Class)]
			  public sealed class GeneratePermissionAttribute(string permissionString) : System.Attribute
			  {
			  	public string PermissionString { get; internal init; } = permissionString;
			  }
			  }
			  
			  namespace Timespace.Features.Login {
			  [Timespace.GeneratePermission("timespace:login")]
			  public static partial class LoginEndpointPolicy;
			  }
			  """, new GeneratePermissionPoliciesGenerator());

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.NotEmpty(result.GeneratedTrees);
	}
}
