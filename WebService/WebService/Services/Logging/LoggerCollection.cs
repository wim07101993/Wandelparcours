using System;
using System.Collections;
using System.Collections.Generic;

namespace WebService.Services.Logging
{
    /// <inheritdoc >
    /// <see cref="ILoggerCollection"/>
    /// </inheritdoc>
    /// <summary>
    /// LoggerCollection is a class that implements the <see cref="ILoggerCollection"/> Interfaces.
    /// <para>
    /// It logs messages using all the loggers it contains.
    /// </para>
    /// </summary>
    public class LoggerCollection : ILoggerCollection
    {
        #region FIELDS

        /// <summary>
        /// _collection is the collection containing the loggers used to log messages
        /// </summary>
        private readonly ICollection<ILogger> _collection;

        #endregion FIELDS


        #region CONSTRUCTOR

        /// <summary>
        /// LoggerCollection is the constructor to create an instance of the <see cref="LoggerCollection"/> class.
        /// </summary>
        /// <param name="loggers">is the initial collection of loggers</param>
        public LoggerCollection(IEnumerable<ILogger> loggers)
        {
            // set the collection
            _collection = new List<ILogger>(loggers);
        }

        /// <summary>
        /// LoggerCollection is the constructor to create an instance of the <see cref="LoggerCollection"/> class.
        /// </summary>
        /// <param name="capacity">is the initial capacity of the collection</param>
        public LoggerCollection(int capacity)
        {
            // set the collection
            _collection = new List<ILogger>(capacity);
        }

        /// <summary>
        /// LoggerCollection is the constructor to create an instance of the <see cref="LoggerCollection"/> class.
        /// </summary>
        public LoggerCollection()
        {
            // set the collection
            _collection = new List<ILogger>();
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        /// <inheritdoc cref="ILoggerCollection.Count" />
        /// <summary>
        /// Gets the number of elements contained in the <see cref="LoggerCollection"/>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="LoggerCollection"/>.</returns>
        public int Count
            => _collection.Count;

        /// <inheritdoc cref="ILoggerCollection.IsReadOnly" />
        /// <summary>
        /// IsReadOnly is a bool to indicate whether the collection is readonly
        /// </summary>
        /// <returns>true if the <see cref="LoggerCollection"/> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
            => _collection.IsReadOnly;

        #endregion PROPERTIES


        #region METHODS

        /// <inheritdoc cref="ILoggerCollection.Log{T}(T,ELogLevel,Exception)"/>
        /// <summary>
        /// Log&lt;T&gt; logs the message of an exception to the console.
        /// <para/>
        /// If the logLevel is <see cref="ELogLevel.Debug"/>, the messages are only shown in the debug build of the app.
        /// </summary>
        /// <typeparam name="T">is the type of the sender that wants to log the exception</typeparam>
        /// <param name="sender">is that wants to log the exception</param>
        /// <param name="logLevel">is the importance of the log</param>
        /// <param name="exception">is the exception to log the message of</param>
        public void Log<T>(T sender, ELogLevel logLevel, Exception exception)
        {
            // if the app is in debug mode and the loglevel is debug, do nothing
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            // log the exception-message to the console
            foreach (var logger in _collection)
                logger.Log(sender, logLevel, exception);
        }

        /// <inheritdoc cref="ILoggerCollection.Log{T}(T,ELogLevel,string)"/>
        /// <summary>
        /// Log&lt;T&gt; logs a message to the console.
        /// <para/>
        /// If the logLevel is <see cref="ELogLevel.Debug"/>, the messages are only shown in the debug build of the app.
        /// </summary>
        /// <typeparam name="T">is the type of the sender that wants to log the message</typeparam>
        /// <param name="sender">is that wants to log the exception</param>
        /// <param name="logLevel">is the importance of the log</param>
        /// <param name="message">is the message to log</param>
        public void Log<T>(T sender, ELogLevel logLevel, string message)
        {
            // if the app is in debug mode and the loglevel is debug, do nothing
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            // log the exception-message to the console
            foreach (var logger in _collection)
                logger.Log(sender, logLevel, message);
        }

        /// <inheritdoc cref="ILoggerCollection.Add" />
        /// <summary>
        /// Adds an item to the <see cref="LoggerCollection"/>.
        /// </summary>
        /// <param name="logger">is the logger to add</param>
        public void Add(ILogger logger)
            => _collection.Add(logger);

        /// <inheritdoc cref="ILoggerCollection.Remove" />
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="LoggerCollection"/>.
        /// </summary>
        /// <param name="logger">is the logger to remove</param>
        public bool Remove(ILogger logger)
            => _collection.Remove(logger);

        /// <inheritdoc cref="ILoggerCollection.Clear" />
        /// <summary>
        /// >Removes all items from the <see cref="LoggerCollection"/>.
        /// </summary>
        public void Clear()
            => _collection.Clear();

        /// <inheritdoc cref="ILoggerCollection.Contains" />
        /// <summary>
        /// Determines whether the <see cref="LoggerCollection"/> contains a specific logger.
        /// </summary>
        /// <param name="logger">is the logger to check</param>
        public bool Contains(ILogger logger)
            => _collection.Contains(logger);

        /// <inheritdoc cref="ILoggerCollection.CopyTo" />
        /// <summary>
        /// Copies the elements of the <see cref="LoggerCollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="LoggerCollection" />. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">he zero-based index in array at which copying begins.</param>
        public void CopyTo(ILogger[] array, int arrayIndex)
            => _collection.CopyTo(array, arrayIndex);

        /// <inheritdoc cref="ILoggerCollection.GetEnumerator" />
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ILogger> GetEnumerator()
            => _collection.GetEnumerator();

        /// <inheritdoc cref="ILoggerCollection.GetEnumerator" />
        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => _collection.GetEnumerator();

        #endregion METHODS
    }
}