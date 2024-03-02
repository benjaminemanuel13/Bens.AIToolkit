using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureChoice
    {
        public int index { get; set; }

        public List<AzureResponseMessage> messages { get; set; }
    }
}
