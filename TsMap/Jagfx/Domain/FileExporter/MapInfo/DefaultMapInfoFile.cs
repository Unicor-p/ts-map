using System;
using Newtonsoft.Json.Linq;
using TsMap.Jagfx.Shared;

namespace TsMap.Jagfx.Domain.FileExporter.MapInfo {
    public class DefaultMapInfoFile {
        protected readonly TsGame _game;
        protected readonly int    _maxZoom;
        protected readonly int    _minZoom;
        protected readonly int    _tileSize;
        protected readonly float  _x1;
        protected readonly float  _x2;
        protected readonly float  _y1;
        protected readonly float  _y2;
        protected          int    _mapPadding;

        public DefaultMapInfoFile( TsGame game, int mapPadding, int tileSize, float x1, float x2, float y1, float y2, int minZoom, int maxZoom ) {
            _game       = game;
            _mapPadding = mapPadding;
            _tileSize   = tileSize;
            _x1         = x1;
            _x2         = x2;
            _y1         = y1;
            _y2         = y2;
            _minZoom    = minZoom;
            _maxZoom    = maxZoom;
        }

        public virtual JObject TileMapInfo() =>
            new JObject {
                [ "maxX" ]        = _tileSize * 256,
                [ "maxY" ]        = _tileSize * 256,
                [ "x1" ]          = _x1,
                [ "x2" ]          = _x2,
                [ "y1" ]          = _y1,
                [ "y2" ]          = _y2,
                [ "tileSize" ]    = _tileSize,
                [ "minZoom" ]     = _minZoom,
                [ "maxZoom" ]     = _maxZoom,
                [ "gameCode" ]    = _game.Code,
                [ "gameName" ]    = _game.FullName(),
                [ "gameVersion" ] = _game.Version,
                [ "generatedAt" ] = DateTime.Now
            };
    }
}