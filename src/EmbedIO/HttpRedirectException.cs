﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace EmbedIO
{
    /// <summary>
    /// When thrown, breaks the request handling control flow
    /// and sends a redirection response to the client.
    /// </summary>
    public class HttpRedirectException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRedirectException"/> class.
        /// </summary>
        /// <param name="location">The redirection target.</param>
        /// <param name="statusCode">
        /// <para>The status code to set on the response, in the range from 300 to 399.</para>
        /// <para>By default, status code 302 (<c>Found</c>) is used.</para>
        /// </param>
        /// <exception cref="ArgumentException"><paramref name="statusCode"/> is not in the 300-399 range.</exception>
        public HttpRedirectException(string location, int statusCode = (int)HttpStatusCode.Found)
            : base(statusCode)
        {
            if (statusCode < 300 || statusCode > 399)
                throw new ArgumentException("Redirect status code is not valid.", nameof(statusCode));

            Location = location;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRedirectException"/> class.
        /// </summary>
        /// <param name="location">The redirection target.</param>
        /// <param name="statusCode">
        /// <para>One of the redirection status codes, to be set on the response.</para>
        /// <para>By default, <see cref="HttpStatusCode.Found"/> is used.</para>
        /// </param>
        /// <exception cref="ArgumentException"><paramref name="statusCode"/> is not a redirection status code.</exception>
        public HttpRedirectException(string location, HttpStatusCode statusCode = HttpStatusCode.Found)
            : this(location, (int)statusCode)
        {
        }

        /// <summary>
        /// Gets the URL where the client will be redirected.
        /// </summary>
        public string Location { get; }

        /// <inheritdoc />
        protected override Task OnSendResponseAsync(IHttpContext context)
        {
            context.Redirect(Location, StatusCode);
            return Task.CompletedTask;
        }
    }
}