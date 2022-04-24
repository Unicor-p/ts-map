using System.Drawing;
using Serilog;
using TsMap2.Helper;
using TsMap2.Model.Ts;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Job.Parse.Overlays {
    public class ParseOverlayFerryJob : ThreadJob {
        protected override void Do() {
            Log.Information( "[Job][OverlayFerry] Parsing..." );

            foreach ( ScsMapFerryItem ferry in Store().Map.FerryConnections ) {
                Bitmap b = ferry.Overlay?.GetBitmap();

                if ( !ferry.Valid || ferry.Hidden || b == null ) continue;
                string overlayName = ScsTokenHelper.TokenToString( ferry.OverlayToken );

                if ( ferry.Train ) {
                    var ov = new TsMapOverlayItem( ferry.X, ferry.Z, overlayName, TsMapOverlayType.Train, b );
                    Store().Map.Overlays.Train.Add( ov );
                } else {
                    var ov = new TsMapOverlayItem( ferry.X, ferry.Z, overlayName, TsMapOverlayType.Ferry, b );
                    Store().Map.Overlays.Ferry.Add( ov );
                }
            }

            Log.Information( "[Job][OverlayFerry] Train: {0}", Store().Map.Overlays.Train.Count );
            Log.Information( "[Job][OverlayFerry] Ferry: {0}", Store().Map.Overlays.Ferry.Count );
        }
    }
}