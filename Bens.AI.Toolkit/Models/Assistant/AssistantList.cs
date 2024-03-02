using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models.Assistant
{
    public class AssistantList
    {
        public List<AssistantData> data { get; set; }

        public bool has_more { get; set; }
    }
}
