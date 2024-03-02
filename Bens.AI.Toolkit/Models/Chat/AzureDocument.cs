using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureDocument
    {
        public string id { get; set; }

        public string content { get; set; }

        public string title { get; set; }

        public string filepath { get; set; }

        public string url { get; set; }

        public string chunk_id { get; set; } = "1";
    }
}
