using System;
using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Map;

public class ScsNode {
    public ScsNode( ScsSector sector ) {
        sector.LastOffset += 0x04;
        int fileOffset = sector.LastOffset;

        Uid = MemoryHelper.ReadUInt64( sector.Stream, fileOffset );
        X   = MemoryHelper.ReadInt32( sector.Stream, fileOffset += 0x08 ) / 256f;
        Z   = MemoryHelper.ReadInt32( sector.Stream, fileOffset += 0x08 ) / 256f;

        float rX = MemoryHelper.ReadSingle( sector.Stream, fileOffset += 0x04 );
        float rZ = MemoryHelper.ReadSingle( sector.Stream, fileOffset + 0x08 );

        double rot = Math.PI - Math.Atan2( rZ, rX );
        Rotation = (float)( rot % Math.PI * 2 );
    }

    public ulong Uid      { get; }
    public float X        { get; }
    public float Z        { get; }
    public float Rotation { get; }
}