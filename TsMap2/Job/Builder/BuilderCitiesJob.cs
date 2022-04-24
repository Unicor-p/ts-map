using System.Collections.Generic;
using System.Linq;
using Serilog;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Job.Builder {
    public class BuilderCitiesJob : ThreadJob {
        protected override void Do() {
            Log.Debug( "[Job][Builder][City] Building" );

            foreach ( KeyValuePair< ulong, ScsMapCityItem > pair in Store().Map.Cities ) {
                ScsMapCityItem item = pair.Value;
                ScsNode?       node = Store().Map.GetNodeByUid( item.NodeUid );

                if ( node == null ) {
                    Log.Debug( "[Job][Builder][City] City '{0}' skipped and will be removed", item.City.Name );
                    continue;
                }

                ( float x, float z ) = ( node.X, node.Z );
                item.City.X          = x;
                item.City.Y          = z;

                // Log.Debug( "[Job][Builder][City] City '{0}' at {1},{2}", item.City.Name, item.City.X, item.City.Y );
            }

            // FIXME: Check if it's possible to read data from Node
            Store().Def.Cities = Store()
                                 .Def.Cities
                                 .Where( kv => kv.Value.X != 0 && kv.Value.Y != 0 )
                                 .ToDictionary( kv => kv.Key, kv => kv.Value );

            Log.Information( "[Job][Builder][City] Done" );
        }
    }
}