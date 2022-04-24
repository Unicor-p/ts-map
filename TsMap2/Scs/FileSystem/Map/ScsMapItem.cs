using System.Collections.Generic;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Map;

public class ScsMapItem {
    protected const int VegetationSphereBlockSize    = 0x14;
    protected const int VegetationSphereBlockSize825 = 0x10;

    protected readonly ScsSector   Sector;
    public readonly    ScsItemType Type;

    public readonly ulong   Uid;
    public readonly float   X;
    public readonly float   Z;
    protected       ScsNode EndNode;
    protected       ulong   EndNodeUid;
    protected       ScsNode StartNode;
    protected       ulong   StartNodeUid;

    public ScsMapItem( ScsSector sector, int offset ) {
        Sector = sector;

        int fileOffset = offset;

        Type = (ScsItemType)MemoryHelper.ReadUInt32( Sector.Stream, fileOffset );
        Uid  = MemoryHelper.ReadUInt64( Sector.Stream, fileOffset += 0x04 );
        X    = MemoryHelper.ReadSingle( Sector.Stream, fileOffset += 0x08 );
        Z    = MemoryHelper.ReadSingle( Sector.Stream, fileOffset += 0x08 );
    }

    public List< ulong > Nodes     { get; protected set; }
    public int           BlockSize { get; protected set; }
    public bool          Valid     { get; protected set; }
    public bool          Hidden    { get; protected set; }

    public bool IsSecret { get; protected set; }

    public ScsNode GetStartNode() {
        return StartNode ??= Store().Map.GetNodeByUid( StartNodeUid );
    }

    public ScsNode GetEndNode() {
        return EndNode ??= Store().Map.GetNodeByUid( EndNodeUid );
    }

    protected static StoreHelper Store() => StoreHelper.Instance;
}