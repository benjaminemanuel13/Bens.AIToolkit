using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class GPTMessage
    {
        public string role { get; set; }

        public string contentText { get; set; }

        public List<GPTContentBase> content { get; set; }
    }
}
