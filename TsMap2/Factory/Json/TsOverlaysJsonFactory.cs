using System;
using Newtonsoft.Json.Linq;
using TsMap2.Helper;
using TsMap2.Model;

namespace TsMap2.Factory.Json {
    public class TsOverlaysJsonFactory : JsonFactory< TsMapOverlays > {
        public TsOverlaysJsonFactory( TsMapOverlays mapOverlays ) => this._mapOverlays = mapOverlays;
        private TsMapOverlays _mapOverlays { get; }

        public override string GetFileName() => AppPath.OverlaysFileName;

        public override string GetSavingPath() => this.Store.Settings.OutputPath;

        public override string GetLoadingPath() => throw new NotImplementedException();

        public override TsMapOverlays Convert( JObject raw ) => raw.ToObject< TsMapOverlays >();

        public override JContainer RawData() => JObject.FromObject( this._mapOverlays );
    }
}