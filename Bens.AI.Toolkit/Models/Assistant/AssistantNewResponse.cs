using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models.Assistant
{
    public class AssistantNewResponse
    {
        public string id { get; set; }

        public string _object { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string instructions { get; set; }

        public List<AssistantTool> tools { get; set; } = new List<AssistantTool>();
    }
}
