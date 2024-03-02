using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class GPTRequest
    {
        public List<GPTMessage> messages { get; set; }

        public int max_tokens { get; set; } = 1000;

        public bool stream { get; set; } = false;
    }
}
