using System.IO;
using Serilog;
using TsMap2.Helper;
using TsMap2.Model.Ts;

namespace TsMap2.Scs.FileSystem.Map;

public class ScsMapMapOverlayItem : ScsMapItem {
    public ScsMapMapOverlayItem( ScsSector sector ) : base( sector, sector.LastOffset ) {
        Valid = true;
        TsMapOverlayItem825();
    }

    public string       OverlayName         { get; private set; }
    public TsMapOverlay Overlay             { get; private set; }
    public byte         ZoomLevelVisibility { get; private set; }

    private void TsMapOverlayItem825() {
        int fileOffset = Sector.LastOffset + 0x34; // Set position at start of flags
        ZoomLevelVisibility = MemoryHelper.ReadUint8( Sector.Stream, fileOffset );
        int dlcGuardCount = Store().Game.IsEts2()
                                ? ScsConst.Ets2DlcGuardCount
                                : ScsConst.AtsDlcGuardCount;
        Hidden   = MemoryHelper.ReadInt8( Sector.Stream, fileOffset + 0x01 ) > dlcGuardCount || ZoomLevelVisibility == 255;
        IsSecret = MemoryHelper.IsBitSet( MemoryHelper.ReadUint8( Sector.Stream, fileOffset + 0x02 ), 3 );

        byte  type         = MemoryHelper.ReadUint8( Sector.Stream, fileOffset + 0x02 );
        ulong overlayToken = MemoryHelper.ReadUInt64( Sector.Stream, fileOffset += 0x05 );
        if ( type == 1 && overlayToken == 0 ) {
            overlayToken = ScsTokenHelper.StringToToken( "parking_ico" ); // parking
            Overlay      = Store().Def.GetOverlay( "parking_ico", ScsOverlayTypes.Map );
        } else
            Overlay = Store().Def.GetOverlay( ScsTokenHelper.TokenToString( overlayToken ), ScsOverlayTypes.Road );

        OverlayName = ScsTokenHelper.TokenToString( overlayToken );
        if ( Overlay == null ) {
            Valid = false;
            if ( overlayToken != 0 )
                if ( overlayToken != 0 )
                    Log.Error( $"Could not find Overlay: '{OverlayName}'({ScsTokenHelper.StringToToken( OverlayName ):X}), item uid: 0x{Uid:X}, "
                               + $"in {Path.GetFileName( Sector.FilePath )} @ {fileOffset} from '{Sector.ArchivePath}'" );
        }

        fileOffset += 0x08       + 0x08; // 0x08(overlayId) + 0x08(nodeUid)
        BlockSize  =  fileOffset - Sector.LastOffset;
    }
}