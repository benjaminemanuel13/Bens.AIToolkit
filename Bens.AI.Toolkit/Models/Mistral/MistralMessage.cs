using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bens.AI.Toolkit.Models.Mistral
{
    public class MistralMessage
    {
        public string role { get; set; } = "system";

        public string content { get; set; }
    }
}
