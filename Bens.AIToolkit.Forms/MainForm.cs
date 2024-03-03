using Bens.AI.Toolkit;
using Bens.AI.Toolkit.Models.Mistral;
using Newtonsoft.Json;

namespace Bens.AIToolkit.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            MistralClient.MistralUrl = "https://XXXX.XXXX";
            MistralClient.MistralCredentials = "XXX";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            MistralClient client = new MistralClient();

            var answer = await client.Ask("Who is ben emanuel?");

        }
    }
}
