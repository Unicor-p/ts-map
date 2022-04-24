using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Serilog;
using TsMap2.Helper;
using TsMap2.Model.Ts;

namespace TsMap2.Scs.FileSystem.Entry {
    public class ScsOverlayEntry : AbstractScsEntry< Dictionary< ulong, TsMapOverlay > > {
        private readonly ScsOverlayIconEntry               _overlayIconEntry = new ScsOverlayIconEntry();
        private readonly Dictionary< ulong, TsMapOverlay > _overlays         = new Dictionary< ulong, TsMapOverlay >();
        private          UberFile                          _currentMat;

        public TsMapOverlay? Get( string overlayName, ScsOverlayTypes overlayType ) {
            if ( overlayName == "" ) return null;

            string path = overlayType switch {
                              ScsOverlayTypes.Road    => $"material/ui/map/road/road_{overlayName}.mat",
                              ScsOverlayTypes.Company => $"material/ui/company/small/{overlayName}.mat",
                              ScsOverlayTypes.Map     => $"material/ui/map/{overlayName}.mat",
                              _                       => PathHelper.EnsureLocalPath( overlayName )
                          };

            ulong        hash    = CityHash.CityHash64( path );
            TsMapOverlay overlay = Store.Def.LookupOverlay( hash );

            if ( overlay != null )
                return overlay;

            UberFile matFile = UberFileSystem.Instance.GetFile( path );
            _currentMat = matFile;
            Generate( matFile.Entry.Read() );

            return _overlays.Count == 0
                       ? null
                       : _overlays.First().Value;
        }

        public Dictionary< ulong, TsMapOverlay > List() {
            var overlaysFiles = new List< string >();

            overlaysFiles.AddRange( GetOverlayFiles( ScsPath.Def.MaterialUiMapPath,     ScsPath.ScsMatExtension ) );
            overlaysFiles.AddRange( GetOverlayFiles( ScsPath.Def.MaterialUiCompanyPath, ScsPath.ScsMatExtension ) );
            overlaysFiles.AddRange( GetOverlayFiles( ScsPath.Def.MaterialUiRoadPath,    ScsPath.ScsMatExtension ) );

            foreach ( string overlayFile in overlaysFiles ) {
                UberFile matFile = UberFileSystem.Instance.GetFile( overlayFile );
                _currentMat = matFile;
                Generate( matFile.Entry.Read() );
            }

            return _overlays;
        }

        private List< string > GetOverlayFiles( string directory, string extenstion ) {
            UberDirectory  overlayDirectory = Store.Ubs.GetDirectory( directory );
            List< string > overlaysFiles    = overlayDirectory.GetFilesByExtension( directory, $".{extenstion}" );
            if ( overlaysFiles.Count == 0 ) return new List< string >();

            return overlaysFiles;
        }

        public override Dictionary< ulong, TsMapOverlay > Generate( byte[] stream ) {
            string[] lines = Encoding.UTF8.GetString( stream ).Split( '\n' );


            foreach ( string line in lines ) {
                ( bool validLine, string key, string value ) = ScsSiiHelper.ParseLine( line );
                if ( !validLine ) continue;
                if ( key != "texture" ) continue;

                string objPath = PathHelper.CombinePath( PathHelper.GetDirectoryPath( _currentMat.Path ), value.Split( '"' )[ 1 ] );

                byte[]? objData = UberFileSystem.Instance.GetFile( objPath )?.Entry?.Read();

                if ( objData == null ) break;

                string path = PathHelper.EnsureLocalPath( Encoding.UTF8.GetString( objData, 0x30, objData.Length - 0x30 ) );

                string name = PathHelper.GetFileNameFromPath( _currentMat.Path );
                if ( name.StartsWith( "map" ) ) continue;
                if ( name.StartsWith( "road_" ) ) name = name.Substring( 5 );

                ulong        token   = ScsTokenHelper.StringToToken( name );
                TsMapOverlay overlay = Parse( path, token );

                if ( overlay != null )
                    AddOverlay( overlay );
            }

            return _overlays;
        }

        private TsMapOverlay Parse( string path, ulong token ) {
            UberFile file = Store.Ubs.GetFile( path );

            if ( file != null ) {
                OverlayIcon icon = _overlayIconEntry.Get( path );

                if ( !icon.Valid ) return null;

                var overlayBitmap = new Bitmap( (int)icon.Width, (int)icon.Height, PixelFormat.Format32bppArgb );

                BitmapData bd = overlayBitmap.LockBits( new Rectangle( 0, 0, overlayBitmap.Width, overlayBitmap.Height ), ImageLockMode.WriteOnly,
                                                        PixelFormat.Format32bppArgb );

                IntPtr ptr = bd.Scan0;

                Marshal.Copy( icon.GetData(), 0, ptr, bd.Width * bd.Height * 0x4 );

                overlayBitmap.UnlockBits( bd );

                return new TsMapOverlay( overlayBitmap, token );
            }

            Log.Warning( "Map Overlay file not found, {0}", path );
            return null;
        }

        private void AddOverlay( TsMapOverlay mapOverlay ) {
            if ( !_overlays.ContainsKey( mapOverlay.Token ) ) _overlays.Add( mapOverlay.Token, mapOverlay );
        }
    }

    public class ScsOverlayIconEntry : AbstractScsEntry< OverlayIcon > {
        public override OverlayIcon Generate( byte[] stream ) {
            var overlay = new OverlayIcon();

            if ( stream.Length < 128 || MemoryHelper.ReadUInt32( stream, 0x00 ) != 0x20534444 || MemoryHelper.ReadUInt32( stream, 0x04 ) != 0x7C ) {
                Log.Error( "Invalid DDS file" );
                return null;
            }

            uint height = MemoryHelper.ReadUInt32( stream, 0x0C );
            uint width  = MemoryHelper.ReadUInt32( stream, 0x10 );

            uint        fourCc = MemoryHelper.ReadUInt32( stream, 0x54 );
            Color8888[] overlayRawData;

            if ( fourCc == 861165636 )
                overlayRawData = ScsOverlayHelper.ParseDxt3( stream, width, height );
            else if ( fourCc == 894720068 )
                overlayRawData = ScsOverlayHelper.ParseDxt5( stream, width, height );
            else
                overlayRawData = ScsOverlayHelper.ParseUncompressed( stream, width, height );

            if ( overlayRawData != null )
                overlay = new OverlayIcon( overlayRawData, width, height );

            return overlay;
        }
    }
}