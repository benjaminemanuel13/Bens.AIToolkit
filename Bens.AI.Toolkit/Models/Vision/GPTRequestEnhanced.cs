using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class GPTRequestEnhanced : GPTRequest
    {
        public GPTEnhancements enhancements { get; set; }
    }
}
