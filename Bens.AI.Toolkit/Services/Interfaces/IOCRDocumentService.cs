using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bens.AI.Toolkit.Interfaces
{
    public interface IOCRDocumentService
    {
        Task<string> Recognise(string url, string filename, bool image = false);
    }
}
