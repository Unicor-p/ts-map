using Newtonsoft.Json.Linq;

namespace TsMap.Jagfx.Shared.JsonFactory {
    public interface IJsonFactory< out T > {
        string GetFileName();
        string GetSavingPath();
        string GetLoadingPath();
        void   Save();
        T      Load();

        JContainer RawData();

        T Convert( JObject raw );
    }
}