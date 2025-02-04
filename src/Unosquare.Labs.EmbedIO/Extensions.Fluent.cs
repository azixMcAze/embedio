﻿namespace Unosquare.Labs.EmbedIO
{
    using Constants;
    using Modules;
    using System;
    using System.Collections.Generic;
    using Swan;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extensions methods to EmbedIO's Fluent Interface.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Add the StaticFilesModule to the specified WebServer.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="rootPath">The static folder path.</param>
        /// <param name="defaultDocument">The default document name.</param>
        /// <param name="useDirectoryBrowser">if set to <c>true</c> [use directory browser].</param>
        /// <returns>
        /// An instance of webserver.
        /// </returns>
        /// <exception cref="ArgumentNullException">webserver.</exception>
        public static IWebServer WithStaticFolderAt(
            this IWebServer webserver,
            string rootPath,
            string defaultDocument = StaticFilesModule.DefaultDocumentName,
            bool useDirectoryBrowser = false)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            webserver.RegisterModule(
                new StaticFilesModule(rootPath, useDirectoryBrowser) {DefaultDocument = defaultDocument});
            return webserver;
        }

        /// <summary>
        /// Add the StaticFilesModule with multiple paths.
        /// </summary>
        /// <param name="webserver">The webserver.</param>
        /// <param name="virtualPaths">The virtual paths.</param>
        /// <param name="defaultDocument">The default document.</param>
        /// <returns>An instance of a web module.</returns>
        /// <exception cref="System.ArgumentNullException">webserver.</exception>
        public static IWebServer WithVirtualPaths(
            this IWebServer webserver,
            Dictionary<string, string> virtualPaths,
            string defaultDocument = StaticFilesModule.DefaultDocumentName)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            webserver.RegisterModule(new StaticFilesModule(virtualPaths) {DefaultDocument = defaultDocument});
            return webserver;
        }

        /// <summary>
        /// Add StaticFilesModule to WebServer.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <returns>An instance of a web module.</returns>
        /// <exception cref="System.ArgumentNullException">webserver.</exception>
        public static IWebServer WithLocalSession(this IWebServer webserver)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            webserver.RegisterModule(new LocalSessionModule());
            return webserver;
        }

        /// <summary>
        /// Add WebApiModule to WebServer.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="assembly">The assembly to load WebApi Controllers from. Leave null to avoid autoloading.</param>
        /// <param name="responseJsonException">if set to <c>true</c> [response json exception].</param>
        /// <returns>
        /// An instance of webserver.
        /// </returns>
        /// <exception cref="ArgumentNullException">webserver.</exception>
        public static IWebServer WithWebApi(this IWebServer webserver, Assembly assembly = null, bool responseJsonException = false)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            webserver.RegisterModule(new WebApiModule());
            return assembly != null ? webserver.LoadApiControllers(assembly, responseJsonException) : webserver;
        }

        /// <summary>
        /// Add WebSocketsModule to WebServer.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="assembly">The assembly to load Web Sockets from. Leave null to avoid autoloading.</param>
        /// <returns>An instance of webserver.</returns>
        /// <exception cref="System.ArgumentNullException">webserver.</exception>
        public static IWebServer WithWebSocket(this IWebServer webserver, Assembly assembly = null)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            webserver.RegisterModule(new WebSocketsModule());
            return assembly != null ? webserver.LoadWebSockets(assembly) : webserver;
        }

        /// <summary>
        /// Load all the WebApi Controllers in an assembly.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="assembly">The assembly to load WebApi Controllers from. Leave null to load from the currently executing assembly.</param>
        /// <param name="responseJsonException">if set to <c>true</c> [response json exception].</param>
        /// <returns>
        /// An instance of webserver.
        /// </returns>
        /// <exception cref="ArgumentNullException">webserver.</exception>
        /// <exception cref="System.ArgumentNullException">webserver.</exception>
        public static IWebServer LoadApiControllers(this IWebServer webserver, Assembly assembly = null, bool responseJsonException = false)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            var types = (assembly ?? Assembly.GetEntryAssembly()).GetTypes();
            var apiControllers = types
                .Where(x => x.GetTypeInfo().IsClass
                                 && !x.GetTypeInfo().IsAbstract
                                 && x.GetTypeInfo().IsSubclassOf(typeof(WebApiController)))
                .ToArray();
            
            foreach (var apiController in apiControllers)
            {
                if (webserver.Module<WebApiModule>() == null) 
                    webserver = webserver.WithWebApi(responseJsonException: responseJsonException);

                webserver.Module<WebApiModule>().RegisterController(apiController);
                $"Registering WebAPI Controller '{apiController.Name}'".Debug(nameof(LoadApiControllers));
            }

            return webserver;
        }

        /// <summary>
        /// Load all the WebApi Controllers in an assembly.
        /// </summary>
        /// <param name="apiModule">The Web API Module instance.</param>
        /// <param name="assembly">The assembly to load WebApi Controllers from. Leave null to load from the currently executing assembly.</param>
        /// <returns>The webserver instance.</returns>
        /// <exception cref="System.ArgumentNullException">webserver.</exception>
        public static WebApiModule LoadApiControllers(this WebApiModule apiModule, Assembly assembly = null)
        {
            if (apiModule == null)
                throw new ArgumentNullException(nameof(apiModule));

            var types = (assembly ?? Assembly.GetEntryAssembly()).GetTypes();
            var apiControllers = types
                .Where(x => x.GetTypeInfo().IsClass
                                 && !x.GetTypeInfo().IsAbstract
                                 && x.GetTypeInfo().IsSubclassOf(typeof(WebApiController)))
                .ToArray();
            
            foreach (var apiController in apiControllers)
            {
                apiModule.RegisterController(apiController);
            }

            return apiModule;
        }

        /// <summary>
        /// Load all the WebSockets in an assembly.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="assembly">The assembly to load WebSocketsServer types from. Leave null to load from the currently executing assembly.</param>
        /// <returns>An instance of webserver.</returns>
        /// <exception cref="System.ArgumentNullException">webserver.</exception>
        public static IWebServer LoadWebSockets(this IWebServer webserver, Assembly assembly = null)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            var types = (assembly ?? Assembly.GetEntryAssembly()).GetTypes();
            
            foreach (var socketServer in types.Where(x => x.GetTypeInfo().BaseType == typeof(WebSocketsServer)))
            {
                if (webserver.Module<WebSocketsModule>() == null) webserver = webserver.WithWebSocket();

                webserver.Module<WebSocketsModule>().RegisterWebSocketsServer(socketServer);
                $"Registering WebSocket Server '{socketServer.Name}'".Debug(nameof(LoadWebSockets));
            }

            return webserver;
        }

        /// <summary>
        /// Enables CORS in the WebServer.
        /// </summary>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="origins">The valid origins, default all.</param>
        /// <param name="headers">The valid headers, default all.</param>
        /// <param name="methods">The valid method, default all.</param>
        /// <returns>An instance of the tiny web server used to handle request.</returns>
        /// <exception cref="ArgumentNullException">webserver.</exception>
        public static IWebServer EnableCors(
            this IWebServer webserver,
            string origins = Strings.CorsWildcard,
            string headers = Strings.CorsWildcard,
            string methods = Strings.CorsWildcard)
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            webserver.RegisterModule(new CorsModule(origins, headers, methods));

            return webserver;
        }

        /// <summary>
        /// Add WebApi Controller to WebServer.
        /// </summary>
        /// <typeparam name="T">The type of Web API Controller.</typeparam>
        /// <param name="webserver">The webserver instance.</param>
        /// <param name="responseJsonException">if set to <c>true</c> [response json exception].</param>
        /// <returns>
        /// An instance of webserver.
        /// </returns>
        /// <exception cref="ArgumentNullException">webserver.</exception>
        public static IWebServer WithWebApiController<T>(this IWebServer webserver, bool responseJsonException = false)
            where T : WebApiController
        {
            if (webserver == null)
                throw new ArgumentNullException(nameof(webserver));

            if (webserver.Module<WebApiModule>() == null)
            {
                webserver.RegisterModule(new WebApiModule(responseJsonException));
            }

            webserver.Module<WebApiModule>().RegisterController<T>();

            return webserver;
        }

        /// <summary>
        /// Creates an instance of <see cref="ActionModule" /> and adds it to a module container.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="baseUrlPath">The base URL path.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>
        ///   <paramref name="this" /> with a <see cref="ActionModule" /> added.
        /// </returns>
        /// <exception cref="ArgumentNullException">webserver</exception>
        public static IWebServer WithAction(this IWebServer @this, string baseUrlPath, HttpVerbs verb, WebHandler handler)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            @this.RegisterModule(new ActionModule(baseUrlPath, verb, handler));

            return @this;
        }
    }
}