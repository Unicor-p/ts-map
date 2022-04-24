using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TsMap2.Exceptions;
using TsMap2.Helper;
using TsMap2.Model.Ts;

namespace TsMap2.Scs.FileSystem.Entry {
    public class ScsCityEntry : AbstractScsEntry< TsCity > {
        public Dictionary< ulong, TsCity > List() {
            UberDirectory defDirectory = Store.Ubs.GetDirectory( ScsPath.Def.DefFolderName );
            if ( defDirectory == null ) {
                var message = $"[{MethodBase.GetCurrentMethod()?.Name}] Could not read '{ScsPath.Def.DefFolderName}' dir";
                throw new ScsEntryException( message );
            }

            List< string > cityFilesName = defDirectory.GetFiles( ScsPath.Def.CityFileName );
            if ( cityFilesName == null ) {
                var message = $"[{MethodBase.GetCurrentMethod()?.Name}] Could not read {ScsPath.Def.CityFileName} files";
                throw new ScsEntryException( message );
            }

            var cities = new Dictionary< ulong, TsCity >();
            foreach ( string cityFileName in cityFilesName ) {
                UberFile cityFile = UberFileSystem.Instance.GetFile( $"def/{cityFileName}" );

                byte[]   data  = cityFile.Entry.Read();
                string[] lines = Encoding.UTF8.GetString( data ).Split( '\n' );
                foreach ( string line in lines ) {
                    if ( line.TrimStart().StartsWith( "#" ) ) continue;
                    if ( !line.Contains( "@include" ) ) continue;

                    string path = PathHelper.GetFilePath( line.Split( '"' )[ 1 ], "def" );
                    TsCity city = Get( path );

                    if ( city != null && city.Token != 0 && !cities.ContainsKey( city.Token ) )
                        cities.Add( city.Token, city );
                }
            }

            return cities;
        }

        public override TsCity Generate( byte[] stream ) {
            string[] lines             = Encoding.UTF8.GetString( stream ).Split( '\n' );
            var      offsetCount       = 0;
            var      xOffsets          = new List< int >();
            var      yOffsets          = new List< int >();
            ulong    token             = 0;
            string   name              = null;
            string   country           = null;
            string   localizationToken = null;

            foreach ( string line in lines ) {
                ( bool validLine, string key, string value ) = ScsSiiHelper.ParseLine( line );
                if ( !validLine ) continue;

                if ( key == "city_data" )
                    token = ScsTokenHelper.StringToToken( ScsSiiHelper.Trim( value.Split( '.' )[ 1 ] ) );
                else if ( key == "city_name" )
                    name = line.Split( '"' )[ 1 ];
                else if ( key == "city_name_localized" )
                    localizationToken = value.Split( '"' )[ 1 ].Replace( "@", "" );
                else if ( key == "country" )
                    country = value;
                else if ( key.Contains( "map_x_offsets[]" ) ) {
                    if ( ++offsetCount > 4 )
                        if ( int.TryParse( value, out int offset ) )
                            xOffsets.Add( offset );

                    if ( offsetCount == 8 ) offsetCount = 0;
                } else if ( key.Contains( "map_y_offsets[]" ) )
                    if ( ++offsetCount > 4 )
                        if ( int.TryParse( value, out int offset ) )
                            yOffsets.Add( offset );
            }

            return new TsCity( name, country, token, localizationToken, xOffsets, yOffsets );
        }
    }
}