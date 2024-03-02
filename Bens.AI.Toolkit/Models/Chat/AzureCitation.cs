using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureCitation
    {
        public string content { get; set; }

        public int? id { get; set; }

        public string title { get; set; }

        public string filepath { get; set; }

        public string url { get; set; }

        public AzureMetadata metadata { get; set; }

        public string chunk_id { get; set; }
    }
}
