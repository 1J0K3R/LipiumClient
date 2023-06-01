using LipiumClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LipiumClient
{
    internal class Program
    {

        public static HttpListener listener;
        public static string url = "http://25.32.62.103:8000/";
        //public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>Lipium Client</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Id Expediteur: {0}</p>" +
            "    <p>Id Receveur: {1}</p>" +
            "    <p>Montant: {2}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {3}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        // http://...:8000/transaction?idexp=..&idrcv=..&montant=0-9
        //Ex : http://25.32.62.103:8000/transaction?idexp='a1'&idrcv='b2'&montant=9

        // input : montant = montant
        //         idExpediteur = idexp
        //         idReceveur = idrcv

        // génération : idTransaction

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                if (req.Url.AbsolutePath == "/transaction")
                {
                    Transaction transaction = new Transaction(req);
                    byte[] data;
                    if (transaction.isNullOrEmpty())
                    {
                        data = Encoding.UTF8.GetBytes("Error, there is a missing parameter");
                    }
                    else
                    {
                        data = Encoding.UTF8.GetBytes("All good, transaction received. \n idExp is " + transaction.IdExp + "\n idRcv is " + transaction.IdRcv + "\n montant " + transaction.Montant);
                        string jsonTransaction = Transaction.getJson(transaction);
                        
                        string sha256Transaction = "";  // faire un sha256 à partir du json de transaction

                        HttpClient httpClient = new HttpClient();
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(new Uri($"http://25.29.51.211:8000/mine?idTrans={jsonTransaction}&oTrans=azerty"));
                        var result = await httpResponseMessage.Content.ReadAsStringAsync();
                        data = Encoding.UTF8.GetBytes(result);
                    }

                    // Write the response info
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
                else
                {
                    // Write the response info
                    string disableSubmit = !runServer ? "disabled" : "";
                    byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, "", "", "", disableSubmit));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
            }
        }

        static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}
