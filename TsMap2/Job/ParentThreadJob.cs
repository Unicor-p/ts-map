using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TsMap2.Helper;

namespace TsMap2.Job {
    public abstract class ParentThreadJob {
        private readonly Dictionary< string, JobInterface > _jobPool = new();

        public Task t { get; set; }

        public string JobName() => GetType().Name;

        protected abstract void Do();

        protected void OnEnd() { }

        public void Run() {
            try {
                t = Task.Factory.StartNew( () => {
                    Do();

                    foreach ( KeyValuePair< string, JobInterface > keyValuePair in _jobPool ) {
                        JobInterface job = keyValuePair.Value;

                        job.Run();
                    }
                } );

                t.Wait();
            } catch ( Exception e ) {
                Console.WriteLine( e );
                throw;
            }
        }

        public void AddJob( JobInterface job ) {
            _jobPool.Add( job.JobName(), job );
        }

        public StoreHelper Store() => StoreHelper.Instance;
    }
}