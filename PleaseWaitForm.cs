using System.Windows.Forms;

namespace CDS_Mapper
{
    public partial class PleaseWaitForm : Form
    {
        public PleaseWaitForm()
        {
            InitializeComponent();
        }

        public void SetMessage(string message)
        {
            pleaseWaitLabel.Text = message;
        }
    }
}
