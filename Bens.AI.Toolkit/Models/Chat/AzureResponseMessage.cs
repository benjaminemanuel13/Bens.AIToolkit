
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureResponseMessage
    {
        public int index { get; set; }

        public string role { get; set; }

        public string content { get; set; }

        public bool end_turn { get; set; }

        public AzureFunctionCall function_call { get; set; }
    }
}
