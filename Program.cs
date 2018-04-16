using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpListenerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:23789/");
            listener.Start();
            Console.WriteLine("started.");

            Task task = new Task(() =>
            {
                bool isListening = true;
                while (isListening)
                {
                    try
                    {
                        var context = listener.GetContext();
                        context.Response.StatusCode = 200;

                        var writer = new StreamWriter(context.Response.OutputStream);

                        var name = context.Request.QueryString["name"];
                        if (string.IsNullOrEmpty(name))
                        {
                            writer.WriteLine("name is null or empty.");
                        }
                        else
                        {
                            writer.WriteLine("Hi, " + name);
                        }

                        writer.Close();
                    }
                    catch (HttpListenerException e)
                    {
                        if (e.ErrorCode == 995)
                        {
                            isListening = false;
                        }
                    }
                }
            });
            task.Start();

            var input = Console.ReadLine();
            while (!"exit".Equals(input))
            {
                input = Console.ReadLine();
            }
            listener.Stop();

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
