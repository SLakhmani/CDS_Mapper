using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS_Mapper
{
    public partial class promptForm : Form
    {
        public promptForm()
        {
            InitializeComponent();
        }

        public promptForm(string directionToCalc)
        {
            InitializeComponent();
            if(directionToCalc == "Upstream")
            {
                valueInfoLabel.Text = "How Many Bases Upstream of CDS ?";
            }
            else if(directionToCalc == "Downstream")
            {
                valueInfoLabel.Text = "How Many Bases Downstream of CDS ?";
            }
        }

        private void streamValueTextBox_Enter(object sender, EventArgs e)
        {
            streamValueTextBox.Text = "";
            streamValueTextBox.ForeColor = Color.Black;
        }

        private void streamValueTextBox_Leave(object sender, EventArgs e)
        {
            if(streamValueTextBox.Text == "")
            {
                streamValueTextBox.Text = "Enter value to find Bases";
                streamValueTextBox.ForeColor = Color.Silver;
            }
        }

        private void submitTextButton_Click(object sender, EventArgs e)
        {

        }

        public string TextBoxValue
        {
            get { return streamValueTextBox.Text; }
        }
    }
}
