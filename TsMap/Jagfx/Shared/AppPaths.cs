using System;
using System.IO;

namespace TsMap.Jagfx.Shared {
    public static class AppPaths {
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
        public static string HomeDirApp => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ), "TsMap" );
        public static string OutputDir  => Path.Combine( HomeDirApp,                                                         "Output" );
        public static string LogPath    => Path.Combine( HomeDirApp,                                                         "Logs/log_.txt" );
        public static string RawFolder  => Path.Combine( HomeDirApp,                                                         "Raw" );
    }
}