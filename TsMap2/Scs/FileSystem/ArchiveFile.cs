using System.IO;

namespace TsMap2.Scs.FileSystem;

/// <summary>
///     Represents an archive file (*.scs, *.zip)
/// </summary>
public abstract class ArchiveFile {
    protected readonly string _path;

    public BinaryReader Br { get; protected set; }

    public ArchiveFile( string path ) => _path = path;

    public abstract bool Parse();

    public string GetPath() => _path;

    ~ArchiveFile() {
        if ( Br == null ) return;
        Br.Close();
        Br = null;
    }
}