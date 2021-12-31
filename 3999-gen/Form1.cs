using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _3999_gen
{
    public partial class Form1 : Form
    {

        private string filePath;

        private Dictionary<int, string> sections = new Dictionary<int, string>();
        private Dictionary<string, string> songData = new Dictionary<string, string>();

        private List<string> metaData = new List<string>();
        private List<string> tempoData = new List<string>();
        private List<string> eventsData = new List<string>();
        private List<string> expertChart = new List<string>();
        private List<string> hardChart = new List<string>();
        private List<string> mediumChart = new List<string>();
        private List<string> easyChart = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        // runs the chart generator function
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            List<string> chartOutput = new List<string>();
            int numNotes = rdoBtn3999.Checked ? 3999 : (rdoBtnCustom.Checked ? int.Parse(txtBoxNoteCount.Text) : -1);

            if (rdoBtnSection.Checked)
            {
                int[] timestamps = sectionGrabber(sections, cmboBoxSection.Text, cmboBoxSection2.Text);
                List<string> section = new List<string>();
                ParseNotes(timestamps, section);

                if (rdoBtnIteration.Checked)
                {
                    numNotes = int.Parse(txtBoxLoopCount.Text) * noteCount(section);
                }

                chartOutput = chartMultiply(section, numNotes);

                //writeChart();
            }

            else if(rdoBtnSong.Checked)
            {
                if (rdoBtnIteration.Checked)
                {
                    numNotes = int.Parse(txtBoxLoopCount.Text) * noteCount(expertChart);
                }

                chartOutput = chartMultiply(expertChart, numNotes);

            }


        }

        private bool ParseNotes(int[] timestamps, List<string> section)
        {
            bool sectionCheck = false;
            foreach (string line in expertChart)
            {
                string[] subs = line.Trim().Split('=');
                int timeStamp = int.Parse(subs[0].Trim());

                switch (timeStamp)
                {
                    case var curTime when curTime == timestamps[0]:
                        sectionCheck = true;
                        break;
                    case var curTime when curTime == timestamps[1]:
                        sectionCheck = false;
                        break;
                }

                /*if (timeStamp == timestamps[0])
                {
                    sectionCheck = true;
                }

                else if(timeStamp == timestamps[1])
                {
                    sectionCheck = false;
                }*/

                if (sectionCheck)
                {
                    section.Append(line);
                }
            }

            return sectionCheck;
        }

        // reads in the chart data and separates it out appropriately
        private void readChart(string filename)
        {
            try
            {
                // grabs all lines from file
                string[] lines = File.ReadAllLines(filename);

                ParseChart(lines);

                // clean up raw data
                sections = getSections(eventsData);
                songData = getMetaData(metaData);

                InitComboBoxes();

                // sets name label to song title
                lblSong.Text = songData["Name"];
            }

            catch (Exception e)
            {
                // oopsie poopsie there was a fucky wucky
                if(MessageBox.Show(e.Message + "\n\n" + e.ToString(), "CHART PARSING ERROR") == DialogResult.OK)
                {
                    Application.Exit();
                }

            }
        }

        private void ParseChart(string[] lines)
        {
            // variable declaration
            int state = -1;
            int startLine = 0;
            int endLine = 0;

            // looks for specific lines in the .chart file and set the sorting state accordingly
            for (int i = 0; i < lines.Length; i++)
            {
                HandleLines(lines, ref state, ref startLine, ref endLine, i);
            }
        }

        private void HandleLines(string[] lines, ref int state, ref int startLine, ref int endLine, int i)
        {
            
            switch (lines[i])
            {
                case "[Song]":
                    state = 0;
                    return;

                case "[SyncTrack]":
                    state = 1;
                    return;

                case "[Events]":
                    state = 2;
                    return;

                case "[ExpertSingle]":
                    state = 3;
                    return;

                case "[HardSingle]":
                    state = 4;
                    return;

                case "[MediumSingle]":
                    state = 5;
                    return;

                case "[EasySingle]":
                    state = 6;
                    return;
                case "{":
                    startLine = i + 1;
                    return;
                case "}":
                    InitChartData(state, startLine, i-1, lines);
                    return;
            }
        }

        private void InitComboBoxes()
        {
            // add sections to the drop-down
            foreach (string section in sections.Values)
            {
                cmboBoxSection.Items.Add(section);
                cmboBoxSection2.Items.Add(section);
            }
        }

        private void InitChartData(int state, int startLine, int endLine, string[] lines)
        {
            switch (state)
            {
                case -1:
                    return;

                case 0:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        metaData.Add(lines[startLine + x]);
                    }
                    return;

                case 1:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        tempoData.Add(lines[startLine + x]);
                    }
                    return;

                case 2:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        eventsData.Add(lines[startLine + x]);
                    }
                    return;

                case 3:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        expertChart.Add(lines[startLine + x]);
                    }
                    return;

                case 4:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        hardChart.Add(lines[startLine + x]);
                    }
                    return;

                case 5:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        mediumChart.Add(lines[startLine + x]);
                    }
                    return;

                case 6:
                    for (int x = 0; x <= endLine - startLine; x++)
                    {
                        easyChart.Add(lines[startLine + x]);
                    }
                    return;
            }
        }

        private Dictionary<int, string> getSections(List<string> data)
        {
            Dictionary<int, string> output = new Dictionary<int, string>();
            for(int i = 0; i<data.Count; i++)
            {
                string[] subs = data[i].Trim().Split(' ');
                AddSections(output, subs);
            }
            return output;
        }

        private static void AddSections(Dictionary<int, string> output, string[] subs)
        {
            string sectionTemp = "";
            if (subs[3].Contains("section"))
            {
                sectionTemp = concatSectionTemp(subs, sectionTemp);
                sectionTemp = sectionTemp.Split('\"')[0];
                output.Add(int.Parse(subs[0]), sectionTemp);
            }
        }

        private static string concatSectionTemp(string[] subs, string sectionTemp)
        {
            for (int x = 4; x < subs.Length; x++)
            {
                sectionTemp  = sectionTemp + subs[x]+" ";
            }

            return sectionTemp;
        }

        // writes .chart as string, outputs string to new notes.chart, named 3999notes.chart
        // INPUT MUST BE MODIFIED EXPERTCHARTS
        private void writeChart(List<string> metaData, List<string> tempoData, List<string> eventsData, List<string> expertChart, List<string> hardChart, List<string> mediumChart, List<string> easyChart, string pathName)
        {
            string output = "";
            output += "[Song]\n{\n";
            foreach(string str in metaData)
            {
                output += str + '\n';
            }
            output += "}\n[SyncTrack]\n{\n";
            foreach(string str in tempoData)
            {
                output += str + '\n';
            }
            output += "}\n[Events]\n{\n";
            foreach (string str in eventsData)
            {
                output += str + '\n';
            }
            output += "}\n[ExpertSingle]\n{\n";
            foreach (string str in expertChart)
            {
                output += str + '\n';
            }
            output += "}\n[HardSingle]\n{\n";
            foreach (string str in hardChart)
            {
                output += str + '\n';
            }
            output += "}\n[MediumSingle]\n{\n";
            foreach (string str in mediumChart)
            {
                output += str + '\n';
            }
            output += "}\n[EasySingle]\n{\n";
            foreach (string str in easyChart)
            {
                output += str + '\n';
            }
            output += "}\n";

            File.WriteAllText(pathName + "3999notes.chart", output);
        }

        // sorts through and constructs a dictionary of the song meta data
        private Dictionary<string, string> getMetaData(List<string> data)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            
            for(int i = 0; i<data.Count; i++)
            {
                string[] subs = data[i].Trim().Split('=');
                output.Add(subs[0].Trim(), subs[1].Trim());
            }

            return output;
        }

        // counts how many notes in chart raw data
        private int noteCount(List<string> chartData)
        {
            int output = 0;
            string[] noteLines = new string[] { };

            foreach(string line in chartData)
            {
                if (line.Contains("N"))
                {
                    string[] subs = line.Trim().Split('=');
                    if (!noteLines.Contains(subs[0].Trim()))
                    {
                        noteLines.Append(subs[0].Trim());
                    }
                }
            }

            output = noteLines.Length;

            return output;
        }

        private int[] sectionGrabber(Dictionary<int, string> sections, string startSection, string endSection)
        {
            int startTick = 0;
            int endTick = 0;

            Dictionary<int, string> orderedSections = (Dictionary<int, string>)sections.OrderBy(key => key.Key);

            SetStartEndTicks(startSection, endSection, ref startTick, ref endTick, orderedSections);

            if (endTick - startTick <= 1)
            {
                MessageBox.Show("naughty ch players go to the pokey");
                Application.Exit();
            }

            return new int[] { startTick, endTick };
        }

        private static void SetStartEndTicks(string startSection, string endSection, ref int startTick, ref int endTick, Dictionary<int, string> orderedSections)
        {
            foreach (KeyValuePair<int, string> section in orderedSections)
            {
                switch(section.Value)
                {
                    case var val when val == startSection:
                        startTick = section.Key;
                        continue;
                    case var val when val == endSection:
                        endTick = section.Key;
                        continue;
                }
            }
        }

        private List<string> chartMultiply(List<string> chartData, int numNotes)
        {
            return null;
        }

        // on file select, assign the path to filePath
        private void FileDialog_FileOk(object sender, CancelEventArgs e)
        {
            filePath = fileDialog.FileName;
        }

        // opens the file dialog and reads in chart data
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "CHART files (*.chart)|*.chart|All files (*.*)|*.*";
            fileDialog.ShowDialog();

            cmboBoxSection.Items.Clear();
            cmboBoxSection2.Items.Clear();

            readChart(filePath);
        }
    }
}
