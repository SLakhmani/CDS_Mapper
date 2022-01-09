using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CDS_Mapper
{
    public partial class MappedCDS : Form
    {
        private PleaseWaitForm waitForm;
        private CDSMapperMainForm mainFormInstance;
        public DataSet gbk;
        public DataSet strand;

        public MappedCDS()
        {
            InitializeComponent();
        }
        
        public MappedCDS(DataSet strandDataSet, DataSet gbkDataSet, string strandType, CDSMapperMainForm MainForm)
        {
            InitializeComponent();
            titleLabel.Text = titleLabel.Text + strandType;
            mainFormInstance = MainForm;

            gbk = gbkDataSet.Copy();
            strand = strandDataSet.Copy();

            DataSet mapped;
            DataSet strands;
            DataColumn col;
            DataTable originalGBKTable;
            DataTable strandTable;

            originalGBKTable = gbkDataSet.Tables[0].Copy();
            strandTable = strandDataSet.Tables[0].Copy();

            // Add new columns
            col = new DataColumn();
            col.DataType = typeof(string);
            col.ColumnName = "Strand Present?";
            originalGBKTable.Columns.Add(col);

            col = new DataColumn();
            col.DataType = typeof(Int32);
            col.ColumnName = "Position(s)";
            originalGBKTable.Columns.Add(col);

            mapped = new DataSet();
            strands = new DataSet();

            // add table to dataset
            mapped.Tables.Add(originalGBKTable);
            strands.Tables.Add(strandTable);
            mappedCDSdgv.DataSource = mapped.Tables[0];
            strand_dgv.DataSource = strands.Tables[0];

            //Find Positions
            int originalLastColumn = 6;
            int columnCounter = 2;

            for (int i = 0; i < strand_dgv.Rows.Count; i++)
            {
                int currentPosition = Convert.ToInt32(strand_dgv[2, i].Value);

                for (int j = 0; j < mappedCDSdgv.Rows.Count - 1; j++)
                {
                    if (currentPosition > Convert.ToInt32(mappedCDSdgv[2,j].Value))
                    {
                        if (currentPosition < Convert.ToInt32(mappedCDSdgv[3, j].Value))
                        {
                            mappedCDSdgv[5, j].Value = "Yes";

                            if (mappedCDSdgv[6, j].Value.ToString() != "") // occupied
                            {
                                int lastColumnIndex = mappedCDSdgv.ColumnCount - 1; // get last column index
                                int unoccupiedIndex = 0;

                                if (mappedCDSdgv[lastColumnIndex, j].Value.ToString() != "")
                                {
                                    col = new DataColumn();
                                    col.DataType = typeof(Int32);
                                    col.ColumnName = "Position(s) " + columnCounter.ToString();
                                    originalGBKTable.Columns.Add(col);
                                    mappedCDSdgv[lastColumnIndex + 1, j].Value = currentPosition;
                                    columnCounter++;
                                    break;
                                }

                                else if(lastColumnIndex - originalLastColumn >= 1)
                                {
                                    for (int k = originalLastColumn + 1; k <= lastColumnIndex; k++)
                                    {
                                        if (mappedCDSdgv[k, j].Value.ToString() == "")
                                        {
                                            unoccupiedIndex = k;
                                            break;
                                        }
                                    }

                                    mappedCDSdgv[unoccupiedIndex, j].Value = currentPosition;
                                    break;
                                }
                            }
                            else // not occupied
                            {
                                mappedCDSdgv[6, j].Value = currentPosition;
                                break;
                            }                         
                        }                     
                    }
                    else
                    {
                        break;
                    }
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

        private void mappedCDSdgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ShowWaitForm("Generating Mapped Data. \nThis could take a while...");
        }

        private void saveMappedcds_Click(object sender, EventArgs e)
        {
            if(mappedCDSdgv.Rows.Count > 0)
            {
                saveFileDialog1.Filter = "CSV Files(*.csv)|*.csv|Text files (*.txt)|*.txt";
                saveFileDialog1.FileName = "Mapped Data";
                
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            SaveMappedDataCSV();
                            break;

                        case 2:
                            SaveMappedDataTXT();
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("There are no records to Export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveMappedDataCSV()
        {
            bool error = false;

            if (File.Exists(saveFileDialog1.FileName))
            {
                try
                {
                    File.Delete(saveFileDialog1.FileName);
                }
                catch (IOException ex)
                {
                    error = true;
                    MessageBox.Show("Failed to Write Data to disk." + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!error)
            {
                try
                {
                    mappedCDSdgv.AllowUserToAddRows = false;

                    int columnCount = mappedCDSdgv.Columns.Count;
                    string columnNames = "";
                    string[] outputCsv = new string[mappedCDSdgv.Rows.Count + 1];

                    for (int i = 0; i < columnCount; i++)
                    {
                        columnNames += mappedCDSdgv.Columns[i].HeaderText.ToString() + ",";
                    }
                    outputCsv[0] += columnNames;

                    for (int i = 1; (i - 1) < mappedCDSdgv.Rows.Count; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            outputCsv[i] += mappedCDSdgv.Rows[i - 1].Cells[j].Value.ToString() + ",";
                        }
                    }

                    File.WriteAllLines(saveFileDialog1.FileName, outputCsv, Encoding.UTF8);
                    MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    mappedCDSdgv.AllowUserToAddRows = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected Error :" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }      
        }

        private void SaveMappedDataTXT()
        {
            bool error = false;

            if (File.Exists(saveFileDialog1.FileName))
            {
                try
                {
                    File.Delete(saveFileDialog1.FileName);
                }
                catch (IOException ex)
                {
                    error = true;
                    MessageBox.Show("Failed to Write Data to disk." + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!error)
            {
                try
                {
                    mappedCDSdgv.AllowUserToAddRows = false;

                    int columnCount = mappedCDSdgv.Columns.Count;
                    string columnNames = "";
                    string[] outputTxt = new string[mappedCDSdgv.Rows.Count + 1];

                    for (int i = 0; i < columnCount; i++)
                    {
                        columnNames += mappedCDSdgv.Columns[i].HeaderText.ToString() + "\t";
                    }
                    outputTxt[0] += columnNames;

                    for (int i = 1; (i - 1) < mappedCDSdgv.Rows.Count; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            outputTxt[i] += mappedCDSdgv.Rows[i - 1].Cells[j].Value.ToString() + "\t";
                        }
                    }

                    File.WriteAllLines(saveFileDialog1.FileName, outputTxt, Encoding.UTF8);
                    MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    mappedCDSdgv.AllowUserToAddRows = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected Error :" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void calculateUpstreamButton_Click(object sender, EventArgs e)
        {
            string calcType = "Upstream";
            CalculateUpstreamDownstream(calcType);
        }

        private void calculateDownstreamButton_Click(object sender, EventArgs e)
        {
            string calcType = "Downstream";
            CalculateUpstreamDownstream(calcType);
        }

        private void CalculateUpstreamDownstream(string calcDirection)
        {
            bool validate = false;
            string baseRangeValue = "";

            using (promptForm promptDialog = new promptForm(calcDirection))
            {
                do
                {
                    if (promptDialog.ShowDialog() == DialogResult.OK)
                    {
                        baseRangeValue = promptDialog.TextBoxValue.ToString();

                        validate = ValidateInput(baseRangeValue);

                        if (!validate || Convert.ToInt32(baseRangeValue) == 0)
                        {
                            MessageBox.Show("Inavlid Value, Please Enter a Positive Integer Value greater than 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        validate = true;
                    }
                } while (!validate || Convert.ToInt32(baseRangeValue) == 0);
            }


            // check if cancel button was pressed
            bool isValueSelected = Int32.TryParse(baseRangeValue, out int streamRange);

            if (isValueSelected == true && streamRange > 0)
            {
                CDSUpstreamDownstream showStreamForm = new CDSUpstreamDownstream(gbk, strand, calcDirection, streamRange, this);
                showStreamForm.Show();
            }
        }

        public bool ValidateInput(string userInput)
        {
            foreach (char check in userInput)
            {
                if (check < '0' || check > '9')
                    return false;
            }
            return true;
        }
    }
        
}
