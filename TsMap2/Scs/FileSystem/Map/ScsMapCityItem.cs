using System.IO;
using Serilog;
using TsMap2.Helper;
using TsMap2.Model.Ts;

namespace TsMap2.Scs.FileSystem.Map;

public class ScsMapCityItem : ScsMapItem // TODO: Add zoom levels/range to show city names and icons correctly
{
    public ScsMapCityItem( ScsSector sector ) : base( sector, sector.LastOffset ) {
        Valid = true;
        TsCityItem825();
    }

    public TsCity City    { get; private set; }
    public ulong  NodeUid { get; private set; }
    public float  Width   { get; private set; }
    public float  Height  { get; private set; }

    private void TsCityItem825() {
        int fileOffset = Sector.LastOffset + 0x34; // Set position at start of flags

        Hidden = ( MemoryHelper.ReadUint8( Sector.Stream, fileOffset ) & 0x01 ) != 0;
        ulong cityId = MemoryHelper.ReadUInt64( Sector.Stream, fileOffset + 0x05 );
        City = Store().Def.LookupCity( cityId );
        if ( City == null ) {
            Valid = false;
            Log.Error( $"Could not find City: '{ScsTokenHelper.TokenToString( cityId )}'({cityId:X}) item uid: 0x{Uid:X}, "
                       + $"in {Path.GetFileName( Sector.FilePath )} @ {fileOffset} from '{Sector.ArchivePath}'" );
        }

        Width      =  MemoryHelper.ReadSingle( Sector.Stream, fileOffset += 0x05 + 0x08 ); // 0x05(flags) + 0x08(cityId)
        Height     =  MemoryHelper.ReadSingle( Sector.Stream, fileOffset += 0x04 );        // 0x08(Width)
        NodeUid    =  MemoryHelper.ReadUInt64( Sector.Stream, fileOffset += 0x04 );        // 0x08(height)
        fileOffset += 0x08;                                                                // nodeUid
        BlockSize  =  fileOffset - Sector.LastOffset;
    }

    public override string ToString() {
        if ( City == null ) return "Error";
        TsCountry country = Store().Def.GetCountryByTokenName( City.CountryName );
        string countryName = country == null
                                 ? City.CountryName
                                 : Store().Localization.GetLocaleValue( country.LocalizationToken ) ?? country.Name;
        return $"{countryName} - {Store().Localization.GetLocaleValue( City.LocalizationToken ) ?? City.Name}";
    }
}