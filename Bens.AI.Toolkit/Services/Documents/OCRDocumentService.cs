
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Bens.AI.Toolkit.Interfaces;

namespace Bens.AI.Toolkit
{
    internal class OCRDocumentService : IOCRDocumentService
    {
        public static string CognitiveKey { get; set; } = string.Empty;
        public static string CognitiveEndpoint { get; set; } = string.Empty;

        public async Task<string> Recognise(string url, string filename, bool image = false)
        {
            AzureKeyCredential credential = new AzureKeyCredential(CognitiveKey);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(CognitiveEndpoint), credential);

            Uri fileUri = new Uri(url + filename);

            string type = "prebuilt-document";

            if (image)
            {
                type = "prebuilt-read";
            }

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, type, fileUri);
            AnalyzeResult result = operation.Value;

            StringBuilder sb = new StringBuilder();

            foreach (DocumentPage page in result.Pages)
            {
                for (int i = 0; i < page.Lines.Count; i++)
                {
                    DocumentLine line = page.Lines[i];
                    sb.AppendLine(line.Content);
                }
            }

            for (int i = 0; i < result.Tables.Count; i++)
            {
                DocumentTable table = result.Tables[i];
                Console.WriteLine($"  Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");

                foreach (DocumentTableCell cell in table.Cells)
                {
                    Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) has kind '{cell.Kind}' and content: '{cell.Content}'.");
                }
            }

            return sb.ToString();
        }
    }
}
