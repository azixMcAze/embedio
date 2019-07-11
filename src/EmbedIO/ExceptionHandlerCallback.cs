﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EmbedIO
{
    /// <summary>
    /// A callback used to provide information about an unhandled exception occurred while processing a request.
    /// </summary>
    /// <param name="context">A <see cref="IHttpContext" /> interface representing the context of the request.</param>
    /// <param name="exception">The unhandled exception.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to cancel the operation.</param>
    /// <returns>A <see cref="Task" /> representing the ongoing operation.</returns>
    /// <remarks>
    /// <para>When this delegate is called, the response's status code has already been set to
    /// <see cref="HttpStatusCode.InternalServerError" />.</para>
    /// <para>Any exception thrown by a handler (even a HTTP exception) will go unhandled: the web server
    /// will not crash, but processing of the request will be aborted, and the response will be flushed as-is.
    /// In other words, it is not a good ides to <c>throw HttpException.NotFound()</c> (or similar)
    /// from a handler.</para>
    /// </remarks>
    public delegate Task ExceptionHandlerCallback(IHttpContext context, Exception exception, CancellationToken cancellationToken);
}