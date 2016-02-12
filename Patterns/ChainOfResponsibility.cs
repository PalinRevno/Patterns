using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SupportFramework.Patterns.Responsibility
{
    /// <typeparam name="T">The output of the process</typeparam>
    /// <typeparam name="I">Additional, reference information to guide the process</typeparam>
    public class ChainOfResponsibility<T,I>
        where T : new()
    {
        private List<Tuple<string, LinkActivity>> _chain;

        public delegate bool LinkActivity(T state, I input);

        /// <summary>
        /// Initializes an instance of the <see cref="ChainOfResponsibility"/> class.
        /// </summary>
        public ChainOfResponsibility()
        {
            _chain = new List<Tuple<string, LinkActivity>>();
        }

        /// <summary>
        /// Appends a new activity to the chain
        /// </summary>
        /// <param name="stageName">A description of the current activity</param>
        /// <param name="link">The stage logic</param>
        public void AddLink(string stageName, LinkActivity link)
        {
            _chain.Add(new Tuple<string,LinkActivity>(stageName,link));
        }

        /// <param name="input">The input into the process. The empty constructor of the output object will be called</param>
        public ChainState<T> Run(I input)
        {
            return Run(input, new T());
        }

        /// <param name="input">The input into the process</param>
        /// <param name="initialState">The initial state of the output object</param>
        public ChainState<T> Run(I input, T initialState)
        {
            IEnumerator<Tuple<string, LinkActivity>> linkEnumeration;
            ChainState<T> state = null;

            // Validating chain consistency
            if (_chain.Count == 0)
            {
                throw new Exception("The chain has no links to execute");
            }

            // Getting the link enumeration
            linkEnumeration = _chain.GetEnumerator();

            // Initializing the state
            state = new ChainState<T>(initialState);
            
            // Iterating the chain
            while (state.ChainSuccessful && linkEnumeration.MoveNext())
            {
                // Applying the stage name
                state.StageName = linkEnumeration.Current.Item1;

                try
                {
                    // Invoking the actual chain link
                    state.ChainSuccessful = linkEnumeration.Current.Item2.Invoke(state.State, input);
                }
                catch (Exception ex)
                {
                    // Indicating stage failure
                    state.ChainSuccessful = false;

                    // Storing the exception
                    state.Error = ex;
                }
            }

            return state;
        }
    }
}
