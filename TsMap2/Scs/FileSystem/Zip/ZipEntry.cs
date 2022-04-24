using System.IO;
using System.IO.Compression;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Zip;

public class ZipEntry : AnEntry {
    public ZipEntry( ZipArchiveFile fsFile ) : base( fsFile ) { }

    public override byte[] Read() {
        byte[] buff = MemoryHelper.ReadBytes( GetArchiveFile().Br, (long)Offset, (int)CompressedSize );
        return IsCompressed()
                   ? Inflate( buff )
                   : buff;
    }

    protected override byte[] Inflate( byte[] buff ) {
        var inflatedBytes = new byte[ Size ];
        using ( var ms = new MemoryStream( buff ) )
        using ( var ds = new DeflateStream( ms, CompressionMode.Decompress ) ) {
            ds.Read( inflatedBytes, 0, (int)Size );

            return inflatedBytes;
        }
    }

    public override bool IsDirectory() => CompressedSize == 0;

    public override bool IsCompressed() => CompressedSize != Size;
}