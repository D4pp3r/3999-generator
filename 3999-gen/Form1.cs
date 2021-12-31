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
using NAudio;

namespace _3999_gen
{


    public partial class Form1 : Form
    {

        private string filePath;

        private Dictionary<int, string> sections;
        private Dictionary<string, string> songData;
        private List<string> metaData;
        private List<string> tempoData;
        private List<string> eventsData;
        private List<string> expertChart;
        private List<string> hardChart;
        private List<string> mediumChart;
        private List<string> easyChart;

        private Dictionary<int, Note> chartDic;

        private int sectionATick;
        private int sectionBTick;

        public Form1()
        {
            InitializeComponent();
        }
        private static Dictionary<int, Note> ChartListToDictionary(List<string> chartData)
        {
            Dictionary<int, Note> output = new Dictionary<int, Note>();
            foreach (string line in chartData)
            {
                Note tempNote;
                string[] lineAr = line.Split('=');
                int timestamp = int.Parse(lineAr[0].Trim());
                string[] noteVals = lineAr[1].Trim().Split(' ');
                if (!output.ContainsKey(timestamp) && noteVals[0] == "N")
                {
                    int noteVal = int.Parse(noteVals[1]);
                    int susLen = int.Parse(noteVals[2]);
                    tempNote = new Note(noteVal, susLen);
                    output.Add(timestamp, tempNote);
                }
                else if (noteVals[0] == "N")
                {
                    int noteVal = int.Parse(noteVals[1]);
                    Note curNote = output[timestamp];
                    curNote.addVal(noteVal);
                    output[timestamp] = curNote;
                }

            }
            return output;
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
                    try
                    {
                        numNotes = int.Parse(txtBoxLoopCount.Text) * noteCount(section);
                    }
                    catch (Exception e1)
                    {
                        ErrorMessageAndClose(e1, "BAD NOTE COUNT DETECTED");
                    }
                }

                chartOutput = chartMultiply(section, numNotes);

                //writeChart();
            }



            else if (rdoBtnSong.Checked)
            {
                if (rdoBtnIteration.Checked)
                {
                    try
                    {
                        numNotes = int.Parse(txtBoxLoopCount.Text) * noteCount(expertChart);
                    }
                    catch (Exception e2)
                    {
                        ErrorMessageAndClose(e2, "BAD ITERATION COUNT DETECTED");
                    }
                }

                chartOutput = chartMultiply(expertChart, numNotes);

            }


        }

        private void ParseNotes(int[] timestamps, List<string> section)
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
                        continue;
                    case var curTime when curTime == timestamps[1]:
                        sectionCheck = false;
                        continue;
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

        }

        // reads in the chart data and separates it out appropriately
        private void readChart(string filename)
        {
            try
            {
                // grabs all lines from file
                string[] lines = File.ReadAllLines(filename);

                ParseChart(lines);

                chartDic = ChartListToDictionary(expertChart);

                // clean up raw data
                sections = getSections(eventsData);
                songData = getMetaData(metaData);

                InitComboBoxes();

                // sets name label to song title
                string songName = !songData.ContainsKey("Name") ? "Unknown Name" : songData["Name"];
                string songArtist = !songData.ContainsKey("Artist") ? "Unknown Artist" : songData["Artist"];
                string songCharter = !songData.ContainsKey("Charter") ? "Unknown Charter" : songData["Charter"];
                lblSong.Text = songName.Replace("\"", "") + " by " + songArtist.Replace("\"", "") + " (Charted by: " + songCharter.Replace("\"", "") + ")";

            }

            catch (Exception e)
            {
                // oopsie poopsie there was a fucky wucky
                ErrorMessageAndClose(e, "CHART PARSING ERROR");

            }
        }

        private KeyValuePair<int, Note> nearestNoteAfterSectionStart(Dictionary<int, Note> notes, int sectionStartTimestamp)
        {
            KeyValuePair<int, Note> curNote = new KeyValuePair<int, Note>();
            foreach (KeyValuePair<int, Note> note in notes)
            {
                curNote = note;
                if (note.Key < sectionStartTimestamp)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            return curNote;
        }

        private static void ErrorMessageAndClose(Exception e, string errorStr)
        {
            if (MessageBox.Show(e.Message + "\n\n" + e.ToString(), errorStr) == DialogResult.OK)
            {
                Application.Exit();
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
                    endLine = i - 1;
                    InitChartData(state, startLine, endLine, lines);
                    return;
            }
        }

        private void InitComboBoxes()
        {
            // add sections to the drop-down
            int i = 1;
            foreach (string section in sections.Values)
            {
                string curSection = i.ToString() + ": " + section;
                cmboBoxSection.Items.Add(curSection);
                cmboBoxSection2.Items.Add(curSection);
                i++;
            }
            cmboBoxSection.SelectedIndex = 0;
            cmboBoxSection2.SelectedIndex = 0;
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
            for (int i = 0; i < data.Count; i++)
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
            if(subs.Length == 5)
            {
                string[] tempAr = subs[4].Split('_');
                string[] newSubs = new string[4 + tempAr.Length];
                for(int i = 0; i < 4; i++)
                {
                    newSubs[i] = subs[i];
                }
                tempAr.CopyTo(newSubs, 4);
                subs = newSubs;
            }
            for (int x = 4; x < subs.Length; x++)
            {
                sectionTemp = sectionTemp + subs[x] + " ";
            }
            return sectionTemp;
        }

        // writes .chart as string, outputs string to new notes.chart, named 3999notes.chart
        // INPUT MUST BE MODIFIED EXPERTCHARTS
        private void writeChart(List<string> metaData, List<string> tempoData, List<string> eventsData, List<string> expertChart, List<string> hardChart, List<string> mediumChart, List<string> easyChart, string pathName)
        {
            string output = "";
            output += "[Song]\n{\n";
            foreach (string str in metaData)
            {
                output += str + '\n';
            }
            output += "}\n[SyncTrack]\n{\n";
            foreach (string str in tempoData)
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

            for (int i = 0; i < data.Count; i++)
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

            foreach (string line in chartData)
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

            Dictionary<int, string> orderedSections = null;
            try
            {
                orderedSections = sections.OrderBy(key => key.Key).ToDictionary(key => key.Key, key => key.Value);
            }
            catch (Exception e)
            {
                ErrorMessageAndClose(e, "CASTING ERROR");
            }

            SetStartEndTicks(startSection, endSection, ref startTick, ref endTick, orderedSections);

            if (endTick - startTick <= 0)
            {
                if (MessageBox.Show("naughty ch players go to the pokey") == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            sectionATick = startTick;
            sectionBTick = endTick;
            return new int[] { startTick, endTick };
        }

        private static void SetStartEndTicks(string startSection, string endSection, ref int startTick, ref int endTick, Dictionary<int, string> orderedSections)
        {
            foreach (KeyValuePair<int, string> section in orderedSections)
            {
                switch (section.Value)
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
            List<string> output = new List<string>();
            int curNumNotes = noteCount(chartData);

            return output;
        }

        // on file select, assign the path to filePath
        private void FileDialog_FileOk(object sender, CancelEventArgs e)
        {
            filePath = fileDialog.FileName;
        }

        private void InitFields()
        {
            filePath = null;

            sections = new Dictionary<int, string>();
            songData = new Dictionary<string, string>();

            metaData = new List<string>();
            tempoData = new List<string>();
            eventsData = new List<string>();
            expertChart = new List<string>();
            hardChart = new List<string>();
            mediumChart = new List<string>();
            easyChart = new List<string>();

            chartDic = null;

            sectionATick = -1;
            sectionBTick = -1;
        }

        // opens the file dialog and reads in chart data
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            InitFields();
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "CHART files (*.chart)|*.chart|All files (*.*)|*.*";
            DialogResult res = fileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                cmboBoxSection.Items.Clear();
                cmboBoxSection2.Items.Clear();
                readChart(filePath);
            }

        }

        private void cmboBoxSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chartDic is null && !(expertChart is null))
            {
                chartDic = ChartListToDictionary(expertChart);
            }
            KeyValuePair<int, Note> note = nearestNoteAfterSectionStart(chartDic, sectionATick);
            return;
        }
    }
    public class Note
    {
        private int noteVal;
        private int susVal;
        private int numNotes;
        private string noteDescriber;
        public Note()
        {
            noteVal = 0b0;
            numNotes = 0;
            noteDescriber = "";
        }
        public Note(int initVal, int initSusLen)
        {
            noteVal = (1 << (initVal));
            susVal = initSusLen;
            if (initVal < 5 || (initVal == 7)) // note/suscheck
            {
                numNotes++;
            }
            else
            {
                numNotes = 0b0;
            }
            addNoteDescriptor(initVal);
        }

        public void addVal(int newVal)
        {
            noteVal += (1 << newVal);
            if (newVal < 5 || newVal == 7) // note/suscheck
            {
                numNotes++;
            }
            addNoteDescriptor(newVal);
        }

        private void addNoteDescriptor(int newVal)
        {
            if (this.numNotes == 1 && this.susVal > 0)
            {
                noteDescriber = "Sustain with length " + susVal + " ";
            }
            noteDescriber += NumToSingleNote(newVal);

        }
        private string NumToSingleNote(int n)
        {
            switch (n)
            {
                case 0:
                    return "Green ";
                case 1:
                    return "Red ";
                case 2:
                    return "Yellow ";
                case 3:
                    return "Blue ";
                case 4:
                    return "Orange ";
                case 5:
                    return "Forced ";
                case 6:
                    return "Tap ";
                case 7:
                    // suscheck
                    return "Open ";
                default:
                    return String.Empty;
            }
        }
    }
}
