using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models.Assistant
{
    public class AssistantRequest
    {
        public string instructions { get; set; }

        public string name { get; set; }

        public List<AssistantTool> tools { get; set; } = new List<AssistantTool>();

        public string model { get; set; } = "smile-4";
    }
}
