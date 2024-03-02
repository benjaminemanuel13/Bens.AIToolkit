using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureDataSourceParameters
    {
        public string endpoint { get; set; } = string.Empty;

        public string key { get; set; } = string.Empty;

        public string indexName { get; set; } = string.Empty;
    }
}
