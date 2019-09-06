using System.Threading;
using System.Threading.Tasks;

namespace CodeTools.SemanticVersioning
{
    /// <summary>
    /// provide next available <see cref="SemanticVersion"/> number according to versioning strategy.
    /// </summary>
    public interface IVersionProvider
    {
        /// <summary>
        /// return the next patch version
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        SemanticVersion GetNextVersion(string packageId);

        /// <summary>
        /// return the next patch version with major matching <paramref name="major"/>
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="major"></param>
        /// <returns></returns>
        SemanticVersion GetNextVersion(string packageId, int major);

        /// <summary>
        /// return the next patch version matching both <paramref name="major"/> and <paramref name="minor"/>
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <returns></returns>
        SemanticVersion GetNextVersion(string packageId, int major, int minor);
    }

    public interface IVersionAsyncProvider
    {
        /// <summary>
        /// return the next patch version
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SemanticVersion> GetNextVersionAsync(string packageId, CancellationToken cancellationToken);

        /// <summary>
        /// return the next patch version with major matching <paramref name="major"/>
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="major"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SemanticVersion> GetNextVersionAsync(string packageId, int major, CancellationToken cancellationToken);

        /// <summary>
        /// return the next patch version matching both <paramref name="major"/> and <paramref name="minor"/>
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SemanticVersion> GetNextVersionAsync(string packageId, int major, int minor, CancellationToken cancellationToken);
    }
}
