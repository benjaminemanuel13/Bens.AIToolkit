using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureDataSource
    {
        public string type { get; set; } = "AzureCognitiveSearch";

        public AzureDataSourceParameters parameters { get; set; } = new AzureDataSourceParameters();

        public string queryType { get; set; } = "semantic";

        public string semanticConfiguration { get; set; } = "default";
    }
}
