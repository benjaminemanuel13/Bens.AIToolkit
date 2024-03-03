using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bens.AI.Toolkit.Models.Mistral
{
    public class MistralRequest
    {
        public List<MistralMessage> messages { get; set; }

        public int max_tokens { get; set; } = 500;

        public decimal temperature { get; set; } = 0.9M;

        public bool stream { get; set; } = false;
    }
}
