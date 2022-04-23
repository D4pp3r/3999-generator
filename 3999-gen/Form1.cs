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
using System.Text.RegularExpressions;
using Xabe.FFmpeg;
using System.Diagnostics;

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
            lblSong.Text = $"{chart.chartName.Replace("&", "&&")} by {chart.chartArtist.Replace("&", "&&")} (Charted by: {chart.charter.Replace("&", "&&")})";
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

            if (cmboBoxSection.SelectedIndex > cmboBoxSection.SelectedIndex)
            {
                cmboBoxSection.SelectedIndex = cmboBoxSection2.SelectedIndex;
                return;
            }

            string curSection = Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", "");
            int index = cmboBoxSection.SelectedIndex;
            foreach (GlobalEvent g in chart.EventsData.FindAll(x => x.eventType == "section" && (x as SectionEvent).sectionIndex == index))
            {
                tickA = g.timestamp;
            }
            cmboBoxSection2.SelectedIndex = cmboBoxSection.SelectedIndex;
        }

        private void cmboBoxSection2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chart is null) return;

            if (cmboBoxSection2.SelectedIndex < cmboBoxSection.SelectedIndex)
            {
                cmboBoxSection2.SelectedIndex = cmboBoxSection.SelectedIndex;
                return;
            }

            string curSection = Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", "");

            bool tickFlag = false;

            int index = cmboBoxSection2.SelectedIndex + 1;
            if (index == cmboBoxSection2.Items.Count)
            {
                tickB = chart.lastTimeStamp;
            }

            foreach (GlobalEvent globalEvent in chart.EventsData.FindAll(x => x.eventType == "section"))
            {

                SectionEvent section = globalEvent as SectionEvent;
                if (index == section.sectionIndex)
                {
                    tickFlag = true;
                }
                if (tickFlag)
                {
                    tickB = section.timestamp;
                    if (tickB == chart.lastTimeStamp)
                    {
                        tickB = chart.lastTimeStamp + (4 * chart.ticksPerQuarterNote);
                    }
                    break;
                }
            }

        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            funnycode.Enqueue(e.KeyChar);
            string curstring = new string(funnycode.ToArray());
            if (curstring == "oboyoboy" || curstring == "54535453")
            {
                if (MessageBox.Show("Hyperspeed Unlocked!") == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            else if (curstring == "borm tim")
            {
                if (MessageBox.Show("Get your BormoTime Guitar from bormotime.com today!") == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

        }
        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            int numNotes = 3999;
            if (chart is null) return;
            if (tickA == -1 || tickB == -1) return;

            if (rdoBtn3999.Checked)
            {
                numNotes = 3999;
            }

            else if (rdoBtnCustom.Checked)
            {
                numNotes = int.Parse(txtBoxNoteCount.Text);
            }

            else if (rdoBtnIteration.Checked)
            {
                numNotes = 3999;
            }

            ChartGenerator gen = new ChartGenerator(chart);

            if (rdoBtnSong.Checked)
            {
                tickA = 0;
                tickB = chart.lastTimeStamp + chart.ticksPerQuarterNote * 4;
            }

            if (rdoBtnCustom.Checked)
            {
                if (!Int32.TryParse(txtBoxNoteCount.Text, out numNotes) || numNotes <= 0)
                {
                    MessageBox.Show("Not a valid note count, to the pokey with you.");
                    return;
                }
            }
            else if (rdoBtnIteration.Checked)
            {
                int numIterations = 0;
                if (!Int32.TryParse(txtBoxLoopCount.Text, out numIterations) || numIterations <= 0)
                {
                    MessageBox.Show("Not a valid note count, to the pokey with you.");
                    return;
                }
                numNotes = numIterations * chart.NumNotesDuration(tickA, tickB, chart.ExpertData);
            }



            gen.Generate(numNotes, tickA, tickB, Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", ""), Regex.Replace(cmboBoxSection2.Text, "[0-9]+:[ ]", ""), $"{chart.pathName}\\..\\..\\{numNotes}_{chart.chartName}_{Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", "")}-{Regex.Replace(cmboBoxSection2.Text, "[0-9]+:[ ]", "")}");

            if (chart.MetaData["MusicStream"] is null)
            {

            }

            else
            {
                float[] seconds = TimestampToSeconds(chart, tickA, tickB/* + (chart.ticksPerQuarterNote / 4)*/);
                if (seconds != new float[] { -1, -1 })
                {
                    await AudioGenerator.Generate("\"" + filePath.Substring(0, filePath.Length-11) + chart.MetaData["MusicStream"] + "\"", "\"" + $"{chart.pathName}\\..\\..\\{numNotes}_{chart.chartName}_{Regex.Replace(cmboBoxSection.Text, "[0-9]+:[ ]", "")}-{Regex.Replace(cmboBoxSection2.Text, "[0-9]+:[ ]", "")}" + "\\song.mp3" + "\"", seconds[0], seconds[1], gen.iterations);
                }
            }

            MessageBox.Show("HOLY FUCK !!!!", "YOU ARE WINNER!");
        }

        private float[] TimestampToSeconds(Chart chart, float startTimestamp, float endTimestamp)
        {
            int resolution = chart.ticksPerQuarterNote;
            float bpm = 0;
            float lastBpm = 0;
            float tickStart = 0;
            float tickEnd = 0;
            bool shit = false;
            bool first = true;
            float startSeconds = 0;
            float endSeconds = 0;
            float endDiff = 0;
            int lastBPMTimestamp = 0;

            float secondsBeforeStart = 0;

            tickStart = startTimestamp;
            tickEnd = endTimestamp;

            foreach (SyncEvent sync in chart.SyncData)
            {
                if (sync.eventType == "B")
                {
                    BPMEvent bpmevent = sync as BPMEvent;

                    if(first)
                    {
                        bpm = bpmevent.BPM / 1000.0f;
                        lastBpm = bpmevent.BPM / 1000.0f;
                        lastBPMTimestamp = bpmevent.timestamp;

                        first = false;
                    }

                    if(bpmevent.timestamp <= tickStart)
                    {
                        secondsBeforeStart += ((((bpmevent.timestamp - lastBPMTimestamp) / resolution) * 60) / lastBpm);
                        lastBPMTimestamp = bpmevent.timestamp;
                        lastBpm = bpmevent.BPM / 1000;
                    }

                    else if(bpmevent.timestamp < tickEnd)
                    {
                        shit = true;
                    }
                }
            }

            startSeconds = ((((tickStart - lastBPMTimestamp) / resolution) * 60) / bpm) + secondsBeforeStart;

            if(!shit)
            {
                endDiff = ((((tickEnd - tickStart) / resolution) * 60) / bpm);

                endSeconds = endDiff + startSeconds;
            }

            float[] output = new float[] { startSeconds, endSeconds };

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

        public bool isNewNote { get; private set; }

        public NoteEvent(int timestamp, int newNoteValue, int newSustainLength, int noteIndex, bool isNewNote, string newData) : base(timestamp, "N", newData)
        {
            this.NoteValue = newNoteValue;
            this.SustainLength = newSustainLength;
            this.NoteType = (NoteEnum)this.NoteValue;
            this.NoteIndex = noteIndex;
            this.isNewNote = isNewNote;
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

        public List<string> iniData { get; private set; }

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
                this.iniData = new List<string>();

                string[] songini = File.ReadAllLines($"{filename.Split(new[] { "notes.chart" }, StringSplitOptions.None)[0]}song.ini");
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
                    iniData.Add(entry);
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
                    state = -1;
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
                                bool isNewNote = false;
                                if (ExpertData.Count != 0)
                                {
                                    if (timestamp != ExpertData[ExpertData.Count - 1].timestamp)
                                    {
                                        this.numNotes++;
                                        isNewNote = true;
                                    }
                                }
                                else
                                {
                                    this.numNotes = 1;
                                    if (ExpertData.Count == 0)
                                    {
                                        isNewNote = true;
                                    }
                                }
                                ExpertData.Add(new NoteEvent(timestamp, noteval, susval, this.numNotes - 1, isNewNote, curLine.Split('=')[1].Trim()));

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
            foreach (NoteEvent n in GetSection(tickA, tickB, eventList).FindAll(x => x.isNewNote))
            {
                numNotes++;
            }
            return numNotes;
        }

        public List<NoteEvent> GetSection(int tickA, int tickB, List<ChartEvent> eventList)
        {
            List<NoteEvent> notes = new List<NoteEvent>();
            foreach (ChartEvent e in eventList.FindAll(x => x.eventType == "N" && x.timestamp >= tickA && x.timestamp < tickB))
            {
                notes.Add(e as NoteEvent);
            }
            return notes;
        }

        public int NumNewNotes(List<NoteEvent> noteList)
        {
            int i = 0;
            foreach (NoteEvent n in noteList.FindAll(x => x.isNewNote))
            {
                i++;
            }
            return i;
        }

        public NoteEvent GetNextNote(int timestamp, List<ChartEvent> eventList)
        {
            NoteEvent n = null;
            foreach (ChartEvent e in eventList.FindAll(x => x.eventType == "N"))
            {
                if (e.timestamp >= timestamp)
                {
                    n = e as NoteEvent;
                    break;
                }
            }
            return n;
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

        private List<NoteEvent> ExpertNotes;

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
            this.ExpertNotes = Chart.GetSection(startTimestamp, endTimestamp, Chart.ExpertData);
            this.sectionLength = Chart.NumNewNotes(this.ExpertNotes);

            if (sectionLength == 0 && MessageBox.Show("cmon bruh") == DialogResult.OK)
            {
                Application.Exit();
            }


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

            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(path);
            }

            using (StreamWriter writer = File.CreateText($"{path}\\notes.chart"))
            {
                foreach (string line in output)
                {
                    writer.WriteLine(line);
                }
            }

            using (StreamWriter writer = File.CreateText($"{path}\\song.ini"))
            {
                foreach (string line in Chart.iniData)
                {
                    if (line.Trim().ToLower().StartsWith("name"))
                    {
                        if (startSection != endSection)
                        {
                            writer.WriteLine($"Name = {numNotes} {Chart.chartName} ({startSection} - {endSection}");
                        }
                        else
                        {
                            writer.WriteLine($"Name = {numNotes} {Chart.chartName} ({startSection})");
                        }
                        continue;
                    }
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
                    if (chartevent.timestamp <= startTimestamp)
                    {
                        newTimestamp = (0 + (i * (endTimestamp - startTimestamp)));

                    }
                    else if (chartevent.timestamp >= startTimestamp && chartevent.timestamp < endTimestamp)
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
                if (chartevent.timestamp >= startTimestamp && chartevent.timestamp < endTimestamp)
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

            for (int iter = 0; iter < iterations; iter++)
            {
                foreach (NoteEvent n in ExpertNotes)
                {
                    if (n.isNewNote)
                    {
                        newNoteCount++;
                        if (newNoteCount > numNotes)
                        {
                            break;
                        }
                    }
                    int newTimestamp = n.timestamp - startTimestamp + (iter * (endTimestamp - startTimestamp));
                    output.Add($" {newTimestamp} = {n.RawData}");
                }
                if (newNoteCount > numNotes)
                {
                    break;
                }
            }

            output.Add("}");
        }
    }

    public static class AudioGenerator
    {
        public static async Task Generate(string path, string output, float startSeconds, float endSeconds, int iterations)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(path);

            var conversionResult = await FFmpeg.Conversions.New().SetOverwriteOutput(true).AddParameter($"-i {path} -ss {startSeconds} -to {endSeconds} -c copy C:/Temp/temp.mp3").Start();

            var conversionResult2 = await FFmpeg.Conversions.New().SetOverwriteOutput(true).AddParameter($"-stream_loop {iterations} -i C:/Temp/temp.mp3 -c copy {output}").Start();

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



}
