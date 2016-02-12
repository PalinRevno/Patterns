using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace SupportFramework.Patterns.Services
{
    /// <summary>
    /// Provides a way for the application to notify the service of an incoming call
    /// </summary>
    [ServiceContract]
    public interface IUIService
    {
        /// <summary>
        /// A blank method that wakes up the IIS, before the real requests start to arrive.
        /// Most of the time used when the requests are made following a human input, not on page load.
        /// </summary>
        [OperationContract]
        void WakeUp();
    }
}
