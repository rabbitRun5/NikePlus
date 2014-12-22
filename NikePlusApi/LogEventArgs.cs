using System;

namespace NikePlusApi
{
    public class LogEventArgs : EventArgs
    {
        #region Properties

        public string Message
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public LogEventArgs(string message)
        {
            Message = message;
        }

        #endregion
    }
}
