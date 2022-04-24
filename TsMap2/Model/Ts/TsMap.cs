using System.Collections.Generic;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Model.Ts;

public class TsMap {
    public readonly Dictionary< ulong, ScsMapCityItem > Cities           = new();
    public readonly List< ScsMapCompanyItem >           Companies        = new();
    public readonly List< ScsMapFerryItem >             FerryConnections = new();
    public readonly List< ScsMapAreaItem >              MapAreas         = new();
    public readonly List< ScsMapMapOverlayItem >        MapOverlays      = new();
    public readonly List< ScsMapPrefabItem >            Prefabs          = new();
    public          List< ScsMapRoadItem >              Roads            = new();
    public readonly List< ScsMapTriggerItem >           Triggers         = new();
    public readonly List< ScsMapCutsceneItem >          Viewpoints       = new();
    public          float                               MaxX             = float.MinValue;
    public          float                               MaxZ             = float.MinValue;
    public          float                               MinX             = float.MaxValue;
    public          float                               MinZ             = float.MaxValue;
    public          Dictionary< ulong, ScsNode >        Nodes            = new();
    public          TsMapOverlays                       Overlays         = new();

    public ScsNode? GetNodeByUid( ulong uid ) =>
        Nodes.ContainsKey( uid )
            ? Nodes[ uid ]
            : null;

    public void AddNode( ScsNode node ) {
        if ( !Nodes.ContainsKey( node.Uid ) )
            Nodes.Add( node.Uid, node );
    }

    public void AddItem( ScsMapItem mapItem ) {
        if ( !mapItem.Valid ) return;

        switch ( mapItem ) {
            case ScsMapRoadItem item:
                if ( !item.Hidden )
                    Roads.Add( item );
                break;
            case ScsMapPrefabItem item:
                Prefabs.Add( item );
                break;
            case ScsMapCompanyItem item:
                Companies.Add( item );
                break;
            case ScsMapCityItem item:
                if ( !Cities.ContainsKey( item.City.Token ) && !item.Hidden )
                    Cities.Add( item.City.Token, item );

                break;
            case ScsMapMapOverlayItem item:
                MapOverlays.Add( item );
                break;
            case ScsMapFerryItem item:
                FerryConnections.Add( item );
                break;
            case ScsMapTriggerItem item:
                Triggers.Add( item );
                break;
            case ScsMapAreaItem item:
                MapAreas.Add( item );
                break;
            case ScsMapCutsceneItem item:
                Viewpoints.Add( item );
                break;
        }
    }

    public void UpdateEdgeCoords( ScsNode node ) {
        if ( MinX > node.X ) MinX = node.X;
        if ( MaxX < node.X ) MaxX = node.X;
        if ( MinZ > node.Z ) MinZ = node.Z;
        if ( MaxZ < node.Z ) MaxZ = node.Z;
    }
}