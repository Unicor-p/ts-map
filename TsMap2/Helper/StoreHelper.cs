using TsMap2.Model;
using TsMap2.Model.Ts;
using TsMap2.Scs.FileSystem;

namespace TsMap2.Helper {
    public sealed class StoreHelper {
        public readonly TsDef  Def = new();
        public          TsGame Game;
        public readonly TsMap  Map = new();

        // --


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static StoreHelper() { }

        private StoreHelper() { }
        public Settings       Settings { get; private set; } = new Settings();
        public UberFileSystem Ubs      => UberFileSystem.Instance;

        public static StoreHelper Instance { get; } = new StoreHelper();

        public LocalizationManager Localization { get; } = new();

        // ---

        public void SetSetting( Settings settings ) {
            Settings = settings;
            Ubs.AddSourceDirectory( settings.GetActiveGamePath() );
        }
    }
}