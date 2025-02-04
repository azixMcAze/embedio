﻿namespace Unosquare.Labs.EmbedIO.Modules
{
    using Constants;
    using EmbedIO;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a simple module to server resource files from the .NET assembly.
    /// </summary>
    [Obsolete("This class will be replaced by FileModule")]
    public class ResourceFilesModule
        : FileModuleBase
    {
        private readonly Assembly _sourceAssembly;
        private readonly string _resourcePathRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFilesModule" /> class.
        /// </summary>
        /// <param name="sourceAssembly">The source assembly.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <param name="headers">The headers.</param>
        /// <exception cref="ArgumentNullException">sourceAssembly.</exception>
        /// <exception cref="ArgumentException">Path ' + fileSystemPath + ' does not exist.</exception>
        public ResourceFilesModule(
            Assembly sourceAssembly,
            string resourcePath,
            IDictionary<string, string> headers = null)
        {
            if (sourceAssembly == null)
                throw new ArgumentNullException(nameof(sourceAssembly));

            if (sourceAssembly.GetName() == null)
                throw new ArgumentException($"Assembly '{sourceAssembly}' is not valid.");

            UseGzip = true;
            _sourceAssembly = sourceAssembly;
            _resourcePathRoot = resourcePath;

            headers?.ForEach(DefaultHeaders.Add);

            AddHandler(ModuleMap.AnyPath, HttpVerbs.Head, (context, ct) => HandleGet(context, ct, false));
            AddHandler(ModuleMap.AnyPath, HttpVerbs.Get, (context, ct) => HandleGet(context, ct));
        }

        /// <inheritdoc />
        public override string Name => nameof(ResourceFilesModule);

        private static string FixPath(string s) => s == "/" ? "index.html" : s.Substring(1, s.Length - 1).Replace('/', '.');

        private async Task<bool> HandleGet(IHttpContext context, CancellationToken ct, bool sendBuffer = true)
        {
            Stream buffer = null;

            try
            {
                var localPath = FixPath(context.RequestPathCaseSensitive());
                var partialHeader = context.RequestHeader(HttpHeaderNames.Range);

                $"Resource System: {localPath}".Debug(nameof(ResourceFilesModule));

                buffer = _sourceAssembly.GetManifestResourceStream($"{_resourcePathRoot}.{localPath}");

                // If buffer is null something is really wrong
                if (buffer == null)
                {
                    return false;
                }

                // check to see if the file was modified or e-tag is the same
                var utcFileDateString = DateTime.Now.ToUniversalTime()
                    .ToString(Strings.BrowserTimeFormat, Strings.StandardCultureInfo);

                context.Response.ContentLength64 = buffer.Length;

                SetGeneralHeaders(context.Response, utcFileDateString, localPath.Contains(".") ? $".{localPath.Split('.').Last()}" : ".html");

                if (sendBuffer)
                {
                    await WriteFileAsync(
                        partialHeader, 
                        context.Response, 
                        buffer, 
                        context.AcceptGzip(buffer.Length),
                        ct)
                        .ConfigureAwait(false);
                }
            }
            catch (System.Net.HttpListenerException)
            {
                // Connection error, nothing else to do
            }
            catch (Net.HttpListenerException)
            {
                // Connection error, nothing else to do
            }
            finally
            {
                buffer?.Dispose();
            }

            return true;
        }
    }
}