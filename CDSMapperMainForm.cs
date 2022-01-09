using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CDS_Mapper
{
    public partial class CDSMapperMainForm : Form
    {
        private PleaseWaitForm waitForm;
        private DataSet data_set_positve;
        private DataSet data_set_negative;
        private DataSet data_set_gbk;
        public CDSMapperMainForm()
        {
            InitializeComponent();
        }

        private void chooseFileButton1_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            var fileContent = string.Empty;

            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                var fileStream = openFileDialog1.OpenFile();
                string[] lines;

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd().Trim();
                    lines = fileContent.Split('\n');
                }

                DataTable table1 = new DataTable("table1");

                DataColumn col;
                DataRow row;

                // Column 1
                col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "A";
                table1.Columns.Add(col);

                // Column 2
                col = new DataColumn();
                col.DataType = typeof(Int32);
                col.ColumnName = "B";
                table1.Columns.Add(col);

                // Column 3
                col = new DataColumn();
                col.DataType = typeof(Int32);
                col.ColumnName = "C";
                table1.Columns.Add(col);

                // Column 4
                col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "D";
                table1.Columns.Add(col);

                // Column 5
                col = new DataColumn();
                col.DataType = typeof(Int32);
                col.ColumnName = "E";
                table1.Columns.Add(col);

                // Column 6
                col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "F";
                table1.Columns.Add(col);

                // create dataset
                data_set_positve = new DataSet();

                // add table to dataset
                data_set_positve.Tables.Add(table1);

                // Add rows
                dataGridView1.DataSource = data_set_positve.Tables["table1"]; // assign data source

                bool errorMsg = false;

                foreach (string line in lines)
                {
                    row = table1.NewRow();

                    string[] rowData = line.Split('\t'); // split tab delimited

                    for (int i = 0; i < rowData.Length; i++)
                    {
                        row[i] = rowData[i]; // row[columnindex] = row[rowvalues]
                    }

                    table1.Rows.Add(row);
                }

                if (table1.Rows[0][5].ToString().Contains('-'))
                {
                    errorMsg = true;
                }
                else
                {
                    errorMsg = false;
                }

                if (errorMsg)
                {
                    positiveLabel.Text = "+ve Strand File Status - ERROR";
                    positiveLabel.ForeColor = Color.Red;
                    MessageBox.Show("Some of the processed data belongs to the -ve Strand Motif File." +
                        "\n\nPlease ensure that the File is of the correct format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);              
                }
                else
                {
                    positiveLabel.Text = "+ve Strand File Status - OK";
                    positiveLabel.ForeColor = Color.Green;                  
                }
            }
        }

        private void chooseFileButton2_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            var fileContent = string.Empty;

            openFileDialog2.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 2;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog2.FileName;
                var fileStream = openFileDialog2.OpenFile();
                string[] lines;

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd().Trim();
                    lines = fileContent.Split('\n');
                }

                DataTable table2 = new DataTable("table2");

                DataColumn col;
                DataRow row;

                // Column 1
                col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "A";
                table2.Columns.Add(col);

                // Column 2
                col = new DataColumn();
                col.DataType = typeof(Int32);
                col.ColumnName = "B";
                table2.Columns.Add(col);

                // Column 3
                col = new DataColumn();
                col.DataType = typeof(Int32);
                col.ColumnName = "C";
                table2.Columns.Add(col);

                // Column 4
                col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "D";
                table2.Columns.Add(col);

                // Column 5
                col = new DataColumn();
                col.DataType = typeof(Int32);
                col.ColumnName = "E";
                table2.Columns.Add(col);

                // Column 6
                col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "F";
                table2.Columns.Add(col);

                // create dataset
                data_set_negative = new DataSet();

                // add table to dataset
                data_set_negative.Tables.Add(table2);

                // Add rows
                dataGridView2.DataSource = data_set_negative.Tables["table2"]; ; // assign data source

                bool errorMsg = false; 

                foreach (string line in lines)
                {
                    row = table2.NewRow();

                    string[] rowData = line.Split('\t'); // split tab delimited

                    for (int i = 0; i < rowData.Length; i++)
                    {
                        row[i] = rowData[i]; // row[columnindex] = row[rowvalues]
                    }

                    table2.Rows.Add(row);
                }

                if (table2.Rows[0][5].ToString().Contains('+'))
                {
                    errorMsg = true;
                }
                else
                {
                    errorMsg = false;
                }

                if (errorMsg)
                {
                    negativeLabel.Text = "-ve Strand File Status - ERROR";
                    negativeLabel.ForeColor = Color.Red;
                    MessageBox.Show("Some of the processed data belongs to the +ve Strand Motif File." +
                        "\n\nPlease ensure that the File is of the correct format.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                else
                {
                    negativeLabel.Text = "-ve Strand File Status - OK";
                    negativeLabel.ForeColor = Color.Green;
                }
            }
        }

        protected void ShowWaitForm(string message)
        {
            // don't display more than one wait form at a time
            if (waitForm != null && !waitForm.IsDisposed)
            {
                return;
            }

            waitForm = new PleaseWaitForm();
            waitForm.SetMessage(message); // "Loading data. Please wait..."
            waitForm.TopMost = true;
            waitForm.StartPosition = FormStartPosition.CenterScreen;
            waitForm.Show();
            waitForm.Refresh();

            // force the wait window to display for at least 700ms so it doesn't just flash on the screen
            System.Threading.Thread.Sleep(700);
            Application.Idle += OnLoaded;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            waitForm.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ShowWaitForm("Loading CDS Mapper...");
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ShowWaitForm("Processing data. Please wait...");
            positiveLabel.Visible = true;
            positivePlaceholder.Visible = false;
        }

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ShowWaitForm("Processing data. Please wait...");
            negativeLabel.Visible = true;
            negativePlaceholder.Visible = false;
        }

        private void chooseGBKButton_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            var fileContent = string.Empty;

            openFileDialog3.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog3.FilterIndex = 2;

            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog3.FileName;
                var fileStream = openFileDialog3.OpenFile();
                string[] lines;

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd().Trim();
                    lines = fileContent.Split('\n');
                }

                // index to chop file info

                try
                {
                    int lineCounterFeatures = 0;
                    int lineCounter = 0;
                    int lineCounterOrigin = 0;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("FEATURES"))
                        {
                            lineCounterFeatures = lineCounter;
                        }
                        if (line.StartsWith("ORIGIN"))
                        {
                            lineCounterOrigin = lineCounter;
                        }

                        lineCounter++;
                    }

                    // Header
                    string[] fileHeaderInformation = new string[lineCounterFeatures + 1];
                    StringBuilder sbHeader = new StringBuilder();

                    for (int i = 0; i < lineCounterFeatures; i++)
                    {
                        fileHeaderInformation[i] = lines[i];
                        sbHeader.Append(fileHeaderInformation[i]);
                        sbHeader.Append('\n');
                    }

                    richTextBox1.Text = sbHeader.ToString();

                    // Origin
                    int originLength = lines.Length - lineCounterOrigin;
                    string[] fileOriginInformation = new string[originLength];
                    StringBuilder sbOrigin = new StringBuilder();

                    for (int i = 0; i < originLength; i++)
                    {
                        fileOriginInformation[i] = lines[lineCounterOrigin];
                        sbOrigin.Append(fileOriginInformation[i]);
                        sbOrigin.Append('\n');

                        lineCounterOrigin++;
                    }

                    richTextBox2.Text = sbOrigin.ToString();

                    // CDS
                    // calculate source length
                    int sourceOffset = 8; // default

                    for (int i = lineCounterFeatures; i < lines.Length; i++ )
                    {
                        string currentLine = lines[i].Trim();

                        if (currentLine.StartsWith("gene"))
                        {
                            sourceOffset = i;
                            break;
                        }
                    }

                    // cds start
                    int cdsStart = sourceOffset + 1; // change as per needed dynamically later

                    DataTable table3 = new DataTable("table3");

                    DataColumn col;
                    DataRow row;

                    // Column 1
                    col = new DataColumn();
                    col.DataType = typeof(string);
                    col.ColumnName = "Type";
                    table3.Columns.Add(col);

                    // Column 2
                    col = new DataColumn();
                    col.DataType = typeof(string);
                    col.ColumnName = "Complement Status";
                    table3.Columns.Add(col);

                    // Column 3
                    col = new DataColumn();
                    col.DataType = typeof(Int32);
                    col.ColumnName = "Start";
                    table3.Columns.Add(col);

                    // Column 4
                    col = new DataColumn();
                    col.DataType = typeof(Int32);
                    col.ColumnName = "End";
                    table3.Columns.Add(col);

                    // Column 5
                    col = new DataColumn();
                    col.DataType = typeof(string);
                    col.ColumnName = "Gene/Locus Tag";
                    table3.Columns.Add(col);

                    // create dataset
                    data_set_gbk = new DataSet();

                    // add table to dataset
                    data_set_gbk.Tables.Add(table3);

                    // Add rows
                    dataGridView3.DataSource = data_set_gbk.Tables["table3"]; // assign data source

                    int cdsLength = lines.Length - originLength - cdsStart;
                    int typeIndex = 5; // type Index
                    string[] cdsArray = new string[cdsLength];

                    Array.Copy(lines, cdsStart, cdsArray, 0, cdsLength); // copying CDS data from lines

                    for (int i = 0; i < cdsArray.Length; i++)
                    {
                        string currentLine = cdsArray[i];
                        string typeValue = string.Empty; ;
                        int startValue = 0;
                        int endValue = 0;
                        string geneName = string.Empty;
                        string isComplement = string.Empty; ;

                        if (!currentLine[typeIndex].Equals(' '))
                        {
                            StringBuilder word = new StringBuilder();
                            int wordCounter = typeIndex;

                            for (int j = typeIndex; j < currentLine.Length; j++)
                            {
                                while (!currentLine[wordCounter].Equals(' '))
                                {
                                    word.Append(currentLine[wordCounter]);
                                    wordCounter++;
                                }

                                break;
                            }

                            // omitting misc_feature and misc_RNA
                            if (!Equals(word.ToString(), "gene") && !Equals(word.ToString(), "misc_feature") && !Equals(word.ToString(), "misc_RNA")
                                && !Equals(word.ToString(), "repeat_region") && !Equals(word.ToString(), "mobile_element"))
                            {
                                string positionString = string.Empty;
                                string positionStringUnformatted = string.Empty;
                                string nameValue = string.Empty;

                                // type
                                typeValue = word.ToString();
                                positionStringUnformatted = (currentLine.Substring(wordCounter, currentLine.Length - word.Length - typeIndex)).Trim();
                                positionStringUnformatted = positionStringUnformatted.Replace("<", "");
                                positionString = positionStringUnformatted.Replace(">", "");

                                if (!positionString.EndsWith(")") && positionString.Contains("complement"))
                                {
                                    nameValue = cdsArray[i + 2].Trim();
                                }
                                else
                                {
                                    nameValue = cdsArray[i + 1].Trim();
                                }
                                  
                                // formatting outputs
                                // name
                                string[] nameValueArray = nameValue.Split('"');
                                geneName = nameValueArray[1];

                                // start and end positions
                                if (positionString.Contains("complement"))
                                {
                                    isComplement = "Complement";

                                    if (positionString.Contains("complement") && positionString.Contains("join"))
                                    {
                                        //if positionString is split in 2 lines
                                        if(!positionString.EndsWith(")"))
                                        {
                                            string nextLine = cdsArray[i + 1].Trim();
                                            positionString = positionString.TrimEnd('\r') + nextLine;

                                            string replaceJoin = positionString.Replace("join", "");
                                            string[] positionArray = (replaceJoin.Replace("complement", "")).Split('(', ')', ',', '.');

                                            // find first and last values
                                            int valueCounter = 0;
                                            List<int> values = new List<int>();

                                            for (int j = 0; j < positionArray.Length; j++)
                                            {
                                                if (valueCounter < 2)
                                                {
                                                    if (positionArray[j] != "")
                                                    {
                                                        values.Add(Convert.ToInt32(positionArray[j]));
                                                        valueCounter += 1;
                                                        Array.Reverse(positionArray);
                                                        j = 0;
                                                    }
                                                }
                                            }

                                            startValue = values.Min();
                                            endValue = values.Max();
                                        }
                                        else
                                        {
                                            // if contains join and complement
                                            string replaceJoin = positionString.Replace("join", "");
                                            string[] positionArray = (replaceJoin.Replace("complement", "")).Split('(', ')', ',', '.');

                                            // find first and last values
                                            int valueCounter = 0;
                                            List<int> values = new List<int>();

                                            for (int j = 0; j < positionArray.Length; j++)
                                            {
                                                if (valueCounter < 2)
                                                {
                                                    if (positionArray[j] != "")
                                                    {
                                                        values.Add(Convert.ToInt32(positionArray[j]));
                                                        valueCounter += 1;
                                                        Array.Reverse(positionArray);
                                                        j = 0;
                                                    }
                                                }
                                            }

                                            startValue = values.Min();
                                            endValue = values.Max();
                                        }
                                    }
                                    else if (positionString.Contains("complement"))
                                    {
                                        // if contains only complement
                                        string[] positionArray = (positionString.Replace("complement", "")).Split('(', ')', ',', '.');

                                        startValue = Convert.ToInt32(positionArray[1]);
                                        endValue = Convert.ToInt32(positionArray[3]);
                                    }                                                                     
                                }
                                else
                                {
                                    isComplement = "Not Complement"; // not complement

                                    if (positionString.Contains("join"))
                                    {
                                        // if contains join
                                        string[] positionArray = (positionString.Replace("join", "")).Split('(', ')', ',', '.');

                                        startValue = Convert.ToInt32(positionArray[1]);
                                        endValue = Convert.ToInt32(positionArray[6]);
                                    }
                                    else
                                    {
                                        string[] positionArray = positionString.Split('.');

                                        startValue = Convert.ToInt32(positionArray[0]);
                                        endValue = Convert.ToInt32(positionArray[positionArray.Length - 1]);
                                    }                                  
                                }

                                // write item to row
                                row = table3.NewRow();
                                row[0] = typeValue;
                                row[1] = isComplement;
                                row[2] = startValue;
                                row[3] = endValue;
                                row[4] = geneName;

                                table3.Rows.Add(row);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    gbkLabel.ForeColor = Color.Red;
                    gbkLabel.Text = "GBK File Status - ERROR";
                    gbkLabel.Visible = true;
                    MessageBox.Show("Unexpected Error :" + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView3_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ShowWaitForm("Processing data. Please wait...");
            
            if (dataGridView3.Rows.Count > 0)
            {
                gbkPlaceholder.Visible = false;
                gbkLabel.Visible = true;
                gbkLabel.Text = "GBK File Status - OK";
                gbkLabel.ForeColor = Color.Green;
            }
            else
            {
                gbkLabel.Visible = true;
                gbkLabel.Text = "GBK File Status -ERROR";
                gbkLabel.ForeColor = Color.Red;
                MessageBox.Show("No Data was Found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void viewPositive_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0 || dataGridView3.Rows.Count == 0)
            {
                MessageBox.Show("Either the +ve Strand Motif File or GBK File was not processed correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (positiveLabel.Text == "+ve Strand File Status - ERROR")
            {
                MessageBox.Show("The +ve Strand Motif File was not processed Correctly.\n\nPlease check the File and reupload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CDSUpstreamDownstream backgroundCollection = new CDSUpstreamDownstream(data_set_positve, data_set_gbk);
                MappedCDS showCDSForm = new MappedCDS(data_set_positve, data_set_gbk, "(+ve Strands)", this);             
                showCDSForm.Show();
            }           
        }

        private void viewNegative_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0 || dataGridView3.Rows.Count == 0)
            {
                MessageBox.Show("Either the -ve Strand Motif File or GBK File was not processed correctly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (negativeLabel.Text == "-ve Strand File Status - ERROR")
            {
                MessageBox.Show("The -ve Strand Motif File was not processed Correctly.\n\nPlease check the File and reupload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CDSUpstreamDownstream backgroundCollection = new CDSUpstreamDownstream(data_set_negative, data_set_gbk);
                MappedCDS showCDSForm = new MappedCDS(data_set_negative, data_set_gbk,"(-ve Strands)", this);
                showCDSForm.Show();
            }
        }
    }
}
