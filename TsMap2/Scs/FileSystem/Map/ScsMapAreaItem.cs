using System.Collections.Generic;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Map;

public class ScsMapAreaItem : ScsMapItem {
    public ScsMapAreaItem( ScsSector sector ) : base( sector, sector.LastOffset ) {
        Valid = true;
        TsMapAreaItem825();
    }

    public List< ulong > NodeUids   { get; private set; }
    public uint          ColorIndex { get; private set; }
    public bool          DrawOver   { get; private set; }

    private void TsMapAreaItem825() {
        int fileOffset = Sector.LastOffset + 0x34; // Set position at start of flags

        DrawOver = MemoryHelper.ReadUint8( Sector.Stream, fileOffset ) != 0;
        int dlcGuardCount = Store().Game.IsEts2()
                                ? ScsConst.Ets2DlcGuardCount
                                : ScsConst.AtsDlcGuardCount;
        Hidden   = MemoryHelper.ReadInt8( Sector.Stream, fileOffset + 0x01 ) > dlcGuardCount;
        IsSecret = MemoryHelper.IsBitSet( MemoryHelper.ReadUint8( Sector.Stream, fileOffset ), 4 );

        NodeUids = new List< ulong >();

        int nodeCount = MemoryHelper.ReadInt32( Sector.Stream, fileOffset += 0x05 ); // 0x05(flags)
        fileOffset += 0x04;                                                          // 0x04(nodeCount)
        for ( var i = 0; i < nodeCount; i++ ) {
            NodeUids.Add( MemoryHelper.ReadUInt64( Sector.Stream, fileOffset ) );
            fileOffset += 0x08;
        }

        ColorIndex =  MemoryHelper.ReadUInt32( Sector.Stream, fileOffset );
        fileOffset += 0x04; // 0x04(colorIndex)
        BlockSize  =  fileOffset - Sector.LastOffset;
    }
}