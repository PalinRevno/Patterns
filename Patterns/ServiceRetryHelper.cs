using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SupportFramework.Patterns.ServiceRetry
{
    public class ServiceRetryHelper
    {
        private int _retriesLeft;
        private int _retryCooldown;
        private string _connectionErrorMessage;
        private string _businessErrorMessage;
        private string _unexpectedErrorMessage;
        private bool _throwErrors;
        private bool _logErrors;
        private ITechnicalLogger _logger;

        private const string ACTUAL_ERROR_MESSAGE_DELIMITER = " Message : ";
        private const string LOG_CATEGORY = "External Service connection";
        private const int MAX_ERROR_LEVEL_DEPTH = 5;
        private const string INNER_ERROR_DELIMITER = " -> ";

        private bool AnyRetriesLeft
        {
            get
            {
                return _retriesLeft > 0;
            }
        }

        /// <summary>
        /// Gets or sets an auxiliary variable, that serves as a stop condition in call loops
        /// </summary>
        public bool NeedToRetry { get; set; }

        public ServiceRetryHelper(
            ITechnicalLogger logger,
            string connectionErrorMessage,
            string businessErrorMessage,
            string unexpectedErrorMessage,
            int maxRetries = 3,
            int retryCooldown = 50,
            bool logErrors = false,
            bool throwErrors = true)
        {
            // Initializing data members with the default values
            NeedToRetry = false;

            // Counting down
            _retriesLeft = maxRetries;
            _retryCooldown = retryCooldown;

            // Initializing provided errors, for later handling
            _logger = logger;
            _connectionErrorMessage = connectionErrorMessage;
            _businessErrorMessage = businessErrorMessage;
            _unexpectedErrorMessage = unexpectedErrorMessage;
            _throwErrors = throwErrors;
            _logErrors = logErrors;
        }

        /// <summary>
        /// Handles, in a standard manner
        /// </summary>
        public void HandleUnexpectedException(Exception ex)
        {
            // Checking if a log is reqired
            if (_logErrors)
            {
                _logger.LogTechnicalError(LOG_CATEGORY, _unexpectedErrorMessage + ACTUAL_ERROR_MESSAGE_DELIMITER + ExtractMultiLevelErrorMessage(ex));
                NeedToRetry = false;
            }

            // Has to be done after the log, because it ends the execution
            if (_throwErrors)
            {
                // Indicating a business exception
                throw new Exception(_unexpectedErrorMessage + ACTUAL_ERROR_MESSAGE_DELIMITER + ex.Message, ex);
            }
        }

        /// <summary>
        /// Iterates the inner levels(up to 5) of the exception and appends the message
        /// </summary>
        /// <param name="ex">The top level error</param>
        /// <returns>All level message</returns>
        private string ExtractMultiLevelErrorMessage(Exception ex)
        {
            StringBuilder response;
            int errorDepth;
            Exception currentException;

            // Initializing the top level error
            response = new StringBuilder(ex.Message);
            errorDepth = 0;
            currentException = ex;

            // Iterating inner errors
            while (currentException.InnerException != null && errorDepth < MAX_ERROR_LEVEL_DEPTH)
            {
                // Advancing the pointer
                currentException = currentException.InnerException;
                errorDepth++;

                // Applying the log
                response.Append(INNER_ERROR_DELIMITER);
                response.Append(currentException.Message);
            }

            return response.ToString();
        }

        /// <summary>
        /// Handles the exception and updates the parameters accodingly
        /// </summary>
        /// <param name="ex">The base exception</param>
        public void HandleException(CommunicationException ex)
        {
            // Checking if the exception came from the code
            if (ex is FaultException)
            {
                // Checking if a log is reqired
                if (_logErrors)
                {
                    _logger.LogTechnicalError(LOG_CATEGORY, _businessErrorMessage + ACTUAL_ERROR_MESSAGE_DELIMITER + ExtractMultiLevelErrorMessage(ex));
                    NeedToRetry = false;
                }

                // Has to be done after the log, because it ends the execution
                if (_throwErrors)
                {
                    // Indicating a business exception
                    throw new Exception(_businessErrorMessage + ACTUAL_ERROR_MESSAGE_DELIMITER + ex.Message, ex);
                }
            }
            else
            {
                // The exception occured in the communication
                // Indicating a failed attempt
                _retriesLeft--;

                // Checking if there are any retries left
                if (AnyRetriesLeft)
                {
                    // Waiting for the cooldown to expire
                    Thread.Sleep(_retryCooldown);
                }
                else
                {
                    // Checking if a log is reqired
                    if (_logErrors)
                    {
                        _logger.LogTechnicalError(LOG_CATEGORY, _connectionErrorMessage + ACTUAL_ERROR_MESSAGE_DELIMITER + ExtractMultiLevelErrorMessage(ex));
                        NeedToRetry = false;
                    }

                    // Has to be done after the log, because it ends the execution
                    if (_throwErrors)
                    {
                        // Indicating a connection exception
                        throw new Exception(_connectionErrorMessage + ACTUAL_ERROR_MESSAGE_DELIMITER + ex.Message, ex);
                    }
                }
            }
        }
    }
}
