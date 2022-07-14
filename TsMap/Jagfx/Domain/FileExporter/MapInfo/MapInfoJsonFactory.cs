using System;
using System.IO;
using Newtonsoft.Json.Linq;
using TsMap.Jagfx.Shared;
using TsMap.Jagfx.Shared.JsonFactory;

namespace TsMap.Jagfx.Domain.FileExporter.MapInfo {
    public class MapInfoJsonFactory : JsonFactory< JObject > {
        private readonly DefaultMapInfoFile _mapInfoFile;

        public MapInfoJsonFactory( DefaultMapInfoFile mapInfoFile ) => _mapInfoFile = mapInfoFile;

        public override string GetFileName() => AppPaths.TileMapInfoFileName;

        // public override string GetSavingPath() => Path.Combine( Store.Settings.OutputPath, Store.Game.Code, "latest/" );
        public override string GetSavingPath() => Path.Combine( AppPaths.OutputDir, Store.Game.Code, "latest/" );

        public override string GetLoadingPath() => throw new NotImplementedException();

        public override JObject Convert( JObject raw ) => throw new NotImplementedException();

        public override JContainer RawData() => _mapInfoFile.TileMapInfo();
    }
}