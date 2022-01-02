namespace _3999_gen
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGenerate = new System.Windows.Forms.Button();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnLoad = new System.Windows.Forms.Button();
            this.rdoBtnSection = new System.Windows.Forms.RadioButton();
            this.rdoBtnSong = new System.Windows.Forms.RadioButton();
            this.cmboBoxSection = new System.Windows.Forms.ComboBox();
            this.rdoBtn3999 = new System.Windows.Forms.RadioButton();
            this.rdoBtnCustom = new System.Windows.Forms.RadioButton();
            this.rdoBtnIteration = new System.Windows.Forms.RadioButton();
            this.grpBoxStyle = new System.Windows.Forms.GroupBox();
            this.grpBoxSelection = new System.Windows.Forms.GroupBox();
            this.txtBoxNoteCount = new System.Windows.Forms.TextBox();
            this.txtBoxLoopCount = new System.Windows.Forms.TextBox();
            this.lblSong = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmboBoxSection2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpBoxStyle.SuspendLayout();
            this.grpBoxSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(12, 260);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(234, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generate!";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // fileDialog
            // 
            this.fileDialog.FileName = "notes.chart";
            this.fileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.FileDialog_FileOk);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 37);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load Chart";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // rdoBtnSection
            // 
            this.rdoBtnSection.AutoSize = true;
            this.rdoBtnSection.Checked = true;
            this.rdoBtnSection.Location = new System.Drawing.Point(18, 19);
            this.rdoBtnSection.Name = "rdoBtnSection";
            this.rdoBtnSection.Size = new System.Drawing.Size(61, 17);
            this.rdoBtnSection.TabIndex = 2;
            this.rdoBtnSection.TabStop = true;
            this.rdoBtnSection.Text = "Section";
            this.rdoBtnSection.UseVisualStyleBackColor = true;
            // 
            // rdoBtnSong
            // 
            this.rdoBtnSong.AutoSize = true;
            this.rdoBtnSong.Location = new System.Drawing.Point(18, 42);
            this.rdoBtnSong.Name = "rdoBtnSong";
            this.rdoBtnSong.Size = new System.Drawing.Size(84, 17);
            this.rdoBtnSong.TabIndex = 3;
            this.rdoBtnSong.Text = "Whole Song";
            this.rdoBtnSong.UseVisualStyleBackColor = true;
            // 
            // cmboBoxSection
            // 
            this.cmboBoxSection.FormattingEnabled = true;
            this.cmboBoxSection.Location = new System.Drawing.Point(125, 84);
            this.cmboBoxSection.Name = "cmboBoxSection";
            this.cmboBoxSection.Size = new System.Drawing.Size(121, 21);
            this.cmboBoxSection.TabIndex = 4;
            this.cmboBoxSection.Text = "Start Section";
            this.cmboBoxSection.SelectedIndexChanged += new System.EventHandler(this.cmboBoxSection_SelectedIndexChanged);
            // 
            // rdoBtn3999
            // 
            this.rdoBtn3999.AutoSize = true;
            this.rdoBtn3999.Checked = true;
            this.rdoBtn3999.Location = new System.Drawing.Point(26, 19);
            this.rdoBtn3999.Name = "rdoBtn3999";
            this.rdoBtn3999.Size = new System.Drawing.Size(80, 17);
            this.rdoBtn3999.TabIndex = 5;
            this.rdoBtn3999.TabStop = true;
            this.rdoBtn3999.Text = "3999 Notes";
            this.rdoBtn3999.UseVisualStyleBackColor = true;
            // 
            // rdoBtnCustom
            // 
            this.rdoBtnCustom.AutoSize = true;
            this.rdoBtnCustom.Location = new System.Drawing.Point(26, 42);
            this.rdoBtnCustom.Name = "rdoBtnCustom";
            this.rdoBtnCustom.Size = new System.Drawing.Size(105, 17);
            this.rdoBtnCustom.TabIndex = 6;
            this.rdoBtnCustom.Text = "Number of Notes";
            this.rdoBtnCustom.UseVisualStyleBackColor = true;
            // 
            // rdoBtnIteration
            // 
            this.rdoBtnIteration.AutoSize = true;
            this.rdoBtnIteration.Location = new System.Drawing.Point(26, 65);
            this.rdoBtnIteration.Name = "rdoBtnIteration";
            this.rdoBtnIteration.Size = new System.Drawing.Size(117, 17);
            this.rdoBtnIteration.TabIndex = 7;
            this.rdoBtnIteration.Text = "Number of Repeats";
            this.rdoBtnIteration.UseVisualStyleBackColor = true;
            // 
            // grpBoxStyle
            // 
            this.grpBoxStyle.Controls.Add(this.rdoBtn3999);
            this.grpBoxStyle.Controls.Add(this.rdoBtnIteration);
            this.grpBoxStyle.Controls.Add(this.rdoBtnCustom);
            this.grpBoxStyle.Location = new System.Drawing.Point(12, 151);
            this.grpBoxStyle.Name = "grpBoxStyle";
            this.grpBoxStyle.Size = new System.Drawing.Size(150, 90);
            this.grpBoxStyle.TabIndex = 8;
            this.grpBoxStyle.TabStop = false;
            this.grpBoxStyle.Text = "Style";
            // 
            // grpBoxSelection
            // 
            this.grpBoxSelection.Controls.Add(this.rdoBtnSection);
            this.grpBoxSelection.Controls.Add(this.rdoBtnSong);
            this.grpBoxSelection.Location = new System.Drawing.Point(12, 66);
            this.grpBoxSelection.Name = "grpBoxSelection";
            this.grpBoxSelection.Size = new System.Drawing.Size(107, 68);
            this.grpBoxSelection.TabIndex = 9;
            this.grpBoxSelection.TabStop = false;
            this.grpBoxSelection.Text = "Selection";
            // 
            // txtBoxNoteCount
            // 
            this.txtBoxNoteCount.Location = new System.Drawing.Point(168, 192);
            this.txtBoxNoteCount.Name = "txtBoxNoteCount";
            this.txtBoxNoteCount.Size = new System.Drawing.Size(77, 20);
            this.txtBoxNoteCount.TabIndex = 10;
            // 
            // txtBoxLoopCount
            // 
            this.txtBoxLoopCount.Location = new System.Drawing.Point(168, 215);
            this.txtBoxLoopCount.Name = "txtBoxLoopCount";
            this.txtBoxLoopCount.Size = new System.Drawing.Size(77, 20);
            this.txtBoxLoopCount.TabIndex = 11;
            // 
            // lblSong
            // 
            this.lblSong.AutoSize = true;
            this.lblSong.Location = new System.Drawing.Point(9, 9);
            this.lblSong.MaximumSize = new System.Drawing.Size(420, 30);
            this.lblSong.Name = "lblSong";
            this.lblSong.Size = new System.Drawing.Size(94, 13);
            this.lblSong.TabIndex = 12;
            this.lblSong.Text = "No Song Selected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "to";
            // 
            // cmboBoxSection2
            // 
            this.cmboBoxSection2.FormattingEnabled = true;
            this.cmboBoxSection2.Location = new System.Drawing.Point(274, 84);
            this.cmboBoxSection2.Name = "cmboBoxSection2";
            this.cmboBoxSection2.Size = new System.Drawing.Size(121, 21);
            this.cmboBoxSection2.TabIndex = 14;
            this.cmboBoxSection2.Text = "End Section";
            this.cmboBoxSection2.SelectedIndexChanged += new System.EventHandler(this.cmboBoxSection2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 244);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Created by RandomDays and BormoTime";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 297);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmboBoxSection2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSong);
            this.Controls.Add(this.txtBoxLoopCount);
            this.Controls.Add(this.txtBoxNoteCount);
            this.Controls.Add(this.grpBoxSelection);
            this.Controls.Add(this.grpBoxStyle);
            this.Controls.Add(this.cmboBoxSection);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnGenerate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(429, 336);
            this.MinimumSize = new System.Drawing.Size(429, 336);
            this.Name = "Form1";
            this.Text = "3999 Generator";
            this.grpBoxStyle.ResumeLayout(false);
            this.grpBoxStyle.PerformLayout();
            this.grpBoxSelection.ResumeLayout(false);
            this.grpBoxSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.RadioButton rdoBtnSection;
        private System.Windows.Forms.RadioButton rdoBtnSong;
        private System.Windows.Forms.ComboBox cmboBoxSection;
        private System.Windows.Forms.RadioButton rdoBtn3999;
        private System.Windows.Forms.RadioButton rdoBtnCustom;
        private System.Windows.Forms.RadioButton rdoBtnIteration;
        private System.Windows.Forms.GroupBox grpBoxStyle;
        private System.Windows.Forms.GroupBox grpBoxSelection;
        private System.Windows.Forms.TextBox txtBoxNoteCount;
        private System.Windows.Forms.TextBox txtBoxLoopCount;
        private System.Windows.Forms.Label lblSong;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmboBoxSection2;
        private System.Windows.Forms.Label label2;
    }
}

