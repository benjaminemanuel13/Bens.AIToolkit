using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bens.AI.Toolkit.Models.Mistral
{
    public class MistralUsage
    {
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
        public int completion_tokens { get; set; }
    }
}
