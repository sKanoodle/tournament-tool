using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GetFunc = System.Func<System.Threading.Tasks.Task<string>>;
using PostFunc = System.Func<System.Collections.Generic.Dictionary<string, string>, System.Threading.Tasks.Task<string>>;

namespace TournamentTool
{
    class RouteHandlerAttribute : Attribute
    {
        public string Route { get; }
        public HttpMethodType HttpMethod { get; }

        public RouteHandlerAttribute(string route) : this(route, HttpMethodType.GET) { }

        public RouteHandlerAttribute(string route, HttpMethodType httpMethod)
        {
            Route = route;
            HttpMethod = httpMethod;
        }

        public enum HttpMethodType
        {
            GET = 0,
            //HEAD = 1,
            POST = 2,
            //PUT = 3,
            //DELETE = 4,
            //CONNECT = 5,
            //OPTIONS = 6,
            //TRACE = 7,
            //PATCH = 8,
        }
    }

    abstract class WebServerBase
    {
        private const int MAX_LISTENERS_COUNT = 10;

        protected abstract string BaseUrl { get; }
        private readonly Dictionary<string, GetFunc> GetHandlers = new Dictionary<string, GetFunc>();
        private readonly Dictionary<string, PostFunc> PostHandlers = new Dictionary<string, PostFunc>();

        protected WebServerBase()
        {
            AddHandlers();
        }

        private void AddHandlers()
        {
            foreach (var method in GetType().GetMethods())
            {
                if (Attribute.GetCustomAttribute(method, typeof(RouteHandlerAttribute)) is RouteHandlerAttribute attribute)
                    switch(attribute.HttpMethod)
                    {
                        case RouteHandlerAttribute.HttpMethodType.GET:
                            GetHandlers.Add(attribute.Route, () => (Task<string>)method.Invoke(this, new object[] { }));
                            break;
                        case RouteHandlerAttribute.HttpMethodType.POST:
                            PostHandlers.Add(attribute.Route, s => (Task<string>)method.Invoke(this, new object[] { s }));
                            break;
                        default: throw new NotImplementedException();
                    }
            }
        }

        public async Task RunServerAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(BaseUrl);
            listener.Start();
            SemaphoreSlim s = new SemaphoreSlim(MAX_LISTENERS_COUNT);
            while (true)
            {
                await s.WaitAsync();
#pragma warning disable CS4014 // this is not awaited, to spawn more listeners while this one in still listening
                listener.GetContextAsync().ContinueWith(async t =>
                {
                    await ProcessContext(await t);
                    s.Release();
                });
#pragma warning restore CS4014
            }
        }

        private async Task ProcessContext(HttpListenerContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    if (GetHandlers.TryGetValue(context.Request.RawUrl, out GetFunc getHandler))
                    {
                        using (var stream = context.Response.OutputStream)
                        using (var writer = new StreamWriter(stream))
                            await writer.WriteAsync(await getHandler.Invoke());
                        context.Response.ContentType = "text/html";
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    context.Response.Close();
                    break;
                case "HEAD": throw new NotImplementedException();
                case "POST":
                    if (PostHandlers.TryGetValue(context.Request.RawUrl, out PostFunc postHandler))
                    {
                        string redirectUrl;
                        using (var stream = context.Request.InputStream)
                        using (var reader = new StreamReader(stream))
                            redirectUrl = await postHandler?.Invoke(SplitFormData(await reader.ReadToEndAsync()));
                        context.Response.Redirect(redirectUrl);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                    context.Response.Close();
                    break;
                case "PUT": throw new NotImplementedException();
                case "DELETE": throw new NotImplementedException();
                case "CONNECT": throw new NotImplementedException();
                case "OPTIONS": throw new NotImplementedException();
                case "TRACE": throw new NotImplementedException();
                case "PATCH": throw new NotImplementedException();
                default: throw new ArgumentException();
            }
        }

        protected Dictionary<string, string> SplitFormData(string data)
        {
            return data.Split('&').Select(s => s.Split('=')).ToDictionary(a => a[0], a => a[1]);
        }
    }
}
