using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SupportFramework.Patterns.ServiceRetry
{
    public interface ITechnicalLogger
    {
        void LogTechnicalError(string category, string message);
    }
}
