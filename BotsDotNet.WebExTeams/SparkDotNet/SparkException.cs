﻿using System;
using System.Net.Http.Headers;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    /// <summary>
    /// Provides a Spark Exception class which will return the original headers for a request
    /// </summary>
    public class SparkException : Exception
    {
        /// <summary>
        /// Gets or sets the error code (HTTP status code)
        /// </summary>
        /// <value>The error code (HTTP status code).</value>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the Http Response Headers
        /// </summary>
        /// <value>HttpResponseHeaders.</value>
        public HttpResponseHeaders Headers { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SparkException class.
        /// </summary>
        public SparkException() { }

        /// <summary>
        /// Initializes a new instance of the SparkException class.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="message">Error message.</param>
        public SparkException(int statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the SparkException class.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="message">Error message.</param>
        /// <param name="headers">HTTP Headers.</param>
        public SparkException(int statusCode, string message, HttpResponseHeaders headers = null) : base(message)
        {
            this.StatusCode = statusCode;
            this.Headers = headers;
        }
    }
}
