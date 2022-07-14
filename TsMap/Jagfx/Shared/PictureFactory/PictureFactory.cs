using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TsMap.Jagfx.Shared.PictureFactory {
    public class PictureFactory {
        private readonly Bitmap _bitmap;
        // private static   StoreHelper Store => StoreHelper.Instance;

        protected PictureFactory( Bitmap bitmap ) => _bitmap = bitmap;

        protected void Save( string dir, string fileName, ImageFormat format ) {
            // string fullDir  = Path.Combine( Store.Settings.OutputPath, Store.Game.Code, "latest/", dir );
            string fullDir  = Path.Combine( AppPaths.OutputDir, "ets2", "latest/", dir );
            string fullPath = Path.Combine( fullDir,            fileName );

            Directory.CreateDirectory( fullDir );
            _bitmap.Save( fullPath, format );
        }
    }
}