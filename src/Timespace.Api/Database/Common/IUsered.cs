using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Database.Common;

public interface IUsered
{
	public int UserId { get; set; }
	public ApplicationUser User { get; set; }
}
