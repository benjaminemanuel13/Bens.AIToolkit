using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureMessage
    {
        public string role { get; set; } = "user";

        public string content { get; set; }
    }
}
