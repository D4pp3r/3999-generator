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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NVorbis;
using NAudio.Vorbis;
using System.Text.RegularExpressions;

namespace _3999_gen
{


    public partial class Form1 : Form
    {

        private string filePath;
        private Chart chart;

        private int tickA;
        private int tickB;

        private LimitedQueue<char> funnycode = new LimitedQueue<char>(8);

        public Form1()
        {
            InitializeComponent();
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        // on file select, assign the path to filePath
        private void FileDialog_FileOk(object sender, CancelEventArgs e)
        {
            filePath = fileDialog.FileName;
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
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

        private void readChart(string filePath)
        {
            chart = new Chart(filePath);
            lblSong.Text = $"{chart.chartName.Replace("&", "&&")} by {chart.chartArtist.Replace("&","&&")} (Charted by: {chart.charter.Replace("&", "&&")})";
            InitComboBoxes();
        }

        private void InitComboBoxes()
        {
            Graphics g = this.CreateGraphics();
            Rectangle rectangle = new Rectangle();
            PaintEventArgs e = new PaintEventArgs(g, rectangle);

            // add sections to the drop-down
            int i = 1;
            float maxWidth = 0f;
            Font font = cmboBoxSection.Font;
            SizeF stringSize = new SizeF();
            List<string> sectionLists = new List<string>();
            foreach (GlobalEvent globalEvent in chart.EventsData)
            {
                if (globalEvent.eventType == "section")
                {

                    SectionEvent sectionEvent = globalEvent as SectionEvent;
                    sectionLists.Add(sectionEvent.sectionName.Replace('_', ' ').Trim());
                }
            }
            if (sectionLists.Count == 0) return;
            foreach (string section in sectionLists)
            {
                string curSection = StripHTML($"{i}: {section}");
                stringSize = e.Graphics.MeasureString(curSection, font);
                if (stringSize.Width > maxWidth)
                {
                    float newWidth = stringSize.Width + 10;
                    cmboBoxSection.DropDownWidth = Convert.ToInt32(newWidth);
                    cmboBoxSection2.DropDownWidth = Convert.ToInt32(newWidth);
                    maxWidth = stringSize.Width;
                }
                cmboBoxSection.Items.Add(curSection);
                cmboBoxSection2.Items.Add(curSection);
                i++;
            }
            cmboBoxSection.SelectedIndex = 0;
            cmboBoxSection2.SelectedIndex = 0;
        }

        private void cmboBoxSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chart is null) return;
            string curSection = Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", "");
            int index = cmboBoxSection.SelectedIndex;
            foreach (GlobalEvent g in chart.EventsData)
            {
                if (g.eventType == "section")
                {
                    SectionEvent section = g as SectionEvent;
                    if (index == section.sectionIndex)
                    {
                        tickA = section.timestamp;
                    }
                }
            }
        }

        private void cmboBoxSection2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chart is null) return;
            string curSection = Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", "");

            bool tickFlag = false;

            int index = cmboBoxSection2.SelectedIndex + 1;
            if (index == cmboBoxSection2.Items.Count)
            {
                tickB = chart.lastTimeStamp;
            }

            foreach (GlobalEvent globalEvent in chart.EventsData)
            {

                if (globalEvent.eventType == "section")
                {
                    SectionEvent section = globalEvent as SectionEvent;
                    if (index == section.sectionIndex)
                    {
                        tickFlag = true;
                    }
                    if (tickFlag)
                    {
                        tickB = section.timestamp - 1;
                        break;
                    }
                    

                }
            }

        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            funnycode.Enqueue(e.KeyChar);
            string curstring = new string(funnycode.ToArray());
            if (curstring == "oboyoboy")
            {
                if (MessageBox.Show("Hyperspeed Unlocked!") == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            else if(curstring == "borm tim")
            {
                if (MessageBox.Show("Get your BormoTime Guitar from bormotime.com today!") == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (chart is null) return;
            if (tickA == -1 || tickB == -1) return;
            ChartGenerator gen = new ChartGenerator(chart);

            int numNotes = 3999;

            if(rdoBtnCustom.Checked)
            {
                if(!Int32.TryParse(txtBoxNoteCount.Text, out numNotes) || numNotes <= 0)
                {
                    MessageBox.Show("Not a valid note count, to the pokey with you.");
                    return;
                }
            }
            else if(rdoBtnIteration.Checked)
            {
                int numIterations = 0;
                if (!Int32.TryParse(txtBoxLoopCount.Text, out numIterations) || numIterations <= 0)
                {
                    MessageBox.Show("Not a valid note count, to the pokey with you.");
                    return;
                }
                numNotes = numIterations * chart.NumNotesDuration(tickA, tickB, chart.ExpertData);
            }

            gen.Generate(numNotes, tickA, tickB, Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", ""), Regex.Replace(cmboBoxSection2.Text, "[0-9]+:[ ]", ""), chart.pathName + "\\..");

            /*if(chart.MetaData["MusicStream"] is null)
            {

            }

            else
            {
                float[] seconds = TimestampToSeconds(chart, tickA, tickB);
                if (seconds != new float[] { -1, -1 })
                {
                    if(chart.MetaData["MusicStream"].Contains(".wav"))
                    {

                    }

                    else
                    {
                        AudioUtils converter = new AudioUtils();
                        WavFileUtils trimmer = new WavFileUtils();
                        converter.ConvertToWav($"{chart.pathName}\\..\\{chart.MetaData["MusicStream"]}", $"{chart.pathName}\\..\\temp.wav");
                        trimmer.TrimWavFile($"{chart.pathName}\\..\\temp.wav", $"{chart.pathName}\\..\\temp2.wav", seconds[0], seconds[1]);
                        trimmer.MultiplyWav($"{chart.pathName}\\..\\temp2.wav", $"{chart.pathName}\\..\\3999-audio.wav", gen.iterations);
                    }
                }
            }*/
        }

        private float[] TimestampToSeconds(Chart chart, int startTimestamp, int endTimestamp)
        {
            int resolution = chart.ticksPerQuarterNote;
            int bpm = 0;
            int tickStart = 0;
            int tickEnd = 0;
            bool shit = false;

            foreach(SyncEvent sync in chart.SyncData)
            {
                if (sync.eventType == "B")
                {
                    BPMEvent bpmevent = sync as BPMEvent;

                    if(startTimestamp < bpmevent.timestamp && endTimestamp > bpmevent.timestamp)
                    {
                        shit = true;
                        return new float[] { -1 , -1 };
                    }

                    else if(!shit && bpmevent.timestamp <= startTimestamp)
                    {
                        bpm = bpmevent.BPM/1000;
                        tickStart = startTimestamp;
                        tickEnd = endTimestamp;
                    }
                }
            }

            float[] output = new float[] { tickStart / resolution * 60 / bpm, tickEnd / resolution * 60 / bpm };

            return output;
        }
    }

    public class TimestampedEvent
    {
        public int timestamp { get; private set; }
        public string SyncGlobalOrChartEvent { get; private set; }

        public string RawData { get; private set; }

        public TimestampedEvent(int newTimestamp, string eventType, string newData)
        {
            this.timestamp = newTimestamp;
            this.SyncGlobalOrChartEvent = eventType;
            this.RawData = newData;
        }
    }

    public class SyncEvent : TimestampedEvent
    {
        public string eventType { get; private set; }

        public SyncEvent(int newTimestamp, string newEventType, string newData) : base(newTimestamp, "Sync", newData)
        {
            this.eventType = newEventType;
        }
    }

    public class BPMEvent : SyncEvent
    {
        public int BPM { get; private set; }
        public BPMEvent(int newTimestamp, int newBPM, string newData) : base(newTimestamp, "B", newData)
        {
            this.BPM = newBPM;
        }
    }

    public class TSEvent : SyncEvent
    {
        public int TSNum { get; private set; }
        public int TSDenom { get; private set; } // STORED AS LOG_2 OF THE ACTUAL TIME SIG DENOMINATOR
        public TSEvent(int newTimestamp, int newTSNum, int newTSDenom, string newData) : base(newTimestamp, "TS", newData)
        {
            this.TSNum = newTSNum;
            this.TSDenom = newTSDenom;
        }

        // No timestamp provided assumes denominator is 4 (2^2)
        public TSEvent(int newTimestamp, int newTSNum, string newData) : this(newTimestamp, newTSNum, 2, newData) { }

    }

    public class GlobalEvent : TimestampedEvent
    {
        public string eventType { get; private set; }

        public GlobalEvent(int newTimestamp, string newEventType, string newData) : base(newTimestamp, "Global", newData)
        {
            this.eventType = newEventType;
        }
    }

    public class TextEvent : GlobalEvent
    {
        public string text { get; private set; }
        public TextEvent(int newTimestamp, string newText, string newData) : base(newTimestamp, "text", newData)
        {
            this.text = newText;
        }
    }

    public class SectionEvent : GlobalEvent
    {
        public string sectionName { get; private set; }
        public int sectionIndex { get; private set; }
        public SectionEvent(int newTimestamp, string newSectionName, int newSectionIndex, string newData) : base(newTimestamp, "section", newData)
        {
            this.sectionName = newSectionName;
            this.sectionIndex = newSectionIndex;
        }
    }

    public class LyricEvent : GlobalEvent
    {
        public string lyric { get; private set; }
        public LyricEvent(int newTimestamp, string newLyric, string newData) : base(newTimestamp, "lyric", newData)
        {
            this.lyric = newLyric;
        }
    }

    public class ChartEvent : TimestampedEvent
    {
        public string eventType { get; private set; }

        public ChartEvent(int newTimestamp, string newEventType, string newData) : base(newTimestamp, "Chart", newData)
        {
            this.eventType = newEventType;
        }
    }

    public enum NoteEnum
    {
        Green = 0,
        Red = 1,
        Yellow = 2,
        Blue = 3,
        Orange = 4,
        Forced = 5,
        Tap = 6,
        Open = 7
    }

    public class NoteEvent : ChartEvent
    {
        public int NoteValue { get; private set; }
        public int SustainLength { get; private set; }

        public NoteEnum NoteType { get; private set; }

        public int NoteIndex { get; private set; }

        public NoteEvent(int timestamp, int newNoteValue, int newSustainLength, int noteIndex, string newData) : base(timestamp, "N", newData)
        {
            this.NoteValue = newNoteValue;
            this.SustainLength = newSustainLength;
            this.NoteType = (NoteEnum)this.NoteValue;
            this.NoteIndex = noteIndex;
        }

    }

    public class SpecialEvent : ChartEvent
    {
        public int SpecialValue { get; private set; }
        public int EventLength { get; private set; }

        public SpecialEvent(int timestamp, int newSpecialValue, int newEventLength, string newData) : base(timestamp, "S", newData)
        {
            this.SpecialValue = newSpecialValue;
            this.EventLength = newEventLength;
        }
    }

    public class TrackEvent : ChartEvent
    {
        public string EventName { get; private set; }

        public TrackEvent(int timestamp, string newEventName, string newData) : base(timestamp, "E", newData)
        {
            this.EventName = newEventName;
        }
    }

    public class Chart
    {
        public int numNotes { get; private set; }

        public int numSections { get; private set; }
        public int numSyncEvents { get; private set; }
        public int ticksPerQuarterNote { get; private set; }

        public int lastTimeStamp { get; private set; }

        private string[] lines;

        public string chartName { get; private set; }
        public string chartArtist { get; private set; }
        public string charter { get; private set; }

        public string pathName { get; private set; }

        public Dictionary<string, string> MetaData { get; private set; }
        public List<SyncEvent> SyncData { get; private set; }

        public List<GlobalEvent> EventsData { get; private set; }
        public List<ChartEvent> ExpertData { get; private set; }

        private static void ErrorMessageAndClose(Exception e, string errorStr)
        {
            if (MessageBox.Show($"{e.Message}\n\n{e}", errorStr) == DialogResult.OK)
            {
                Application.Exit();
            }
        }



        public Chart(string filename)
        {
            lastTimeStamp = -1;
            numSections = 0;
            this.pathName = filename;
            try
            {
                this.lines = File.ReadAllLines(filename);
                this.MetaData = new Dictionary<string, string>();
                this.SyncData = new List<SyncEvent>();
                this.EventsData = new List<GlobalEvent>();
                this.ExpertData = new List<ChartEvent>();

                string[] songini = File.ReadAllLines(filename.Split(new[] { "notes.chart" }, StringSplitOptions.None)[0] + "song.ini");
                chartName = null; chartArtist = null; charter = null;
                string frets = null;
                bool hasSongName, hasSongArtist, hasSongCharter, hasFrets;
                foreach (string entry in songini)
                {
                    string newEntry = Form1.StripHTML(entry);
                    hasSongName = newEntry.Trim().ToLower().StartsWith("name");
                    hasSongArtist = newEntry.Trim().ToLower().StartsWith("artist");
                    hasSongCharter = newEntry.Trim().ToLower().StartsWith("charter");
                    hasFrets = newEntry.Trim().ToLower().StartsWith("frets");
                    if (hasSongName) chartName = newEntry.Trim().Split('=')[1].Trim();
                    else if (hasSongArtist) chartArtist = newEntry.Trim().Split('=')[1].Trim();
                    else if (hasSongCharter) charter = newEntry.Trim().Split('=')[1].Trim();
                    else if (hasFrets) frets = newEntry.Trim().Split('=')[1].Trim();
                    if (!(chartName is null || chartArtist is null || charter is null)) break;
                }

                if (charter is null && !(frets is null))
                {
                    charter = frets; // add support for CH defaulting to frets if charter doesn't exist
                }

                if (chartName is null) chartName = "Unknown Name";
                if (chartArtist is null) chartArtist = "Unknown Artist";
                if (charter is null) charter = "Unknown Charter";

                ParseChart(lines);
            }
            catch (Exception e)
            {
                ErrorMessageAndClose(e, $"Cannot find path: {pathName}");
            }
        }



        private void ParseChart(string[] lines)
        {
            int state = -1;
            int startLine = 0;
            int endLine = 0;

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

        private void InitChartData(int state, int startLine, int endLine, string[] lines)
        {
            switch (state)
            {
                case -1:
                    return;

                // Parse through [Song]
                case 0:
                    for (int i = 0; i <= endLine - startLine; i++)
                    {
                        string curLine = lines[startLine + i];
                        string key = curLine.Split('=')[0].Trim();
                        string value = curLine.Split('=')[1].Replace("\"", "").Trim();
                        if (key.Contains("Resolution")) ticksPerQuarterNote = Int32.Parse(value.Trim());
                        MetaData.Add(key, value);
                    }
                    return;

                // Parse through [SyncTrack]
                case 1:
                    for (int i = 0; i <= endLine - startLine; i++)
                    {
                        this.numSyncEvents++;
                        string curLine = lines[startLine + i];
                        int timestamp = Int32.Parse(curLine.Trim().Split('=')[0]);
                        if (timestamp >= lastTimeStamp)
                        {
                            lastTimeStamp = timestamp;
                        }
                        // this mess of logic basically checks the first letter (whether it's B, T, or A)
                        string[] RHS = curLine.Split('=')[1].Trim().Split(' ');
                        switch (RHS[0].ToCharArray()[0])
                        {
                            case 'B':
                                int BPM = Int32.Parse(RHS[1]);
                                SyncData.Add(new BPMEvent(timestamp, BPM, RHS[1]));
                                break;
                            case 'T':
                                bool is4_4 = RHS.Length == 2;
                                int numerator = Int32.Parse(RHS[1].Trim());
                                int denom = is4_4 ? 2 : Int32.Parse(RHS[2].Trim()); // we're storing Log_2
                                SyncData.Add(new TSEvent(timestamp, numerator, denom, RHS[1]));
                                break;
                            case 'A':
                                // ignore anchor events
                                continue;
                            default:
                                ErrorMessageAndClose(new Exception("Incorrect SyncTrack format detected."), "Incorrect SyncTrack format detected.");
                                return;
                        }
                    }
                    return;
                case 2:
                    for (int i = 0; i <= endLine - startLine; i++)
                    {
                        string curLine = Form1.StripHTML(lines[startLine + i]);
                        int timestamp = Int32.Parse(curLine.Trim().Split('=')[0]);
                        if (timestamp >= lastTimeStamp)
                        {
                            lastTimeStamp = timestamp;
                        }
                        // this mess of logic basically checks the first letter (whether it's N, S, or E)
                        string curEvent = curLine.Split('=')[1].Trim().Split('\"')[1];
                        if (curEvent.StartsWith("lyric "))
                        {
                            string lyric = curEvent.Substring("lyric ".Length);
                            EventsData.Add(new LyricEvent(timestamp, lyric, curLine.Split('=')[1].Trim()));
                        }
                        else if (curEvent.StartsWith("section "))
                        {
                            string section = curEvent.Substring("section ".Length).Trim();
                            EventsData.Add(new SectionEvent(timestamp, section, numSections, curLine.Split('=')[1].Trim()));
                            numSections++;
                        }
                        else
                        {
                            EventsData.Add(new TextEvent(timestamp, curEvent, curLine.Split('=')[1].Trim()));
                        }
                    }
                    return;
                case 3:
                    for (int i = 0; i <= endLine - startLine; i++)
                    {
                        string curLine = lines[startLine + i];
                        int timestamp = Int32.Parse(curLine.Trim().Split('=')[0]);
                        if (timestamp >= lastTimeStamp)
                        {
                            lastTimeStamp = timestamp;
                        }
                        string[] RHS = curLine.Split('=')[1].Trim().Split(' ');
                        switch (RHS[0].ToCharArray()[0])
                        {
                            case 'N':
                                int noteval = Int32.Parse(RHS[1]);
                                // when the imposter is SUS am i right???
                                int susval = Int32.Parse(RHS[2]);
                                if (ExpertData.Count != 0)
                                {
                                    if (timestamp != ExpertData[ExpertData.Count - 1].timestamp)
                                    {
                                        this.numNotes++;
                                    }
                                }
                                else
                                {
                                    this.numNotes = 1;
                                }
                                ExpertData.Add(new NoteEvent(timestamp, noteval, susval, this.numNotes - 1, curLine.Split('=')[1].Trim()));

                                continue;

                            case 'S':
                                int specval = Int32.Parse(RHS[1]);
                                int speclen = Int32.Parse(RHS[2]);
                                ExpertData.Add(new SpecialEvent(timestamp, specval, speclen, curLine.Split('=')[1].Trim()));

                                continue;

                            case 'E':
                                ExpertData.Add(new TrackEvent(timestamp, RHS[1], curLine.Split('=')[1].Trim()));

                                continue;

                            default:
                                ErrorMessageAndClose(new Exception("Incorrect ChartType format detected."), "Incorrect ChartType format detected.");

                                break;
                        }
                    }
                    return;
                case 4:

                    return;
                case 5:

                    return;

                case 6:

                    return;

                default: return;
            }
        }

        public int NumNotesDuration(int tickA, int tickB, List<ChartEvent> eventList)
        {
            int numNotes = 0;
            int mostrecentTimestamp = -1;
            foreach(ChartEvent e in eventList)
            { 
                if(e.eventType == "N" && e.timestamp >= tickA && mostrecentTimestamp != e.timestamp)
                {
                    numNotes++;
                }
                if (e.timestamp > tickB) break;
                mostrecentTimestamp = e.timestamp;
            }
            return numNotes;
        }

        public NoteEvent GetNextNote(int timestamp, List<ChartEvent> eventList)
        {
            NoteEvent note = null;
            foreach (ChartEvent e in eventList)
            {
                if (e.eventType == "N")
                {
                    note = e as NoteEvent;
                    if (timestamp >= note.timestamp)
                    {
                        break;
                    }
                }
            }
            if (note is null) ErrorMessageAndClose(new Exception("no next note"), "there was no next note, to the pokey with you");
            return note;
        }



    }

    public class ChartGenerator
    {
        private List<string> output;

        private int startTimestamp;

        private int endTimestamp;

        private string startSection;

        private string endSection;

        private int startIndex;

        private int endIndex;

        private NoteEvent lastNote;

        private int sectionLength;

        public int iterations { get; private set; }

        private int nonoNotes;

        private int newNoteCount;

        private List<string> timestamps;

        private int cutoffTimestamp;

        public Chart Chart { get; private set; }

        public int numNotes { get; private set; }

        public ChartGenerator(Chart baseChart)
        {
            this.Chart = baseChart;
            this.newNoteCount = 0;
            this.startIndex = 0;
            this.endIndex = 0;
            this.sectionLength = 0;
            this.iterations = 0;
            this.nonoNotes = 0;
            this.cutoffTimestamp = 0;
            this.output = new List<string>();
            this.timestamps = new List<string>();
        }

        public void Generate(int numNotes, int startTimestamp, int endTimestamp, string startSection, string endSection, string path)
        {
            this.numNotes = numNotes;
            this.startTimestamp = startTimestamp;
            this.endTimestamp = endTimestamp;
            this.startSection = startSection;
            this.endSection = endSection;

            foreach (ChartEvent chartevent in Chart.ExpertData)
            {
                if (chartevent.eventType == "N")
                {
                    NoteEvent note = chartevent as NoteEvent;

                    if (note.timestamp == startTimestamp)
                    {
                        startIndex = note.NoteIndex;
                    }

                    else if (note.timestamp < endTimestamp)
                    {
                        lastNote = note;
                    }
                }
            }

            endIndex = lastNote.NoteIndex;

            sectionLength = endIndex - startIndex;

            iterations = numNotes / sectionLength;

            nonoNotes = numNotes % sectionLength;

            if (nonoNotes > 0)
            {
                iterations += 1;
            }

            GenerateMetaData();
            GenerateSyncData(iterations);
            GenerateEventData();
            GenerateExpertChart(iterations);

            using (StreamWriter writer = File.CreateText(path + "\\notes-3999.chart"))
            {
                foreach (string line in output)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private void GenerateMetaData()
        {
            output.Add("[Song]");
            output.Add("{");

            foreach (string key in Chart.MetaData.Keys)
            {
                switch (key)
                {
                    case var k when k == "Name":
                        output.Add($" {key} = \"{numNotes} {Chart.chartName} ({startSection} - {endSection})\"");
                        break;
                    case var k when k == "Artist":
                        output.Add($" {key} = \"{Chart.chartArtist}\"");
                        break;
                    case var k when k == "Charter":
                        output.Add($" {key} = \"{Chart.charter}\"");
                        break;
                    case var k when k == "Album" || key == "Year" || key == "Genre" || key == "MediaType" || key == "MusicStream":
                        output.Add($" {key} = \"{Chart.MetaData[key]}\"");
                        break;
                    default:
                        output.Add($" {key} = {Chart.MetaData[key]}");
                        break;

                }
            }

            output.Add("}");
        }

        private void GenerateSyncData(int iterations)
        {
            output.Add("[SyncTrack]");
            output.Add("{");

            for (int i = 0; i < iterations; i++)
            {
                foreach (SyncEvent chartevent in Chart.SyncData)
                {
                    int newTimestamp = -1;
                    if (chartevent.timestamp == 0)
                    {
                        newTimestamp = (chartevent.timestamp + (i * (endTimestamp - startTimestamp)));
                        
                    }
                    else if (chartevent.timestamp >= startTimestamp && chartevent.timestamp <= endTimestamp)
                    {
                        newTimestamp = (chartevent.timestamp - startTimestamp + (i * (endTimestamp - startTimestamp)));
                    }
                    else                    
                    {
                        continue;
                    }

                    // Possible boilerplate code to add most recent TS + BPM event
                    //if(chartevent.timestamp == startTimestamp)
                    //{
                    //    SyncEvent newEvent = null;
                    //    foreach(SyncEvent s in Chart.SyncData)
                    //    {
                    //        newEvent = s;
                    //        if (s.timestamp == startTimestamp) break;
                    //    }
                    //    output.Add($" {newTimestamp} = {chartevent.eventType} {chartevent.RawData}");
                    //}

                    output.Add($" {newTimestamp} = {chartevent.eventType} {chartevent.RawData}");
                }
            }

            output.Add("}");
        }

        private void GenerateEventData()
        {
            output.Add("[Events]");
            output.Add("{");

            foreach (GlobalEvent chartevent in Chart.EventsData)
            {
                if (chartevent.timestamp >= startTimestamp && chartevent.timestamp <= endTimestamp)
                {
                    output.Add($" {chartevent.timestamp - startTimestamp} = {chartevent.RawData}");
                }
            }

            output.Add("}");
        }

        private void GenerateExpertChart(int iterations)
        {
            output.Add("[ExpertSingle]");
            output.Add("{");

            for (int i = 0; i < iterations; i++)
            {
                foreach (ChartEvent chartevent in Chart.ExpertData)
                {
                    if (chartevent.timestamp >= startTimestamp && chartevent.timestamp <= endTimestamp)
                    {
                        int newTimestamp = (chartevent.timestamp - startTimestamp + (i * (endTimestamp - startTimestamp)));
                        output.Add($" {newTimestamp} = {chartevent.RawData}");
                        if (!timestamps.Contains($"{newTimestamp}") && chartevent.eventType == "N")
                        {
                            timestamps.Add($"{newTimestamp}");
                            newNoteCount++;
                        }

                        if (newNoteCount >= numNotes)
                        {
                            break;
                        }
                    }
                }

                if (newNoteCount >= numNotes)
                {
                    break;
                }
            }

            output.Add("}");
        }
    }

    public class AudioUtils
    {
        public string SourcePath { get; private set; }

        public string DestinationPath { get; private set; }

        public AudioUtils()
        {

        }

        public void ConvertToWav(string sourcePath, string destPath)
        {
            this.SourcePath = sourcePath;
            this.DestinationPath = destPath;

            ConstructWav(sourcePath, destPath, 1);
        }

        public void ConvertToWavAndMultiply(string sourcePath, string destPath, int iterations)
        {
            this.SourcePath = sourcePath;
            this.DestinationPath = destPath;

            ConstructWav(sourcePath, destPath, iterations);
        }

        private void ConstructWav(string sourcePath, string destPath, int iterations)
        {
            WaveFormat waveFormat = new WaveFormat(8000, 8, 2);
            string outputPath = destPath;

            string fileFormat = sourcePath.Substring(sourcePath.Length-4);

            StreamReader inputStream = new StreamReader(sourcePath);
            StreamWriter outputStream = new StreamWriter(outputPath);

            if(fileFormat == ".mp3")
            {
                using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(inputStream.BaseStream)))
                using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream.BaseStream, waveStream.WaveFormat))
                {
                    byte[] bytes = new byte[waveStream.Length];
                    waveStream.Read(bytes, 0, (int)waveStream.Length);

                    for (int i = 0; i < iterations; i++)
                    {
                        waveFileWriter.Write(bytes, 0, bytes.Length);
                    }

                    waveFileWriter.Flush();
                }
            }

            else if(fileFormat == ".ogg")
            {
                using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(new VorbisWaveReader(inputStream.BaseStream)))
                using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream.BaseStream, waveStream.WaveFormat))
                {
                    byte[] bytes = new byte[waveStream.Length];
                    waveStream.Read(bytes, 0, (int)waveStream.Length);

                    for (int i = 0; i < iterations; i++)
                    {
                        waveFileWriter.Write(bytes, 0, bytes.Length);
                    }

                    waveFileWriter.Flush();
                }
            }

            else
            {
                MessageBox.Show("Unsupported Song Format");
            }
        }
    }

    public class LimitedQueue<T> : Queue<T>
    {
        public int Limit { get; set; }

        public LimitedQueue(int limit) : base(limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(item);
        }
    }

    public class WavFileUtils
    {
        public WavFileUtils()
        {

        }

        public void TrimWavFile(string inPath, string outPath, float cutFromStart, float cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    float startPos = cutFromStart * 1000 * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    float endBytes = cutFromEnd * 1000 * bytesPerMillisecond;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    float endPos = endBytes;

                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        public void MultiplyWav(string inPath, string outPath, int iterations)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    for(int i = 0; i<iterations; i++)
                    {
                        reader.Position = 0;
                        byte[] buffer = new byte[1024];

                        while (reader.Position < reader.Length)
                        {
                            int bytesRequired = (int)(reader.Length - reader.Position);
                            if (bytesRequired > 0)
                            {
                                int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                                int bytesRead = reader.Read(buffer, 0, bytesToRead);
                                if (bytesRead > 0)
                                {
                                    writer.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, float startPos, float endPos)
        {
            reader.Position = (int)startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }

}
