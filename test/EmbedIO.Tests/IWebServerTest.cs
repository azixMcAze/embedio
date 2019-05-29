﻿using System.Threading.Tasks;
using EmbedIO.Actions;
using EmbedIO.Sessions;
using EmbedIO.Utilities;
using EmbedIO.Tests.TestObjects;
using NUnit.Framework;
using Unosquare.Swan.Formatters;

namespace EmbedIO.Tests
{
    public class IWebServerTest
    {
        [Test]
        public void SetupInMemoryWebServer_ReturnsValidInstance()
        {
            using (var webserver = new TestWebServer())
            {
                Assert.IsNotNull(webserver);
            }
        }

        [Test]
        public void AddModule_ReturnsValidInstance()
        {
            using (var webserver = new TestWebServer())
            {
                webserver.Modules.Add(new ActionModule("/", HttpVerbs.Any, (ctx, path, ct) => ctx.JsonResponseAsync(nameof(TestWebServer), ct)));

                Assert.AreEqual(1, webserver.Modules.Count);
            }
        }

        [Test]
        public void SetSessionManager_ReturnsValidInstance()
        {
            using (var webserver = new TestWebServer())
            {
                webserver.SessionManager = new LocalSessionManager();

                Assert.NotNull(webserver.SessionManager);
            }
        }

        [Test]
        public void SetSessionManagerToNull_ReturnsValidInstance()
        {
            using (var webserver = new TestWebServer())
            {
                webserver.SessionManager = new LocalSessionManager();
                webserver.SessionManager = null;

                Assert.IsNull(webserver.SessionManager);
            }
        }

        [Test]
        public async Task RunsServerAndRequestData_ReturnsValidData()
        {
            using (var webserver = new TestWebServer())
            {
                webserver.OnAny((ctx, path, ct) => ctx.JsonResponseAsync(new Person {Name = nameof(Person)}, ct));

#pragma warning disable 4014
                webserver.RunAsync();
#pragma warning restore 4014

                var client = webserver.GetClient();

                var data = await client.GetAsync("/");
                Assert.IsNotNull(data);

                var person = Json.Deserialize<Person>(data);
                Assert.IsNotNull(person);

                Assert.AreEqual(person.Name, nameof(Person));
            }
        }
    }
}