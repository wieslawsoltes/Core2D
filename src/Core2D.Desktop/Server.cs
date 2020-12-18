using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using Core2D.ViewModels.Editor;

namespace Core2D.Desktop
{
    internal class RequestHandler
    {
        private  HttpListenerContext _context;

        public RequestHandler(HttpListenerContext context)
        {
            _context = context;
        }

        public async Task ProcessRequest()
        {
            var msg = $"{_context.Request.HttpMethod} {_context.Request.Url}";
            Console.WriteLine(msg);

            var url = _context.Request.RawUrl;
            if (url != null && url != "/" && url != "" && url != "/new" && url != "/close")
            {
                _context.Response.StatusCode = 401;
                _context.Response.Close();
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine("<html><body>");

            await Util.RunUiJob(() => 
            {
                var control = Repl.GetMainView();
                if (control is { })
                {
                    if (url == "/new")
                    {
                        var editor = control.DataContext as ProjectEditorViewModel;
                        editor?.OnNew(null);
                        Dispatcher.UIThread.RunJobs();
                    }
                    else if (url == "/close")
                    {
                        var editor = control.DataContext as ProjectEditorViewModel;
                        editor?.OnCloseProject();
                        Dispatcher.UIThread.RunJobs();
                    }
                    Console.WriteLine($"Rendering...");
                    var sw = new Stopwatch();
                    sw.Start();
                    var size = new Size(1366, 690);
                    using var stream = new MemoryStream();
                    Util.RenderAsSvg(control, size, stream);
                    stream.Position = 0;
                    using var reader = new StreamReader(stream);
                    var svg = reader.ReadToEnd();
                    sb.AppendLine(svg);
                    sw.Stop();
                    Console.WriteLine($"Done in {sw.ElapsedMilliseconds}ms");
                }
            });

            sb.AppendLine("</body></html>");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            _context.Response.ContentLength64 = bytes.Length;
            _context.Response.ContentType = "text/html";
            _context.Response.StatusCode = 200;
            _context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            _context.Response.OutputStream.Close();
        }
    }

    internal static class Server
    {
        public static async Task Run()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();

            Console.WriteLine("Listening...");

            while(true)
            {
                var ctx = await listener.GetContextAsync();
                await new RequestHandler(ctx).ProcessRequest();
            }
        }
    }
}
