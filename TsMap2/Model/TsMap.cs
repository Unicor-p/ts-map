﻿using System.Collections.Generic;
using TsMap2.Model.TsMapItem;

namespace TsMap2.Model {
    public class TsMap {
        public readonly List< TsMapCityItem >       Cities           = new List< TsMapCityItem >();
        public readonly List< TsMapCompanyItem >    Companies        = new List< TsMapCompanyItem >();
        public readonly List< TsMapFerryItem >      FerryConnections = new List< TsMapFerryItem >();
        public readonly List< TsMapMapAreaItem >    MapAreas         = new List< TsMapMapAreaItem >();
        public readonly List< TsMapMapOverlayItem > MapOverlays      = new List< TsMapMapOverlayItem >();
        public readonly List< TsMapPrefabItem >     Prefabs          = new List< TsMapPrefabItem >();
        public readonly List< TsMapRoadItem >       Roads            = new List< TsMapRoadItem >();
        public readonly List< TsMapTriggerItem >    Triggers         = new List< TsMapTriggerItem >();
        public          Dictionary< ulong, TsNode > Nodes            = new Dictionary< ulong, TsNode >();
        public          TsMapOverlays               Overlays         = new TsMapOverlays();

        public TsNode GetNodeByUid( ulong uid ) =>
            this.Nodes.ContainsKey( uid )
                ? this.Nodes[ uid ]
                : null;
    }
}