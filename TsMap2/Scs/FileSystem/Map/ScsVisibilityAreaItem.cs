using System.IO;
using Serilog;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Map;

public class ScsVisibilityAreaItem : ScsMapItem {
    public ScsVisibilityAreaItem( ScsSector sector ) : base( sector, sector.LastOffset ) {
        Valid = false;

        if ( Sector.Version >= 891 )
            TsVisibilityAreaItem891( sector.LastOffset );
        else
            Log.Warning(
                        $"Unknown base file version ({Sector.Version}) for item {Type} in file '{Path.GetFileName( Sector.FilePath )}' @ {sector.LastOffset}." );
    }

    public void TsVisibilityAreaItem891( int startOffset ) {
        int fileOffset    = startOffset + 0x34;                                                 // Set position at start of flags
        int childrenCount = MemoryHelper.ReadInt32( Sector.Stream, fileOffset += 0x05 + 0x10 ); // 0x05(flags) + 0x10(node_uid, width, height)
        fileOffset += 0x04       + 0x08 * childrenCount;                                        // 0x04(childrenCount) + childrenUids
        BlockSize  =  fileOffset - startOffset;
    }
}