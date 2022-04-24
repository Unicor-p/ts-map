using System.IO;
using System.Linq;
using Serilog;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Zip;

public class ZipArchiveFile : ArchiveFile {
    private readonly byte[] _eocdSignature = { 0x50, 0x4b, 0x05, 0x06 };

    private int GetEndOfCentralDirectory() {
        byte[] data;
        if ( Br.BaseStream.Length < 1001 )
            data = Br.ReadBytes( (int)Br.BaseStream.Length );
        else {
            Br.BaseStream.Position = Br.BaseStream.Length - 1000;
            data                   = Br.ReadBytes( 1000 );
        }

        for ( int i = data.Length - 5; i >= 0; --i )
            if ( data.Skip( i ).Take( _eocdSignature.Length ).SequenceEqual( _eocdSignature ) )
                return data.Length - i;

        return 0;
    }

    public ZipArchiveFile( string path ) : base( path ) { }

    public override bool Parse() {
        if ( !File.Exists( _path ) ) {
            Log.Error( $"Could not find file {_path}" );
            return false;
        }

        FileStream fs = File.OpenRead( _path );
        Br = new BinaryReader( fs );

        long eocdOffset = Br.BaseStream.Length - GetEndOfCentralDirectory();

        if ( eocdOffset == Br.BaseStream.Length ) {
            Log.Error( $"Could not find End of Central Directory in zip file, '{_path}'" );
            return false;
        }

        ushort entryCount = MemoryHelper.ReadUInt16( Br, eocdOffset + 10 );
        uint   cdOffset   = MemoryHelper.ReadUInt32( Br, eocdOffset + 16 );

        uint fileOffset = cdOffset;

        for ( var i = 0; i < entryCount; i++ ) {
            var entry = new ZipEntry( this ) {
                CompressedSize = MemoryHelper.ReadUInt32( Br, fileOffset += 0x14 ),
                Size           = MemoryHelper.ReadUInt32( Br, fileOffset += 0x04 ) // 0x04(CompressedSize)
            };

            ushort nameLen = MemoryHelper.ReadUInt16( Br, fileOffset += 0x04 ); // 0x04(Size)

            ushort extraFieldLength = MemoryHelper.ReadUInt16( Br, fileOffset += 0x02 ); // 0x02(nameLen)
            ushort fileCommentLength = MemoryHelper.ReadUInt16( Br, fileOffset += 0x02 ); // 0x02(extraFieldLength)
            uint offsetLocalHeader =
                MemoryHelper.ReadUInt32( Br, fileOffset += 0x02 + 0x08 ); // 0x02(fileCommentLength) + 0x08(deDiskNumberStart + deInternalAttributes + deExternalAttributes)

            string name                      = MemoryHelper.ReadString( Br, fileOffset += 0x04, nameLen );
            if ( name.EndsWith( "/" ) ) name = name.Substring( 0, name.Length - 1 );
            entry.Hash = CityHash.CityHash64( name );

            fileOffset += (uint)nameLen + extraFieldLength + fileCommentLength;

            if ( entry.Size != 0 ) {
                uint prevOffset = fileOffset;

                fileOffset = offsetLocalHeader + 0x1A; // offset to name length

                ushort localNameLength = MemoryHelper.ReadUInt16( Br, fileOffset );

                if ( nameLen != localNameLength ) Log.Debug( $"Local name length is different than CD one for zip entry {i} '{name}' in '{_path}'" );

                ushort localExtraField = MemoryHelper.ReadUInt16( Br, fileOffset += 0x02 ); // 0x02(localNameLength)

                entry.Offset = fileOffset += 0x02u + nameLen + localExtraField; // Offset to data

                fileOffset = prevOffset;
            }

            string parentDirPath = Path.GetDirectoryName( name ).Replace( '\\', '/' );
            ulong  parentDirHash = CityHash.CityHash64( parentDirPath );

            UberDirectory parentDir;

            if ( UberFileSystem.Instance.Directories.ContainsKey( parentDirHash ) )
                parentDir = UberFileSystem.Instance.Directories[ parentDirHash ];
            else {
                parentDir = new UberDirectory();
                UberFileSystem.Instance.Directories.Add( parentDirHash, parentDir );
            }

            if ( entry.IsDirectory() ) {
                if ( UberFileSystem.Instance.Directories.ContainsKey( entry.GetHash() ) ) {
                    UberDirectory dirEntry = UberFileSystem.Instance.Directories[ entry.GetHash() ];
                    dirEntry.AddNewEntry( entry );
                } else {
                    var dir = new UberDirectory();
                    dir.AddNewEntry( entry );
                    UberFileSystem.Instance.Directories.Add( entry.GetHash(), dir );
                }

                parentDir.AddSubDirName( Path.GetFileName( name ) );
            } else {
                if ( UberFileSystem.Instance.Files.ContainsKey( entry.Hash ) )
                    UberFileSystem.Instance.Files[ entry.Hash ] = new UberFile( entry, _path );
                else {
                    parentDir.AddSubFileName( Path.GetFileName( name ) );
                    UberFileSystem.Instance.Files.Add( entry.Hash, new UberFile( entry, _path ) );
                }
            }
        }

        Log.Debug( $"Mounted '{Path.GetFileName( _path )}' with {entryCount} entries" );
        return true;
    }
}