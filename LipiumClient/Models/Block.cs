using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LipiumClient.Models
{
    public class Block
    {
        /// <summary>
        /// Numéro du block exmple Block N°100
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; }

        /// <summary>
        /// Date et heure de création du block
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Hash du blcok précédent
        /// </summary>
        [JsonPropertyName("previousHash")]
        public string PreviousHash { get; set; }

        /// <summary>
        /// Hash du block en cours
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Chiffre unique permettant de généré le hash en cas de manque de transaction.
        /// Il permet d'éviter les attaques de rejeu
        /// </summary>
        [JsonPropertyName("nonce")]
        public int Nonce { get; set; }

        /// <summary>
        /// Liste des transactions du block
        /// </summary>
        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; }
    }
}
