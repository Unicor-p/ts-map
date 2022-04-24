using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using TsMap2.Helper;
using TsMap2.Scs;
using TsMap2.Scs.FileSystem;
using TsMap2.Scs.FileSystem.Map;

namespace TsMap2.Job.Parse.Map {
    public class ParseMapFilesJob : ThreadJob {
        protected override void Do() {
            Log.Debug( "[Job][MapFiles] Loading" );

            UberDirectory baseMapEntry = Store().Ubs.GetDirectory( ScsPath.Map.MapDirectory );
            if ( baseMapEntry == null ) {
                var message = $"[Job][MapFiles] Could not read {ScsPath.Map.MapDirectory} dir";
                throw new JobException( message, JobName(), ScsPath.Map.MapDirectory );
            }

            List< string > mbdFilePaths = baseMapEntry.GetFilesByExtension( "map", ".mbd" );

            if ( mbdFilePaths.Count == 0 ) {
                var message = $"[Job][MapFiles] Could not find {ScsPath.Map.MapExtension} file";
                throw new JobException( message, JobName(), ScsPath.Map.MapExtension );
            }


            var sectorFiles = new List< string >();

            foreach ( string filePath in mbdFilePaths ) {
                string mapName = PathHelper.GetFileNameWithoutExtensionFromPath( filePath );

                UberDirectory mapFileDir = Store().Ubs.GetDirectory( $"map/{mapName}" );
                if ( mapFileDir == null ) {
                    var message = $"[Job][MapFiles] Could not read map/{mapName} directory";
                    throw new JobException( message, JobName(), mapName );
                }

                sectorFiles.AddRange( mapFileDir.GetFilesByExtension( $"map/{mapName}", ".base" ) );
            }

            sectorFiles.ForEach( Parse );

            Log.Information( "[Job][MapFiles] Loaded. Roads: {0}",            Store().Map.Roads.Count );
            Log.Information( "[Job][MapFiles] Loaded. Prefabs: {0}",          Store().Map.Prefabs.Count );
            Log.Information( "[Job][MapFiles] Loaded. Companies: {0}",        Store().Map.Companies.Count );
            Log.Information( "[Job][MapFiles] Loaded. Cities: {0}",           Store().Map.Cities.Count );
            Log.Information( "[Job][MapFiles] Loaded. MapOverlays: {0}",      Store().Map.MapOverlays.Count );
            Log.Information( "[Job][MapFiles] Loaded. FerryConnections: {0}", Store().Map.FerryConnections.Count );
            Log.Information( "[Job][MapFiles] Loaded. Triggers: {0}",         Store().Map.Triggers.Count );
            Log.Information( "[Job][MapFiles] Loaded. MapAreas: {0}",         Store().Map.MapAreas.Count );
        }

        private void Parse( string filePath ) {
            UberFile file  = Store().Ubs.GetFile( filePath );
            var      empty = false;

            if ( file == null ) // empty = true;
                return;

            byte[] stream = file.Entry.Read();
            var    sector = new ScsSector( filePath, stream );

            if ( sector.Version < 825 ) {
                Log.Warning( $"{filePath} version ({sector.Version}) is too low, min. is 825" );
                return;
            }

            if ( sector.ItemCount == 0 ) empty = true;
            if ( empty ) return;

            for ( var i = 0; i < sector.ItemCount; i++ ) {
                ScsItemType type = sector.ItemType;

                if ( sector.Version <= 825 ) type++; // after version 825 all types were pushed up 1

                ScsMapItem mapItem;
                switch ( type ) {
                    case ScsItemType.Road: {
                        mapItem           =  new ScsMapRoadItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.Prefab: {
                        mapItem           =  new ScsMapPrefabItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.Company: {
                        mapItem           =  new ScsMapCompanyItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.Service: {
                        mapItem           =  new ScsMapServiceItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.CutPlane: {
                        mapItem           =  new ScsMapCutPlaneItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.City: {
                        mapItem           =  new ScsMapCityItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.MapOverlay: {
                        mapItem           =  new ScsMapMapOverlayItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.Ferry: {
                        mapItem           =  new ScsMapFerryItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.Garage: {
                        mapItem           =  new ScsMapGarageItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.Trigger: {
                        mapItem           =  new ScsMapTriggerItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.FuelPump: {
                        mapItem           =  new ScsMapFuelPumpItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.RoadSideItem: {
                        mapItem           =  new ScsMapRoadSideItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.BusStop: {
                        mapItem           =  new ScsMapBusStopItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.TrafficRule: {
                        mapItem           =  new ScsMapTrafficRuleItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.TrajectoryItem: {
                        mapItem           =  new ScsMapTrajectoryItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    case ScsItemType.MapArea: {
                        mapItem           =  new ScsMapAreaItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.Cutscene: {
                        mapItem           =  new ScsMapCutsceneItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        Store().Map.AddItem( mapItem );
                        break;
                    }
                    case ScsItemType.VisibilityArea: {
                        mapItem           =  new ScsVisibilityAreaItem( sector );
                        sector.LastOffset += mapItem.BlockSize;
                        break;
                    }
                    default: {
                        Log.Warning( "Unknown Type: {0} in {1} @ {2}", type, Path.GetFileName( filePath ), sector.LastOffset );
                        break;
                    }
                }
            }

            int nodeCount = MemoryHelper.ReadInt32( stream, sector.LastOffset );
            for ( var i = 0; i < nodeCount; i++ ) {
                var node = new ScsNode( sector );
                Store().Map.UpdateEdgeCoords( node );
                Store().Map.AddNode( node );
                sector.LastOffset += 0x34;
            }

            sector.LastOffset += 0x04;

            if ( sector.Version >= 891 ) {
                var visAreaChildCount = BitConverter.ToInt32( stream, sector.LastOffset );
                sector.LastOffset += 0x04 + 0x08 * visAreaChildCount; // 0x04(visAreaChildCount) + (visAreaChildUids)
            }

            if ( sector.LastOffset != stream.Length )
                Log.Warning( $"File '{Path.GetFileName( filePath )}' was not read correctly. Read offset was at 0x{sector.LastOffset:X} while file is 0x{stream.Length:X} bytes long." );

            sector.ClearFileData();
        }
    }
}