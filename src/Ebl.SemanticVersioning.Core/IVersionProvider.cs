using System.Threading.Tasks;

namespace SemanticVersioning
{
	/// <summary>
	/// provide next available <see cref="SemanticVersion"/> number according to versioning strategy.
	/// </summary>
	public interface IVersionProvider
	{
		SemanticVersion GetNextPatch(string packageId);

		SemanticVersion GetNextPatch(string packageId, int major);

		SemanticVersion GetNextPatch(string packageId, int major, int minor);

		Task<SemanticVersion> GetNextPatchAsync(string packageId);

		Task<SemanticVersion> GetNextPatchAsync(string packageId, int major);

		Task<SemanticVersion> GetNextPatchAsync(string packageId, int major, int minor);
	}
}
