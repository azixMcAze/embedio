﻿using System;
using System.Threading.Tasks;
using EmbedIO.Testing.Internal;
using EmbedIO.Utilities;

namespace EmbedIO.Testing
{
    /// <summary>
    /// Represents a HTTP Client for unit testing.
    /// </summary>
    /// <seealso cref="IHttpContext" />
    public class TestHttpClient
    {
        private readonly TestWebServer _webServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHttpClient" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public TestHttpClient(TestWebServer server)
        {
            _webServer = server;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// A task representing the GET call.
        /// </returns>
        public async Task<string> GetAsync(string url = "")
        {
            var response = await SendAsync(new TestHttpRequest($"http://test/{url}")).ConfigureAwait(false);

            return response.GetBodyAsString();
        }

        /// <summary>
        /// Sends the HTTP request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A task representing the HTTP response.</returns>
        /// <exception cref="InvalidOperationException">The IWebServer implementation should be TestWebServer.</exception>
        public async Task<TestHttpResponse> SendAsync(TestHttpRequest request)
        {
            var context = new TestHttpContext(Validate.NotNull(nameof(request), request));

            _webServer.EnqueueContext(context);

            if (!(context.Response is TestHttpResponse response))
                throw new InvalidOperationException("The response object is invalid.");
            
            try
            {
                while (!response.IsClosed)
                    await Task.Delay(1).ConfigureAwait(false);
            }
            catch
            {
                // ignore
            }

            return response;
        }
    }
}