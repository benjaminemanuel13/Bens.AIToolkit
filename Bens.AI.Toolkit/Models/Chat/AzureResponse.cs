using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureResponse
    {
        public string id { get; set; }

        public string model { get; set; }

        public int created { get; set; }

        public string Object { get; set; }

        public List<AzureChoice> choices { get; set; }
    }
}
