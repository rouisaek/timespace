using System.Security.Cryptography;

namespace Timespace.Api.Features.Shared.Helpers;

public static class SecureRandomStringGenerator
{
	public static string Generate(int length = 32)
	{
		var allowed = "ABCDEFGHIJKLMONOPQRSTUVWXYZabcdefghijklmonopqrstuvwxyz0123456789";
		var randomChars = new char[length];

		for (int i = 0; i < length; i++)
		{
			randomChars[i] = allowed[RandomNumberGenerator.GetInt32(0, allowed.Length)];
		}

		return new string(randomChars);
	}
}
