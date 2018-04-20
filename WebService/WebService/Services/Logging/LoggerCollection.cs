using System;
using System.Collections;
using System.Collections.Generic;

namespace WebService.Services.Logging
{
    public class LoggerCollection : ILoggerCollection
    {
        #region FIELDS

        private readonly ICollection<ILogger> _collection;

        #endregion FIELDS


        #region CONSTRUCTORS

        public LoggerCollection(IEnumerable<ILogger> loggers)
        {
            _collection = new List<ILogger>(loggers);
        }

        public LoggerCollection(int capacity)
        {
            _collection = new List<ILogger>(capacity);
        }

        public LoggerCollection()
        {
            _collection = new List<ILogger>();
        }

        #endregion CONSTRUCTORS


        #region PROPERTIES

        public int Count
            => _collection.Count;

        public bool IsReadOnly
            => _collection.IsReadOnly;

        #endregion PROPERTIES


        #region METHODS

        public void Log<T>(T sender, ELogLevel logLevel, Exception exception)
        {
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            foreach (var logger in _collection)
                logger.Log(sender, logLevel, exception);
        }

        public void Log<T>(T sender, ELogLevel logLevel, string message)
        {
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            foreach (var logger in _collection)
                logger.Log(sender, logLevel, message);
        }

        public void Add(ILogger logger)
            => _collection.Add(logger);

        public bool Remove(ILogger logger)
            => _collection.Remove(logger);

        public void Clear()
            => _collection.Clear();

        public bool Contains(ILogger logger)
            => _collection.Contains(logger);

        public void CopyTo(ILogger[] array, int arrayIndex)
            => _collection.CopyTo(array, arrayIndex);

        public IEnumerator<ILogger> GetEnumerator()
            => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _collection.GetEnumerator();

        #endregion METHODS
    }
}