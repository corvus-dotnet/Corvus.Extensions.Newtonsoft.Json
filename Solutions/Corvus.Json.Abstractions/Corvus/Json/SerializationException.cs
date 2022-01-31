// <copyright file="SerializationException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Json
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a failure occurs while attempting to serailize or deserialize data.
    /// </summary>
    [Serializable]
    public class SerializationException : Exception
    {
        /// <summary>
        /// Creates a <see cref="SerializationException"/>.
        /// </summary>
        /// <param name="innerException">The underlying exception.</param>
        public SerializationException(Exception innerException)
            : base("Serialization error", innerException)
        {
        }

        /// <summary>
        /// Creates a <see cref="SerializationException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a <see cref="SerializationException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The underlying exception.</param>
        public SerializationException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a <see cref="SerializationException"/> when the exception itself has
        /// been serialized.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected SerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}