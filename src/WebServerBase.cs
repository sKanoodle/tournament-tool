using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GetFunc = System.Func<System.Threading.Tasks.Task<string>>;
using PostFunc = System.Func<System.Threading.Tasks.Task<string>>;

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
        public const string FORM_ID = "gorughw94seugbvur";
        private const int MAX_LISTENERS_COUNT = 10;
        private readonly string[] StaticFileExtensions = { ".css", ".js" };

        protected abstract string BaseUrl { get; }
        private readonly Dictionary<string, GetFunc> GetHandlers = new Dictionary<string, GetFunc>();
        private readonly Dictionary<string, PostFunc> PostHandlers = new Dictionary<string, PostFunc>();
        private readonly Dictionary<string, byte[]> StaticResources = new Dictionary<string, byte[]>();

        protected WebServerBase()
        {
            AddHandlers();
            LoadStaticResources();
        }

        public string SurroundWithBoilerplate(string content)
        {
            return $@"<HTML>
    <head>
        <link rel=""stylesheet"" href=""css/style.css"" type=""text/css"" />
        <script src=""js/auto-commit.js"" type=""text/javascript""></script>
    </head>
    <body>
        <form id=""{FORM_ID}"" method=""post"" action=""/auto-refresh""></form>
        {content}
    </body>
</HTML>";
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
                            PostHandlers.Add(attribute.Route, () => (Task<string>)method.Invoke(this, new object[] { }));
                            break;
                        default: throw new NotImplementedException();
                    }
            }
        }

        private async Task LoadStaticResources()
        {
            //.Replace('\\', '/')
            string currentFolder = Path.GetFullPath(".");
            string[] folders = { "css", "js" };

            foreach (var filePath in folders.SelectMany(f => Directory.GetFiles(f, "*", SearchOption.AllDirectories)))
            {
                string shortPath = filePath.Replace(currentFolder, String.Empty).Replace('\\', '/');

                using (var stream = File.OpenRead(filePath))
                using (var reader = new StreamReader(stream))
                {
                    byte[] bytes = new byte[stream.Length];
                    int offset = 0;
                    int writtenBytes = 0;
                    while ((writtenBytes = await stream.ReadAsync(bytes, offset, Math.Min(1024, (int)stream.Length - offset))) > 0)
                        offset += writtenBytes;

                    StaticResources.Add($"/{shortPath}", bytes);
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

        private async Task<bool> DeliverStaticContent(string extension, string contentType, HttpListenerResponse response, HttpListenerRequest request)
        {
            if (request.RawUrl.EndsWith(extension))
            {
                byte[] bytes = GetStaticContent(request.RawUrl);
                if (bytes is null)
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                else
                {
                    await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                    response.ContentType = contentType;
                }
                return true;
            }
            return false;
        }

        private async Task ProcessContext(HttpListenerContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    if (new []{ (".css", "text/css"), (".js", "text/javascript") }
                        .Any(t => DeliverStaticContent(t.Item1, t.Item2, context.Response, context.Request).Result))
                    {

                    }
                    else if (GetHandlers.TryGetValue(context.Request.RawUrl, out GetFunc getHandler))
                    {
                        using (var stream = context.Response.OutputStream)
                        using (var writer = new StreamWriter(stream))
                            await writer.WriteAsync(SurroundWithBoilerplate(await getHandler.Invoke()));
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
                        using (var stream = context.Request.InputStream)
                        using (var reader = new StreamReader(stream))
                            InputObject.Update(SplitFormData(await reader.ReadToEndAsync()));
                        context.Response.Redirect(await postHandler?.Invoke());
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

        private byte[] GetStaticContent(string file)
        {
            if (StaticResources.TryGetValue(file, out byte[] result))
                return result;
            System.Diagnostics.Debug.WriteLine($"static resource {file} not found");
            return null;
        }

        protected Dictionary<string, string> SplitFormData(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
                return null;
            return data.Split('&').Select(s => s.Split('=')).ToDictionary(a => a[0], a => a[1]);
        }
    }
}
