using System;
using System.Threading;

namespace Libra.Base
{
    public abstract class Service
    {
        public virtual string Name { get; }
        public virtual bool Status { get { return Status; } }

        public Action Start { get { return () => ExecuteStart(); } }
        public Action Stop { get { return () => ExecuteStop(); } }
        public Action Restart { get; }

        public abstract void ExecuteStart();
        public abstract void ExecuteStop();
        public void ExecuteRestart()
        {
            ExecuteStop();
            Thread.Sleep(1000);
            ExecuteStart();
        }
    }
}
