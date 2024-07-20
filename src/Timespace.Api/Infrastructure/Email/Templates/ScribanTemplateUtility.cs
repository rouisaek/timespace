using System.Reflection;
using Scriban;

namespace Timespace.Api.Infrastructure.Email.Templates;

public static class ScribanTemplateUtility
{
	public static Template GetScribanTemplate(string templateName)
	{
		using var stream = Assembly
			.GetExecutingAssembly()
			.GetManifestResourceStream(
				typeof(ScribanTemplateUtility),
				$"{templateName}.sbntxt"
			)!;

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}
}
