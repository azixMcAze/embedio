﻿using System;
using System.Net;

namespace EmbedIO
{
    partial class HttpException
    {
        /// <summary>
        /// Returns a new instance of <see cref="HttpException"/> that, when thrown,
        /// will break the request handling control flow and send a <c>401 Unauthorized</c>
        /// response to the client.
        /// </summary>
        /// <returns>A newly-created <see cref="HttpException"/>.</returns>
        public static HttpException Unauthorized() => new HttpException(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Returns a new instance of <see cref="HttpException"/> that, when thrown,
        /// will break the request handling control flow and send a <c>403 Forbidden</c>
        /// response to the client.
        /// </summary>
        /// <returns>A newly-created <see cref="HttpException"/>.</returns>
        public static HttpException Forbidden() => new HttpException(HttpStatusCode.Forbidden);

        /// <summary>
        /// Returns a new instance of <see cref="HttpException"/> that, when thrown,
        /// will break the request handling control flow and send a <c>400 Bad Request</c>
        /// response to the client.
        /// </summary>
        /// <returns>A newly-created <see cref="HttpException"/>.</returns>
        public static HttpException BadRequest() => new HttpException(HttpStatusCode.BadRequest);

        /// <summary>
        /// Returns a new instance of <see cref="HttpException"/> that, when thrown,
        /// will break the request handling control flow and send a <c>404 Not Found</c>
        /// response to the client.
        /// </summary>
        /// <returns>A newly-created <see cref="HttpException"/>.</returns>
        public static HttpException NotFound() => new HttpException(HttpStatusCode.NotFound);

        /// <summary>
        /// Returns a new instance of <see cref="HttpException"/> that, when thrown,
        /// will break the request handling control flow and send a <c>405 Method Not Allowed</c>
        /// response to the client.
        /// </summary>
        /// <returns>A newly-created <see cref="HttpException"/>.</returns>
        public static HttpException MethodNotAllowed() => new HttpException(HttpStatusCode.MethodNotAllowed);

        /// <summary>
        /// Returns a new instance of <see cref="HttpException"/> that, when thrown,
        /// will break the request handling control flow and send a <c>406 Not Acceptable</c>
        /// response to the client.
        /// </summary>
        /// <returns>A newly-created <see cref="HttpException"/>.</returns>
        public static HttpException NotAcceptable() => new HttpException(HttpStatusCode.NotAcceptable);

        /// <summary>
        /// Returns a new instance of <see cref="HttpRedirectException" /> that, when thrown,
        /// will break the request handling control flow and redirect the client
        /// to the specified location, using the specified response status code.
        /// </summary>
        /// <param name="location">The redirection target.</param>
        /// <param name="statusCode">
        /// <para>The status code to set on the response, in the range from 300 to 399.</para>
        /// <para>By default, status code 302 (<c>Found</c>) is used.</para>
        /// </param>
        /// <returns>
        /// A newly-created <see cref="HttpRedirectException" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="statusCode"/> is not in the 300-399 range.</exception>
        public static HttpRedirectException Redirect(string location, int statusCode = (int)HttpStatusCode.Found)
            => new HttpRedirectException(location, statusCode);

        /// <summary>
        /// Returns a new instance of <see cref="HttpRedirectException" /> that, when thrown,
        /// will break the request handling control flow and redirect the client
        /// to the specified location, using the specified response status code.
        /// </summary>
        /// <param name="location">The redirection target.</param>
        /// <param name="statusCode">
        /// <para>One of the redirection status codes, to be set on the response.</para>
        /// <para>By default, <see cref="HttpStatusCode.Found"/> is used.</para>
        /// </param>
        /// <returns>
        /// A newly-created <see cref="HttpRedirectException" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="statusCode"/> is not a redirection status code.</exception>
        public static HttpRedirectException Redirect(string location, HttpStatusCode statusCode = HttpStatusCode.Found)
            => new HttpRedirectException(location, statusCode);
    }
}