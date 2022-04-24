using System.IO;
using System.Text;
using Serilog;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Hash;

public class HashArchiveFile : ArchiveFile {
    /// <summary>
    ///     Hashing method used in scs files, 'CITY' as utf-8 bytes
    /// </summary>
    internal const uint HashMethod = 1498696003;

    private const ushort SupportedHashVersion = 1;
    private const ushort EntryBlockSize       = 0x20;

    private HashArchiveHeader _hashHeader;

    /// <summary>
    ///     Represents a hash archive file.
    ///     Need to run <see cref="Parse" /> after to actually read the file contents
    /// </summary>
    /// <param name="path">Path to the hash file</param>
    public HashArchiveFile( string path ) : base( path ) { }

    /// <summary>
    ///     Does minimal validation on the file and reads all the <see cref="AnEntry">Entries</see>
    /// </summary>
    /// <returns>Whether parsing was successful or not</returns>
    public override bool Parse() {
        if ( !File.Exists( _path ) ) {
            Log.Error( $"Could not find file {_path}" );
            return false;
        }

        FileStream fs = File.OpenRead( _path );
        Br = new BinaryReader( fs );

        _hashHeader = new HashArchiveHeader {
            Magic       = MemoryHelper.ReadUInt32( Br, 0x0 ),
            Version     = MemoryHelper.ReadUInt16( Br, 0x04 ),
            Salt        = MemoryHelper.ReadUInt16( Br, 0x06 ),
            HashMethod  = MemoryHelper.ReadUInt32( Br, 0x08 ),
            EntryCount  = MemoryHelper.ReadUInt32( Br, 0x0C ),
            StartOffset = MemoryHelper.ReadUInt32( Br, 0x10 )
        };

        if ( _hashHeader.Magic != ScsConst.ScsMagic ) {
            Log.Error( "Incorrect File Structure" );
            return false;
        }

        if ( _hashHeader.HashMethod != HashMethod ) {
            Log.Error( "Incorrect Hash Method" );
            return false;
        }

        if ( _hashHeader.Version != SupportedHashVersion ) {
            Log.Error( "Unsupported Hash Version" );
            return false;
        }

        Br.BaseStream.Seek( _hashHeader.StartOffset, SeekOrigin.Begin );
        byte[] entriesRaw = Br.ReadBytes( (int)_hashHeader.EntryCount * EntryBlockSize ); // read all entries at once for performance

        for ( var i = 0; i < _hashHeader.EntryCount; i++ ) {
            int offset = i * EntryBlockSize;
            var entry = new HashEntry( this ) {
                Hash           = MemoryHelper.ReadUInt64( entriesRaw, offset ),
                Offset         = MemoryHelper.ReadUInt64( entriesRaw, offset + 0x08 ),
                Flags          = MemoryHelper.ReadUInt32( entriesRaw, offset + 0x10 ),
                Crc            = MemoryHelper.ReadUInt32( entriesRaw, offset + 0x14 ),
                Size           = MemoryHelper.ReadUInt32( entriesRaw, offset + 0x18 ),
                CompressedSize = MemoryHelper.ReadUInt32( entriesRaw, offset + 0x1C )
            };

            if ( entry.IsDirectory() ) {
                UberDirectory dir = UberFileSystem.Instance.GetDirectory( entry.GetHash() );
                if ( dir == null ) {
                    dir = new UberDirectory();
                    dir.AddNewEntry( entry );
                    UberFileSystem.Instance.Directories[ entry.GetHash() ] = dir;
                }

                string[] lines = Encoding.UTF8.GetString( entry.Read() ).Split( '\n' );
                foreach ( string line in lines ) {
                    if ( line == "" ) continue;

                    if ( line.StartsWith( "*" ) ) // dir
                        dir.AddSubDirName( line.Substring( 1 ) );
                    else
                        dir.AddSubFileName( line );
                }
            } else {
                if ( UberFileSystem.Instance.Files.ContainsKey( entry.GetHash() ) )
                    UberFileSystem.Instance.Files[ entry.GetHash() ] =
                        new UberFile( entry, _path ); // overwrite if there already is a file with the current hash
                else
                    UberFileSystem.Instance.Files.Add( entry.GetHash(), new UberFile( entry, _path ) );
            }
        }

        Log.Debug( $"Mounted '{Path.GetFileName( _path )}' with {_hashHeader.EntryCount} entries" );
        return true;
    }
}