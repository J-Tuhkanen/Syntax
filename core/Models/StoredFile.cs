namespace Syntax.Core.Models
{
    /// <summary>
    /// A file read back from storage. The caller owns <see cref="Content"/> and must dispose it.
    /// </summary>
    public sealed record StoredFile(Stream Content, string ContentType, DateTimeOffset? LastModified);
}
