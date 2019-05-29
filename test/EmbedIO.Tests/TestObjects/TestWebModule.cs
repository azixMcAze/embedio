﻿using System.Threading;
using System.Threading.Tasks;

namespace EmbedIO.Tests.TestObjects
{
    public class TestWebModule : WebModuleBase
    {
        public const string RedirectUrl = "redirect";
        public const string RedirectAbsoluteUrl = "redirectAbsolute";
        public const string AnotherUrl = "anotherUrl";

        public TestWebModule()
            : base("/")
        {
        }

        protected override Task<bool> OnRequestAsync(IHttpContext context, string path, CancellationToken cancellationToken)
        {
            switch (path)
            {
                case RedirectAbsoluteUrl:
                case RedirectUrl:
                    context.Redirect("/" + AnotherUrl);
                    return Task.FromResult(true);
                case AnotherUrl:
                    return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}