using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LipiumClient.Models
{
    public class Transaction
    {
        /// <summary>
        /// Identifiant du compte Expediteur
        /// </summary>
        public string IdExp { get; private set; }
        
        /// <summary>
        /// Identifiant du compte Receveur
        /// </summary>
        public string IdRcv { get; private set; }
        
        /// <summary>
        /// Montant de la transaction
        /// </summary>
        public decimal Montant { get; private set; }

        /// <summary>
        /// Constructeur de la transaction prenant en paramètre la requete qui contient les paramètres
        /// </summary>
        /// <param name="req"></param>
        public Transaction(HttpListenerRequest req)
        {
            Console.WriteLine("Requete receive");

            IdExp = req.QueryString["idexp"];
            IdRcv = req.QueryString["idrcv"];
            decimal conteneur;
            decimal.TryParse(req.QueryString["montant"], out conteneur);
            Montant = conteneur;
        }

        /// <summary>
        /// Méthode que le contenu des propriétés de l'objet ne soit pas null vide ou à 0 (pour un double)
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Transforme notre objet en Json.
        /// Manuellement pour garder un ordre dans les propriétés
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static string getJson(Transaction transaction)
        {
            // exemple de réponse : {"IdExp":"a1","IdRcv":"b2","Montant":"9"}
            string jsonResult = "{\"IdExp\" :\"" + transaction.IdExp + "\" ,\"IdRcv\":\"" + transaction.IdRcv + "\",\"Montant\":\"" + transaction.Montant + "\"}";
            return jsonResult;
        }

        /// <summary>
        /// Calcul le hash à partir d'une chaine
        /// </summary>
        /// <param name="stringToHash"></param>
        /// <returns></returns>
        public static string getHash(string stringToHash)
        {
            Console.WriteLine(stringToHash);
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
