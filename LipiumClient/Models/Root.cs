using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LipiumClient.Models
{
    public class Root
    {
        /// <summary>
        /// Liste des transactions du block
        /// </summary>
        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; }
    }
}
