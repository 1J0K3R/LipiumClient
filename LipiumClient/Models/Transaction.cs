using System;
using System.Net;
using System.Text.Json;

namespace LipiumClient.Models
{
    public class Transaction
    {
        public string IdExp { get; private set; }
        public string IdRcv { get; private set; }
        public decimal Montant { get; private set; }

        public Transaction(HttpListenerRequest req)
        {
            Console.WriteLine("Requete receive");

            IdExp = req.QueryString["idexp"];
            IdRcv = req.QueryString["idrcv"];
            decimal conteneur;
            decimal.TryParse(req.QueryString["montant"], out conteneur);
            Montant = conteneur;
        }

        public bool isNullEmptyOr0()
        {
            bool isNullEmptyOr0 = false;
            string response = string.Empty;
            if (string.IsNullOrEmpty(IdExp) || string.IsNullOrEmpty(IdRcv) || Montant == 0 )
            {
                isNullEmptyOr0 = true;
            }

            return isNullEmptyOr0;
        }

        public static string getJson(Transaction transaction)
        {
            // exemple de réponse : {"IdExp":"a1","IdRcv":"b2","Montant":"9"}
            string jsonResult = "{\"IdExp\" :\"" + transaction.IdExp + "\" ,\"IdRcv\":\"" + transaction.IdRcv + "\",\"Montant\":\"" + transaction.Montant + "\"}";
            return jsonResult;
        }
    }
}
