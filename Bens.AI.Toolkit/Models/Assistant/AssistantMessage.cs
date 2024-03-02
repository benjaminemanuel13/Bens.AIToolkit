using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models.Assistant
{
    public class AssistantMessage
    {
        public string id { get; set; }

        public List<AssistantContent> content { get; set; }
    }
}
