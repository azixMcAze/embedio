﻿namespace Unosquare.Labs.EmbedIO.Modules
{
    using Constants;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    /// <summary>
    /// Simple authorization module that requests http auth from client
    /// will return 401 + WWW-Authenticate header if request isn't authorized.
    /// </summary>
    public class AuthModule : WebModuleBase
    {
        private readonly ConcurrentDictionary<string, string> _accounts = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthModule"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthModule(string username, string password)
            : this()
        {
            AddAccount(username, password);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthModule"/> class.
        /// </summary>
        public AuthModule()
        {
            AddHandler(ModuleMap.AnyPath, HttpVerbs.Any, (context, ct) =>
            {
                try
                {
                    if (!IsAuthorized(context.Request))
                        context.Response.StatusCode = 401;
                }
                catch (FormatException)
                {
                    // Credentials were not formatted correctly.
                    context.Response.StatusCode = 401;
                }

                if (context.Response.StatusCode != 401) return Task.FromResult(false);

                context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"Realm\"");

                return Task.FromResult(true);
            });
        }

        /// <inheritdoc />
        public override string Name => nameof(AuthModule);

        /// <summary>
        /// Validates request and returns <c>true</c> if that account data registered in this module and request has auth data.
        /// </summary>
        /// <param name="request">The HTTP Request.</param>
        /// <returns>
        /// <c>true</c> if request authorized, otherwise <c>false</c>.
        /// </returns>
        public bool IsAuthorized(IHttpRequest request)
        {
            try
            {
                var data = GetAccountData(request);

                if (!_accounts.TryGetValue(data.Key, out var password) || password != data.Value)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add new account.
        /// </summary>
        /// <param name="username">account username.</param>
        /// <param name="password">account password.</param>
        public void AddAccount(string username, string password) => _accounts.TryAdd(username, password);

        /// <summary>
        /// Parses request for account data.
        /// </summary>
        /// <param name="request">The HTTP Request.</param>
        /// <returns>user-password KeyValuePair from request.</returns>
        /// <exception>
        /// if request isn't authorized.
        /// </exception>
        private static KeyValuePair<string, string> GetAccountData(IHttpBase request)
        {
            var authHeader = request.Headers["Authorization"];
            if (authHeader == null) 
                throw new ArgumentException("Authorization header not found");

            var authHeaderParts = authHeader.Split(' ');

            // RFC 2617 sec 1.2, "scheme" name is case-insensitive
            // header contains name and parameter separated by space. If it equals just "basic" - it's empty
            if (!authHeaderParts[0].Equals("basic", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Authorization header not found");

            var credentials = Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(authHeaderParts[1]));

            var separator = credentials.IndexOf(':');
            var name = credentials.Substring(0, separator);
            var password = credentials.Substring(separator + 1);

            return new KeyValuePair<string, string>(name, password);
        }
    }
}
