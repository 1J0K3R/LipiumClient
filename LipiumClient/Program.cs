using LipiumClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

                byte[] data = Encoding.UTF8.GetBytes("Nothing");

                
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    // reception d'une requete "/shutdown"
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }
                else if (req.Url.AbsolutePath == "/soldeAccount")
                {
                    // reception d'une requete "/soldeAccount"
                    string idAccount = req.QueryString["idaccount"];
                    if(string.IsNullOrEmpty(idAccount))
                    {
                        data = Encoding.UTF8.GetBytes("Error, there is a missing parameter or bad parameter.");
                    }
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(new Uri("http://25.28.20.82:8000/lastblock?blockId=1&blockNb=1&blockInfo=Lipum")); //http://25.28.20.82:8000/lastblock
                    var result = await httpResponseMessage.Content.ReadAsStringAsync();
                    Root root = JsonSerializer.Deserialize<Root>(result);

                    decimal solde = 0;
                    // Calcul le solde global d'un compte (à partir de son id)
                    foreach(var block in root.Blocks)
                    {
                        foreach(var transaction in block.Transactions)
                        {
                            // je test que le compte ne soit pas l'id recepteur et l'id expediteur
                            if(!(transaction.IdRcv == idAccount & transaction.IdExp == idAccount))
                            {
                                // si le compte est l'id receveur alors j'ajoute le montant à son solde
                                if (transaction.IdRcv == idAccount)
                                {
                                    solde += transaction.Montant;
                                }

                                // si le compte est l'id expediteur alors je soustrais le montant à son solde
                                if (transaction.IdExp == idAccount)
                                {
                                    solde -= transaction.Montant;
                                }
                            }                            
                        }
                    }
                    data = Encoding.UTF8.GetBytes($"Votre solde total est de : {solde}");
                }
                else if (req.Url.AbsolutePath == "/transaction")
                {
                    // reception d'une requete "/transaction"
                    Transaction transaction = new Transaction(req);
                    if (transaction.isNullEmptyOr0())
                    {
                        // Réponse retourner en cas d'erreur
                        data = Encoding.UTF8.GetBytes("Error, there is a missing parameter or bad parameter.");
                    }
                    else
                    {
                        // Réponse retourner en cas de succès
                        data = Encoding.UTF8.GetBytes("All good, transaction received. \n idExp is " + transaction.IdExp + "\n idRcv is " + transaction.IdRcv + "\n montant " + transaction.Montant);
                        
                        string jsonTransaction = Transaction.getJson(transaction);
                        string hashTransaction = Transaction.getHash(jsonTransaction);

                        HttpClient httpClient = new HttpClient();                        
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(new Uri($"http://25.29.51.211:8000/mine?idTrans={hashTransaction}&oTrans={jsonTransaction}"));
                        var result = await httpResponseMessage.Content.ReadAsStringAsync();
                        data = Encoding.UTF8.GetBytes(result);
                    }
                }
                else
                {
                    // Write the response info
                    string disableSubmit = !runServer ? "disabled" : "";
                    data = Encoding.UTF8.GetBytes(String.Format(pageData, "", "", "", disableSubmit));
                }
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;

                    // Write out to the response stream (asynchronously), then close it
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
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
