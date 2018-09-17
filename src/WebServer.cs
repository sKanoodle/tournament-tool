using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTool
{
    class WebServer : WebServerBase
    {
        protected override string BaseUrl => "http://127.0.0.1:22542/";

        [RouteHandler("/")]
        public async Task<string> GetLandingPage()
        {
            return @"<HTML>
    <head>
    </head>
    <body>
        HELLO WORLD!
        <form id=""random_id"" method=""post"">
            foo: <input type=""text"" name=""foo""><br />
            bar: <input type=""text"" name=""bar""><br />
        </form>

        <button form=""random_id"" type=""submit"" formaction=""/post-item/"">
            Submit
        </button>
    </body>
</HTML>";
        }

        [RouteHandler("/post-item/", RouteHandlerAttribute.HttpMethodType.POST)]
        public async Task<string> PostItemPage(string postObject)
        {
            return "/item/";
        }

        [RouteHandler("/item/")]
        public async Task<string> GetItemPage()
        {
            return "<HTML><head></head><body>item posted</body></HTML>";
        }
    }
}
