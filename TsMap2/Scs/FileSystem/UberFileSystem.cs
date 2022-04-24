using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using TsMap2.Helper;
using TsMap2.Scs.FileSystem.Hash;
using TsMap2.Scs.FileSystem.Zip;

namespace TsMap2.Scs.FileSystem;

public class UberFileSystem {
    private static readonly Lazy< UberFileSystem > _instance = new(() => new UberFileSystem());
    public static           UberFileSystem         Instance => _instance.Value;

    internal         Dictionary< ulong, UberFile >      Files       { get; } = new();
    internal         Dictionary< ulong, UberDirectory > Directories { get; } = new();
    private readonly List< ArchiveFile >                _archiveFiles = new();

    /// <summary>
    ///     Reads and adds a single file to the filesystem
    ///     Checks if file is an SCS Hash file, if not, assumes it's a zip file
    /// </summary>
    /// <param name="path">Path for the archive file to add</param>
    /// <returns>Whether or not the file was parsed correctly</returns>
    public bool AddSourceFile( string path ) {
        if ( !File.Exists( path ) ) {
            Log.Error( $"Could not find file '{path}'" );
            return false;
        }

        var buff = new byte[ 4 ];
        using ( FileStream f = File.OpenRead( path ) ) {
            f.Seek( 0, SeekOrigin.Begin );
            f.Read( buff, 0, 4 ); // read magic bytes (first 4 bytes of file)
        }

        if ( BitConverter.ToUInt32( buff, 0 ) == ScsConst.ScsMagic ) {
            var hashFile = new HashArchiveFile( path );
            if ( hashFile.Parse() ) {
                _archiveFiles.Add( hashFile );
                return true;
            }

            Log.Error( $"Could not load hash file '{path}'" );
            return false;
        }

        var zipFile = new ZipArchiveFile( path );
        if ( zipFile.Parse() ) {
            _archiveFiles.Add( zipFile );
            return true;
        }

        Log.Error( $"Could not load zip file '{path}'" );
        return false;
    }

    /// <summary>
    ///     Adds all files from the specified directory matching the filter to the filesystem
    /// </summary>
    /// <param name="path">Path to the directory where to find the files to include</param>
    /// <param name="searchPattern">Search pattern to select specific files eg. "*.scs"</param>
    /// <returns>Whether or not all files were added successfully</returns>
    public bool AddSourceDirectory( string path, string searchPattern = "*.scs" ) {
        if ( !Directory.Exists( path ) ) {
            Log.Error( $"Could not find directory '{path}'" );
            return false;
        }

        string[] scsFilesPaths = Directory.GetFiles( path, searchPattern );

        var result = true;

        foreach ( string scsFilePath in scsFilesPaths ) {
            bool fileResult           = AddSourceFile( scsFilePath );
            if ( !fileResult ) result = false;
        }

        return result;
    }

    /// <summary>
    ///     Tries to find the directory by the given path
    /// </summary>
    /// <param name="path">Path for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    public UberDirectory GetDirectory( string path ) => GetDirectory( CityHash.CityHash64( PathHelper.EnsureLocalPath( path ) ) );

    /// <summary>
    ///     Tries to find the directory by the given hash
    /// </summary>
    /// <param name="pathHash">Hash for the wanted directory</param>
    /// <returns>
    ///     <see cref="UberDirectory" /> for the given hash
    ///     <para>Null if hash was not found</para>
    /// </returns>
    public UberDirectory GetDirectory( ulong pathHash ) {
        if ( Directories.ContainsKey( pathHash ) ) return Directories[ pathHash ];
        return null;
    }

    /// <summary>
    ///     Tries to find the file by a given path
    /// </summary>
    /// <param name="path">Path for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given path
    ///     <para>Null if path was not found</para>
    /// </returns>
    public UberFile GetFile( string path ) {
        UberFile file = GetFile( CityHash.CityHash64( PathHelper.EnsureLocalPath( path ) ) );
        file.Path = path;

        return file;
    }

    /// <summary>
    ///     Tries to find the file by a given hash
    /// </summary>
    /// <param name="pathHash">Hash for the wanted file</param>
    /// <returns>
    ///     <see cref="UberFile" /> for the given hash
    ///     <para>Null if hash was not found</para>
    /// </returns>
    public UberFile GetFile( ulong pathHash ) {
        if ( Files.ContainsKey( pathHash ) ) return Files[ pathHash ];
        return null;
    }
}