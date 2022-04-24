namespace TsMap2.Scs.FileSystem;

/// <summary>
///     Represents a file in the filesystem
///     <para>
///         Currently nothing more than just an <see cref="AnEntry" /> with how the workings of the filesystem changed,
///         just keeping this in case I want to add something specific to files
///     </para>
///     <para>Is unaware of it's own location/path</para>
/// </summary>
public class UberFile {
    public AnEntry Entry { get; }
    public string  Path;

    public UberFile( AnEntry entry, string path ) {
        Entry = entry;
        Path  = path;
    }
}