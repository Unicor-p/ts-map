namespace TsMap.Jagfx.Shared {
    public static class ScsPath {
        public const string GameVersion      = "/version.txt";
        public const string ScsFileExtension = "sii";
        public const string ScsMatExtension  = "mat";

        public static class Def {
            public const string DefFolderName = "def";

            // -- TsGame
            public const string Ets2LogoScene = "def/ets2_logo_scene.sii";

            // -- TsCountry
            public const string CountryFileName = "country";

            // -- TsPrefab
            public const string WorldPath      = "def/world";
            public const string PrefabFileName = "prefab";

            // -- TsFerryConnection
            public const string FerryConnectionPath = "def/ferry/connection";

            // -- TsCity
            public const string CityFileName = "city";

            // -- TsRoadLook
            public const string RoadLook = "road_look";

            // -- TsOverlay
            public const           string MaterialUiMapPath     = "material/ui/map";
            public const           string MaterialUiCompanyPath = "material/ui/company/small";
            public const           string MaterialUiRoadPath    = "material/ui/map/road";
            public static readonly string CountryBaseName       = "custom/country.sii";
        }

        public static class Map {
            // -- TsMap
            public const string MapDirectory     = "map";
            public const string MapExtension     = "mbd";
            public const string MapFileExtension = ".base";
        }
    }
}