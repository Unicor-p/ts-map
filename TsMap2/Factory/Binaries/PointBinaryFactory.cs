using System.Collections.Generic;
using System.IO;
using TsMap2.Helper;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Factory.Binaries {
    public class PointBinaryFactory : BinaryFactory< List< ScsMapRoadItem > > {
        private readonly List< ScsMapRoadItem > _roadItems;

        public PointBinaryFactory( List< ScsMapRoadItem > roadItems ) => _roadItems = roadItems;

        public override string GetSavingPath() => Path.Combine( Store.Settings.OutputPath, Store.Game.Code, "latest/", AppPath.PointsBinary );

        public override void Save() {
            Writer().Write( 1 );
            Writer().Write( _roadItems.Count );
            Writer().Write( 0 );

            foreach ( ScsMapRoadItem roadItem in _roadItems ) {
                Writer().Write( roadItem.GetStartNode().X );
                Writer().Write( roadItem.GetStartNode().Z );
                Writer().Write( roadItem.GetEndNode().X );
                Writer().Write( roadItem.GetEndNode().Z );
            }
        }
    }
}