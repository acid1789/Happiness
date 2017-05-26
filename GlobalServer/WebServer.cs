using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using NetworkCore;

namespace GlobalServer
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            //if (method == null)
            //    throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);


            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = (ctx.Request.RawUrl.Contains("/checkout")) ? CheckoutResponse(ctx.Request) : PurchaseResponse(ctx.Request);

                                //string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        string CheckoutResponse(HttpListenerRequest request)
        {
            string post = "";
            if (request.HttpMethod == "POST")
            {
                using (System.IO.StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    post = reader.ReadToEnd();
                }
            }
            System.Diagnostics.Debug.WriteLine(post);
            try
            {
                string[] pieces = post.Split('&');
                Dictionary<string, string> args = new Dictionary<string, string>();
                foreach (string piece in pieces)
                {
                    string[] kv = piece.Split('=');
                    args[kv[0]] = kv[1];
                }

                string purchaseResult = BraintreeManager.DoPurchase(args["payment_method_nonce"], args["uid"], args["pid"]);
                return string.Format("<HTML><BODY>Thank you!<br>{0}</BODY></HTML>", purchaseResult);
            }
            catch (Exception ex)
            {
                return string.Format("<HTML><BODY>Data Error <br /> {0}</BODY></HTML>", ex.ToString());
            }
        }

        string PurchaseResponse(HttpListenerRequest request)
        {
            //return string.Format("<HTML><BODY>My web page.<br>{0}<br>{1}<br>{2}</BODY></HTML>", DateTime.Now, request.QueryString["uid"], request.QueryString["pid"]);

            System.Diagnostics.Debug.WriteLine(request.RawUrl);
            GlobalProduct prod = Marketplace.Instance.GetProduct(request.QueryString["pid"]);
            if( prod == null )
                return "<HTML><BODY>Invalid Product</BODY></HTML>";

            string html = LoadPurchaseHtml();
            html = html.Replace("CLIENT_AUTH_TOKEN", BraintreeManager.GetClientToken(request.QueryString["uid"]));
            html = html.Replace("PAYPAL_AMOUNT_TOKEN", prod.USD.ToString());
            html = html.Replace("PAYPAL_CURRENCY_TOKEN", "USD");
            
            html = html.Replace("COIN_AMMOUNT", prod.Coins.ToString());
            html = html.Replace("VIP_AMMOUNT", prod.VIP.ToString());
            html = html.Replace("USD_AMMOUNT", prod.USD.ToString());

            html = html.Replace("USER_ID", request.QueryString["uid"]);
            html = html.Replace("PRODUCT_ID", request.QueryString["pid"]);

            return html;
        }

        static string LoadPurchaseHtml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string result = null;

            using (Stream stream = assembly.GetManifestResourceStream("GlobalServer.Purchase.html"))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
