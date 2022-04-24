using TsMap2.Helper;

namespace TsMap2.Scs.FileSystem.Entry {
    public abstract class AbstractScsEntry< T > {
        protected StoreHelper Store => StoreHelper.Instance;

        public T? Get( string path ) {
            UberFile file = Store.Ubs.GetFile( path );

            if ( file == null ) return default;
            byte[] fileContent = file.Entry.Read();

            return Generate( fileContent );
        }

        public abstract T? Generate( byte[] stream );
    }
}