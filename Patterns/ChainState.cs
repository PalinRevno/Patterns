using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SupportFramework.Patterns.Responsibility
{
    public class ChainState<T>
            where T : new()
    {
        public bool ChainSuccessful { get; internal set; }
        public string StageName { get; internal set; }
        public Exception Error { get; internal set; }
        public T State { get; private set; }

        internal ChainState(T state)
        {
            State = state;
            ChainSuccessful = true;
            Error = null;
        }
    }
}
