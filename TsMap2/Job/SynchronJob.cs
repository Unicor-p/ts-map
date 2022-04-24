using System;
using Serilog;
using TsMap2.Helper;

namespace TsMap2.Job;

public abstract class SynchronJob : JobInterface {
    protected abstract void        Do();
    public virtual     string      JobName() => GetType().Name;
    protected          StoreHelper Store()   => StoreHelper.Instance;

    public void Run() {
        try {
            Do();
        } catch ( Exception e ) {
            HandleException( e );
        }
    }

    private static void HandleException( Exception e ) {
        if ( e.GetBaseException().GetType() != typeof( JobException ) ) return;

        var ex = (JobException)e.GetBaseException();

        Log.Error( "Job Exception ({0}): {1} | Stack: {2}", ex.JobName, ex.Message, ex.StackTrace );
    }
}