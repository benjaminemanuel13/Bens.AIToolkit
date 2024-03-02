using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class GPTEnhancements
    {
        public GPTEnhancement ocr { get; set; }

        public GPTEnhancement grounding { get; set; }

        public List<GPTDataSource> dataSource { get; set; }
    }
}
