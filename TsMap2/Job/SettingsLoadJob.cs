using Serilog;
using TsMap2.Factory.Json;
using TsMap2.Model;

namespace TsMap2.Job {
    public class SettingsLoadJob : ThreadJob {
        public TsSettingsJsonFactory SettingFactory = new TsSettingsJsonFactory( new Settings() );

        protected override void Do() {
            Log.Debug( "[Job][Setting] Loading" );
            this.Store().SetSetting( this.SettingFactory.Load() );
            Log.Debug( "[Job][Setting] Loaded" );
        }

        public override string JobName() => "Setting";

        protected override void OnEnd() { }
    }
}