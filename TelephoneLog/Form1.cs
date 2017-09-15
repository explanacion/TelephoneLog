using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace TelephoneLog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default["logpath"].ToString();
            textBox2.Text = Properties.Settings.Default["filepattern"].ToString();
            int rwidth = (dataGridView1.Width - dataGridView1.Columns[0].Width) / 3;
            dataGridView1.Columns[1].Width = rwidth;
            dataGridView1.Columns[2].Width = rwidth;
            dataGridView1.Columns[3].Width = rwidth;
            //dataGridView1.Rows.Add(new string[] { "column2 value", "column6 value", "test" });


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                // resize controls
                dataGridView1.Size = new Size(Form1.ActiveForm.Width - 200, 0);
                int rwidth = (dataGridView1.Width - dataGridView1.Columns[0].Width) / 3;
                dataGridView1.Columns[1].Width = rwidth;
                dataGridView1.Columns[2].Width = rwidth;
                dataGridView1.Columns[3].Width = rwidth;
            }
            catch (Exception)
            { 
            }
        }

        private void ReadFromLogFile(string filename)
        {
            try
            {
                bool success = false;
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    string[] lines = sr.ReadToEnd().Split('\n');
                    
                    DateTime talkingtill = DateTime.Now;
                    DateTime talkingsince = DateTime.Now;
                    string cdate = monthCalendar1.SelectionRange.Start.ToString("dd-MM-yy");
                    foreach (string line in lines)
                    {
                        if (line.IndexOf("Line DialNum_New") != -1)
                        {
                            // outcome call
                            string since = numericUpDown1.Value.ToString("00") + ":" + numericUpDown2.Value.ToString("00") + ":" + numericUpDown3.Value.ToString("00");
                            DateTime dateTimeSince = DateTime.ParseExact(cdate + " " + since, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
                            string till = numericUpDown4.Value.ToString("00") + ":" + numericUpDown5.Value.ToString("00") + ":" + numericUpDown6.Value.ToString("00");
                            DateTime dateTimeTill = DateTime.ParseExact(cdate + " " + till, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);

                            string[] linearr = line.Split(new char[] { ' ' });
                            string curtime = linearr[0];
                            DateTime curdatetime = DateTime.ParseExact(cdate + " " + curtime, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);

                            // trying to get a duration
                            DateTime tempsince, temptill;
                            tempsince = talkingsince;
                            temptill = talkingtill;
                            string duration = "";
                            try
                            {
                                //Console.WriteLine(talkingsince.ToString("dd-MM-yy HH:mm:ss"));
                                TimeSpan durspan = (temptill - tempsince);
                                duration = durspan.ToString(@"hh\:mm\:ss");
                            }
                            catch (Exception)
                            {
                            }

                            if (curdatetime > dateTimeSince && curdatetime < dateTimeTill)
                            {
                                string substr = linearr.Last();
                                substr = substr.Substring(1, substr.Length - 3);
                                
                                if (tempsince.ToString("dd-MM-yy HH:mm:ss") != "")
                                    dataGridView1.Rows.Add(new string[] { curtime, "Исходящий звонок", substr, duration });
                            }
                        }
                        else if (line.IndexOf("Get CallerId=") != -1)
                        {
                            // income call
                            string since = numericUpDown1.Value.ToString("00") + ":" + numericUpDown2.Value.ToString("00") + ":" + numericUpDown3.Value.ToString("00");
                            DateTime dateTimeSince = DateTime.ParseExact(cdate + " " + since, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
                            string till = numericUpDown4.Value.ToString("00") + ":" + numericUpDown5.Value.ToString("00") + ":" + numericUpDown6.Value.ToString("00");
                            DateTime dateTimeTill = DateTime.ParseExact(cdate + " " + till, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
                            string[] linearr = line.Split(new char[] { ' ' });
                            string curtime = linearr[0];
                            DateTime curdatetime = DateTime.ParseExact(cdate + " " + curtime, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);

                            // trying to get a duration
                            DateTime tempsince, temptill;
                            tempsince = talkingsince;
                            temptill = talkingtill;
                            string duration = "";
                            try
                            {
                                //Console.WriteLine(talkingsince.ToString("dd-MM-yy HH:mm:ss"));
                                TimeSpan durspan = (temptill - tempsince);
                                duration = durspan.ToString(@"hh\:mm\:ss");
                            }
                            catch (Exception ex)
                            {
                            }

                            if (curdatetime > dateTimeSince && curdatetime < dateTimeTill)
                            {
                                string substr = linearr.Last();
                                substr = substr.Replace("CallerId=", "");
                                substr = substr.Substring(1, substr.Length - 2);
                                dataGridView1.Rows.Add(new string[] { curtime, "Входящий звонок", substr, duration });

                            }
                        }
                        else if (line.IndexOf("==14:Talking") != -1)
                        {
                            // start talking time
                            string[] linearr = line.Split(new char[] { ' ' });
                            string talksince = linearr[0];
                            talkingsince = DateTime.ParseExact(cdate + " " + talksince, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        else if (line.IndexOf("Fxs Hangup") != -1)
                        {
                            // stop talking time
                            string[] linearr = line.Split(new char[] { ' ' });
                            string talktill = linearr[0];
                            talkingtill = DateTime.ParseExact(cdate + " " + talktill, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        else if (line.IndexOf("=ACCEPT") != -1)
                        {
                            success = true;
                            int lrow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Displayed);
                            if (success)
                                dataGridView1.Rows[lrow - 1].Cells[0].Style.BackColor = Color.LightGreen;
                        }
                        else if (line.IndexOf("=CONNECTED") != -1)
                        {
                            success = true;
                            int lrow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Displayed);
                            if (success)
                                dataGridView1.Rows[lrow - 1].Cells[0].Style.BackColor = Color.LightGreen;
                        }
                        else if (line.IndexOf("=BUSY") != -1)
                        {
                            success = false;
                            int lrow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Displayed);
                            if (!success)
                                dataGridView1.Rows[lrow - 1].Cells[0].Style.BackColor = Color.LightPink;
                        }

                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Файл лога, соответствующий дате не найден");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Директория не найдена");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string dir = textBox1.Text;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Внимание. Директория " + dir + " не найдена.");
            }
            string[] filenames = textBox2.Text.Split(new char[] {'|'});
            //string sMonth = DateTime.Now.ToString("MM");
            //string sDay = DateTime.Now.ToString("dd");
            string sMonth = monthCalendar1.SelectionRange.Start.ToString("MM");
            string sDay = monthCalendar1.SelectionRange.Start.ToString("dd");
            for (int i = 0; i < filenames.Length; i++)
            {
                string filepath = dir + "\\" + filenames[i] + "." + sMonth + sDay;
                ReadFromLogFile(filepath);
            }
            // sort by time after reading all files
            dataGridView1.Sort(dataGridView1.Columns.GetFirstColumn(DataGridViewElementStates.Displayed), ListSortDirection.Ascending);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }
    }
}
