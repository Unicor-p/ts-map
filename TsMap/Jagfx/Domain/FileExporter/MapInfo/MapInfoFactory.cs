using System;
using TsMap.Jagfx.Shared;

namespace TsMap.Jagfx.Domain.FileExporter.MapInfo {
    public static class MapInfoFactory {
        public static DefaultMapInfoFile Generate( MapInfoType type,
                                                   TsGame      game,
                                                   int         mapPadding,
                                                   int         tileSize,
                                                   float       x1,
                                                   float       x2,
                                                   float       y1,
                                                   float       y2,
                                                   int         minZoom,
                                                   int         maxZoom ) {
            switch ( type ) {
                case MapInfoType.Default:
                    return new DefaultMapInfoFile( game, mapPadding, tileSize, x1, x2, y1, y2, minZoom, maxZoom );
                case MapInfoType.Jagfx:
                    return new JagfxMapInfoFile( game, mapPadding, tileSize, x1, x2, y1, y2, minZoom, maxZoom );
                default: throw new ArgumentOutOfRangeException( nameof( type ), type, null );
            }
        }
    }
}