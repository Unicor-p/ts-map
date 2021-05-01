using System.Text;
using Serilog;
using TsMap2.Model;
using TsMap2.ScsHash;

namespace TsMap2.Job.Parse {
    public class ParseGameJob : ThreadJob {
        protected override void Do() {
            Log.Debug( "[Job][Game] Loading" );
            // Console.WriteLine( this.Store().Settings.GamePath );

            // --- Kind of game
            ScsFile ets2File = this.Store().Rfs.GetFileEntry( TsSiiDef.Ets2LogoScene );
            ScsFile atsFile  = this.Store().Rfs.GetFileEntry( TsSiiDef.AtsLogoScene );

            if ( ets2File != null ) // Log.Msg( "ETS2 detected" );
                this.Store().Game.Code = TsGame.GAME_ETS;
            else if ( atsFile != null ) // Log.Msg( "ATS detected" );
                this.Store().Game.Code = TsGame.GAME_ATS;
            else // Log.Msg( "Unknown game" );
                this.Store().Game.Code = null;


            // --- Game version
            ScsFile versionFile = this.Store().Rfs.GetFileEntry( TsSiiDef.GameVersion );
            byte[]  content     = versionFile.Entry.Read();

            this.Store().Game.Version = Encoding.UTF8.GetString( content ).Split( '\n' )[ 0 ];

            Log.Debug( "[Job][Game] Loaded. Game: {0} | Version: {1}", this.Store().Game.FullName(), this.Store().Game.Version );
        }

        protected override void OnEnd() { }

        public override string JobName() => "ParseGameJob";
    }
}