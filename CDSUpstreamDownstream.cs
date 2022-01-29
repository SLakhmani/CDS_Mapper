using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CDS_Mapper
{
    public partial class CDSUpstreamDownstream : Form
    {
        private MappedCDS previousForm;
        private PleaseWaitForm waitForm;
        private DataSet strandDataSet;
        private DataSet gbkDataSet;

        public CDSUpstreamDownstream(DataSet strandsDataSet, DataSet gbk)
        {
            InitializeComponent();

            strandDataSet = strandsDataSet.Copy();
            gbkDataSet = gbk.Copy();
        
        }

        public CDSUpstreamDownstream(DataSet gbkDataSet, DataSet strandDataSet, string streamDirection, int streamValue, MappedCDS CDSform)
        {
            InitializeComponent();

            previousForm = CDSform;

            titleLabel.Text = streamValue.ToString() + " Bases " + streamDirection;
            this.Text = " CDS Data (" + streamDirection + ")";
            this.Update();

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
            streamDGV.DataSource = mapped.Tables[0];
            strand_dgv.DataSource = strands.Tables[0];

            UpdateStartEndValues(streamDirection, streamValue);

            GetPositions(originalGBKTable);

            previousForm.Visible = false;
        }

        private void UpdateStartEndValues(string calcDirection, int streamRange)
        {
            streamDGV.AllowUserToAddRows = false;

            if (calcDirection == "Upstream")
            {
                // update Data in table
                for (int i = 0; i < streamDGV.Rows.Count; i++)
                {
                    if (streamDGV[1, i].Value.ToString() == "Complement") // upstream complement
                    {
                        int startVal = Convert.ToInt32(streamDGV[3, i].Value.ToString());
                        streamDGV[2, i].Value = startVal;
                        streamDGV[3, i].Value = startVal + streamRange;
                    }
                    else if (streamDGV[1, i].Value.ToString() == "Not Complement") // upstream not complement
                    {
                        int startVal = Convert.ToInt32(streamDGV[2, i].Value.ToString());

                        if (startVal - streamRange <= 0)
                        {
                            streamDGV[2, i].Value = 0;
                            streamDGV[3, i].Value = startVal;
                        }
                        else if (startVal - streamRange > 0)
                        {
                            streamDGV[2, i].Value = startVal - streamRange;
                            streamDGV[3, i].Value = startVal;
                        }
                    }
                }
            }
            else if (calcDirection == "Downstream")
            {
                for (int i = 0; i < streamDGV.Rows.Count; i++)
                {
                    if (streamDGV[1, i].Value.ToString() == "Complement") // downstream complement
                    {
                        int startVal = Convert.ToInt32(streamDGV[2, i].Value.ToString());

                        if (startVal - streamRange <= 0)
                        {
                            streamDGV[2, i].Value = 0;
                            streamDGV[3, i].Value = startVal;
                        }
                        else if (startVal - streamRange > 0)
                        {
                            streamDGV[2, i].Value = startVal - streamRange;
                            streamDGV[3, i].Value = startVal;
                        }
                    }
                    else if (streamDGV[1, i].Value.ToString() == "Not Complement") // downstream not complement
                    {
                        int startVal = Convert.ToInt32(streamDGV[3, i].Value.ToString());
                        streamDGV[2, i].Value = startVal;
                        streamDGV[3, i].Value = startVal + streamRange;
                    }
                }
            }

            streamDGV.AllowUserToAddRows = true;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Visible = true;
            previousForm.Show();                 
        }

        private void GetPositions(DataTable originalGBKTable)
        {
            //Find Positions
            DataColumn col;
            int originalLastColumn = 6;
            int columnCounter = 2;

            for (int i = 0; i < strand_dgv.Rows.Count; i++)
            {
                int currentPosition = Convert.ToInt32(strand_dgv[2, i].Value);

                for (int j = 0; j < streamDGV.Rows.Count - 1; j++)
                {
                    if (currentPosition > Convert.ToInt32(streamDGV[2, j].Value))
                    {
                        if (currentPosition < Convert.ToInt32(streamDGV[3, j].Value))
                        {
                            streamDGV[5, j].Value = "Yes";

                            if (streamDGV[6, j].Value.ToString() != "") // occupied
                            {
                                int lastColumnIndex = streamDGV.ColumnCount - 1; // get last column index
                                int unoccupiedIndex = 0;

                                if (streamDGV[lastColumnIndex, j].Value.ToString() != "")
                                {
                                    col = new DataColumn();
                                    col.DataType = typeof(Int32);
                                    col.ColumnName = "Position(s) " + columnCounter.ToString();
                                    originalGBKTable.Columns.Add(col);
                                    streamDGV[lastColumnIndex + 1, j].Value = currentPosition;
                                    columnCounter++;
                                    //break;
                                }

                                else if (lastColumnIndex - originalLastColumn >= 1)
                                {
                                    for (int k = originalLastColumn + 1; k <= lastColumnIndex; k++)
                                    {
                                        if (streamDGV[k, j].Value.ToString() == "")
                                        {
                                            unoccupiedIndex = k;
                                            break;
                                        }
                                    }

                                    streamDGV[unoccupiedIndex, j].Value = currentPosition;
                                    //break;
                                }
                            }
                            else // not occupied
                            {
                                streamDGV[6, j].Value = currentPosition;
                                //break;
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

        private void streamDGV_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if(titleLabel.Text.Contains("Bases"))
            {
                ShowWaitForm("Finding Positions " + titleLabel.Text + "\nThis could take a while...");
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

        private void saveMappedcds_Click(object sender, EventArgs e)
        {
            if (streamDGV.Rows.Count > 0)
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
                    streamDGV.AllowUserToAddRows = false;

                    int columnCount = streamDGV.Columns.Count;
                    string columnNames = "";
                    string[] outputCsv = new string[streamDGV.Rows.Count + 1];

                    for (int i = 0; i < columnCount; i++)
                    {
                        columnNames += streamDGV.Columns[i].HeaderText.ToString() + ",";
                    }
                    outputCsv[0] += columnNames;

                    for (int i = 1; (i - 1) < streamDGV.Rows.Count; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            outputCsv[i] += streamDGV.Rows[i - 1].Cells[j].Value.ToString() + ",";
                        }
                    }

                    File.WriteAllLines(saveFileDialog1.FileName, outputCsv, Encoding.UTF8);
                    MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    streamDGV.AllowUserToAddRows = true;
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
                    streamDGV.AllowUserToAddRows = false;

                    int columnCount = streamDGV.Columns.Count;
                    string columnNames = "";
                    string[] outputTxt = new string[streamDGV.Rows.Count + 1];

                    for (int i = 0; i < columnCount; i++)
                    {
                        columnNames += streamDGV.Columns[i].HeaderText.ToString() + "\t";
                    }
                    outputTxt[0] += columnNames;

                    for (int i = 1; (i - 1) < streamDGV.Rows.Count; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            outputTxt[i] += streamDGV.Rows[i - 1].Cells[j].Value.ToString() + "\t";
                        }
                    }

                    File.WriteAllLines(saveFileDialog1.FileName, outputTxt, Encoding.UTF8);
                    MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    streamDGV.AllowUserToAddRows = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected Error :" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
