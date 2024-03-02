
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BensAIToolkit.Models
{
    public class AzureAnswer
    {
        public List<string> Answers { get; set; }

        public List<string> FilePaths { get; set; }

        public List<string> FileUrls { get; set; }

        public AzureResponse Response { get; set; }
    }
}
