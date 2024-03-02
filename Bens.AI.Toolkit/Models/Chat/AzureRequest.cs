using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureRequest<T>
    {
        public double top_p { get; set; }

        public int temperature { get; set; } = 0;

        public int max_tokens { get; set; } = 800;

        public List<AzureDataSource> dataSources { get; set; }

        public List<AzureMessage> messages { get; set; }

        //public List<AzureFunction<T>> functions { get; set; } = new List<AzureFunction<T>>();

        public string function_call { get; set; } = "auto";
    }

    public class AzureRequest
    {
        public double top_p { get; set; }

        public int temperature { get; set; } = 0;

        public int max_tokens { get; set; } = 800;


        public List<AzureDataSource> dataSources { get; set; }

        public List<AzureMessage> messages { get; set; }

        //public string function_call { get; set; } = "auto";
    }
}
