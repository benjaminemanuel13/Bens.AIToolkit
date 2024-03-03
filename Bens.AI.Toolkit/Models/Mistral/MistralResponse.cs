using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bens.AI.Toolkit.Models.Mistral
{
    public class MistralResponse
    {
        public List<MistralChoice> choices { get; set; }


        public MistralUsage usage { get; set; }
    }
}
