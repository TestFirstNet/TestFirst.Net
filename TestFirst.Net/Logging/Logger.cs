using System;

namespace TestFirst.Net.Logging
{
    /// <summary>
    /// The static log factory used to create all the loggers internally. Register your custom factory to override (e.g. to 
    /// hook in log4net or something else)
    /// </summary>
    public static class Logger
    {
        private static readonly ILogFactory DefaultFactory = new DefaultLogFactory(LogLevel.Info);

        private static ILogFactory _factory = DefaultFactory; 

        /// <summary>
        /// Gets or sets the default log factory. If null the default is used again
        /// </summary>
        public static ILogFactory Factory 
        {
            get { return _factory;  }
            set { _factory = value ?? DefaultFactory; }
        }

        /// <summary>
        /// Sets the default log factory with the given level
        /// </summary>
        public static LogLevel Level 
        {
            set { _factory = new DefaultLogFactory(value); }
        }

        public static ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILogger GetLogger(Type type)
        {
            return _factory.GetLogger(type);
        }

        public static ILogger GetLogger(string logName)
        {
            return _factory.GetLogger(logName);
        }
    }
}
