using System;
using System.Net;
using System.Text.Json;

namespace LipiumClient.Models
{
    public class Transaction
    {
        public string IdExp { get; private set; }
        public string IdRcv { get; private set; }
        public string Montant { get; private set; }

        public Transaction(HttpListenerRequest req)
        {
            Console.WriteLine("Requete receive");

            IdExp = req.QueryString["idexp"];
            IdRcv = req.QueryString["idrcv"];
            Montant = req.QueryString["montant"];
        }

        public bool isNullOrEmpty()
        {
            bool isNullOrEmpty = false;
            string response = string.Empty;
            if (string.IsNullOrEmpty(IdExp) || string.IsNullOrEmpty(IdRcv) || string.IsNullOrEmpty(Montant))
            {
                isNullOrEmpty = true;
            }

            return isNullOrEmpty;
        }

        public static string getJson(Transaction transaction)
        {
            // exemple de réponse : {"IdExp":"a1","IdRcv":"b2","Montant":"9"}
            string jsonResult = "{\"IdExp\" \"" + transaction.IdExp + "\" ,\"IdRcv\":\"" + transaction.IdRcv + "\",\"Montant\":\"" + transaction.Montant + "\"}";
            return jsonResult;
        }
    }
}
