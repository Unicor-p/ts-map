using System;
using System.IO;

namespace TsMap2.Helper {
    public static class Common {
        public const float LaneWidth = 4.5f;
    }

    public static class AppPath {
        // -- Settings
        public const  string TileMapInfoFileName = "TileMapInfo.json";
        public const  string SettingFileName     = "Settings.json";
        public const  string CitiesFileName      = "Cities.json";
        public const  string CountriesFileName   = "Countries.json";
        public const  string OverlaysFileName    = "Overlays.json";
        public const  string DataOverview        = "DataOverview.json";
        public const  string GeoJsonCities       = "Cities.geojson";
        public const  string CitiesBinary        = "cities.ci.bin";
        public const  string PointsBinary        = "points.po.bin";
        public static string HomeDirApp => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ), "TsMap2" );
        public static string OutputDir  => Path.Combine( HomeDirApp,                                                         "Output" );
        public static string LogPath    => Path.Combine( HomeDirApp,                                                         "Logs/log_.txt" );
        public static string RawFolder  => Path.Combine( HomeDirApp,                                                         "Raw" );
    }


    [ Flags ]
    public enum RenderFlags {
        None             = 0,
        TextOverlay      = 1 << 0, // 1
        Prefabs          = 1 << 1, // 2
        Roads            = 1 << 2, // 4
        MapAreas         = 1 << 3, // 8
        MapOverlays      = 1 << 4, // 16
        FerryConnections = 1 << 5, // 32
        CityNames        = 1 << 6, // 64
        SecretRoads      = 1 << 7, // 128
        All              = int.MaxValue
    }

    [ Flags ]
    public enum ExportFlags {
        None                  = 0,
        TileMapInfo           = 1 << 0, // 1
        CityList              = 1 << 1, // 2
        CityDimensions        = 1 << 2, // 4
        CityLocalizedNames    = 1 << 3, // 8
        CountryList           = 1 << 4, // 16
        CountryLocalizedNames = 1 << 5, // 32
        OverlayList           = 1 << 6, // 64
        OverlayPNGs           = 1 << 7, // 128
        All                   = int.MaxValue
    }

    public static class FlagMethods {
        public static bool IsActive( this RenderFlags self, RenderFlags value ) => ( self & value ) == value;
    }

    public class Mod {
        public Mod( string path ) {
            ModPath = path;
            Load    = false;
        }

        public string ModPath { get; set; }
        public bool   Load    { get; set; }

        public override string ToString() => Path.GetFileName( ModPath );
    }

    public class JobException : Exception {
        public JobException( string message, string jobName, object context ) : base( message ) {
            JobName = jobName;
            Context = context;
        }

        public string JobName { get; }
        public object Context { get; }
    }
}