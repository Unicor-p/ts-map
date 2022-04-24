using System.Text;
using TsMap2.Exceptions;

namespace TsMap2.Scs.FileSystem.Entry {
    public class ScsVersionEntry : AbstractScsEntry< string > {
        public string Get() {
            UberFile versionFile = Store.Ubs.GetFile( ScsPath.GameVersion );

            if ( versionFile == null )
                throw new ScsEntryException( "Version file named was not found" );

            return Generate( versionFile.Entry.Read() );
        }

        public override string Generate( byte[] stream ) => Encoding.UTF8.GetString( stream ).Split( '\n' )[ 0 ];
    }
}