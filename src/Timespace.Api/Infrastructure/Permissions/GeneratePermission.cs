// ReSharper disable once CheckNamespace
namespace Timespace;

[AttributeUsage(AttributeTargets.Class)]
public sealed class GeneratePermissionAttribute(string permissionString) : Attribute
{
	public string PermissionString { get; internal init; } = permissionString;
}
