namespace MapViewer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            UxMapList = new ListBox();
            splitContainer1 = new SplitContainer();
            UxMapCanvas = new Canvas();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            UxBG0Toggle = new ToolStripButton();
            UxBG1Toggle = new ToolStripButton();
            UxBG2Toggle = new ToolStripButton();
            UxBG3Toggle = new ToolStripButton();
            UxConnectorsToggle = new ToolStripButton();
            UxEventsToggle = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel2 = new ToolStripLabel();
            UxZoomOption = new ToolStripComboBox();
            UxResetView = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            UxTotalMapsLabel = new ToolStripStatusLabel();
            UxMapOffsetLabel = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)UxMapCanvas).BeginInit();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // UxMapList
            // 
            UxMapList.Dock = DockStyle.Fill;
            UxMapList.Font = new Font("Lucida Console", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UxMapList.FormattingEnabled = true;
            UxMapList.ItemHeight = 13;
            UxMapList.Location = new Point(0, 0);
            UxMapList.Name = "UxMapList";
            UxMapList.ScrollAlwaysVisible = true;
            UxMapList.Size = new Size(198, 559);
            UxMapList.TabIndex = 0;
            UxMapList.SelectedIndexChanged += UxMapList_SelectedIndexChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(UxMapList);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(UxMapCanvas);
            splitContainer1.Panel2.Controls.Add(toolStrip1);
            splitContainer1.Size = new Size(802, 559);
            splitContainer1.SplitterDistance = 198;
            splitContainer1.TabIndex = 999;
            // 
            // UxMapCanvas
            // 
            UxMapCanvas.BackColor = Color.Black;
            UxMapCanvas.Dock = DockStyle.Fill;
            UxMapCanvas.Location = new Point(0, 25);
            UxMapCanvas.Name = "UxMapCanvas";
            UxMapCanvas.Size = new Size(600, 534);
            UxMapCanvas.TabIndex = 2;
            UxMapCanvas.TabStop = false;
            UxMapCanvas.ZoomMode = 1;
            // 
            // toolStrip1
            // 
            toolStrip1.GripMargin = new Padding(0);
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, UxBG0Toggle, UxBG1Toggle, UxBG2Toggle, UxBG3Toggle, UxConnectorsToggle, UxEventsToggle, toolStripSeparator1, toolStripLabel2, UxZoomOption, UxResetView });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(5, 0, 1, 0);
            toolStrip1.Size = new Size(600, 25);
            toolStrip1.TabIndex = 1;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(50, 22);
            toolStripLabel1.Text = "Toggles:";
            // 
            // UxBG0Toggle
            // 
            UxBG0Toggle.Checked = true;
            UxBG0Toggle.CheckOnClick = true;
            UxBG0Toggle.CheckState = CheckState.Indeterminate;
            UxBG0Toggle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxBG0Toggle.Image = (Image)resources.GetObject("UxBG0Toggle.Image");
            UxBG0Toggle.ImageTransparentColor = Color.Magenta;
            UxBG0Toggle.Margin = new Padding(5, 1, 0, 2);
            UxBG0Toggle.Name = "UxBG0Toggle";
            UxBG0Toggle.Size = new Size(32, 22);
            UxBG0Toggle.Text = "BG0";
            UxBG0Toggle.CheckedChanged += UxToggle_CheckChanged;
            // 
            // UxBG1Toggle
            // 
            UxBG1Toggle.Checked = true;
            UxBG1Toggle.CheckOnClick = true;
            UxBG1Toggle.CheckState = CheckState.Indeterminate;
            UxBG1Toggle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxBG1Toggle.Image = (Image)resources.GetObject("UxBG1Toggle.Image");
            UxBG1Toggle.ImageTransparentColor = Color.Magenta;
            UxBG1Toggle.Margin = new Padding(5, 1, 0, 2);
            UxBG1Toggle.Name = "UxBG1Toggle";
            UxBG1Toggle.Size = new Size(32, 22);
            UxBG1Toggle.Text = "BG1";
            UxBG1Toggle.CheckedChanged += UxToggle_CheckChanged;
            // 
            // UxBG2Toggle
            // 
            UxBG2Toggle.Checked = true;
            UxBG2Toggle.CheckOnClick = true;
            UxBG2Toggle.CheckState = CheckState.Indeterminate;
            UxBG2Toggle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxBG2Toggle.Image = (Image)resources.GetObject("UxBG2Toggle.Image");
            UxBG2Toggle.ImageTransparentColor = Color.Magenta;
            UxBG2Toggle.Margin = new Padding(5, 1, 0, 2);
            UxBG2Toggle.Name = "UxBG2Toggle";
            UxBG2Toggle.Size = new Size(32, 22);
            UxBG2Toggle.Text = "BG2";
            UxBG2Toggle.CheckedChanged += UxToggle_CheckChanged;
            // 
            // UxBG3Toggle
            // 
            UxBG3Toggle.Checked = true;
            UxBG3Toggle.CheckOnClick = true;
            UxBG3Toggle.CheckState = CheckState.Indeterminate;
            UxBG3Toggle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxBG3Toggle.Image = (Image)resources.GetObject("UxBG3Toggle.Image");
            UxBG3Toggle.ImageTransparentColor = Color.Magenta;
            UxBG3Toggle.Margin = new Padding(5, 1, 0, 2);
            UxBG3Toggle.Name = "UxBG3Toggle";
            UxBG3Toggle.Size = new Size(32, 22);
            UxBG3Toggle.Text = "BG3";
            UxBG3Toggle.CheckedChanged += UxToggle_CheckChanged;
            // 
            // UxConnectorsToggle
            // 
            UxConnectorsToggle.Checked = true;
            UxConnectorsToggle.CheckOnClick = true;
            UxConnectorsToggle.CheckState = CheckState.Indeterminate;
            UxConnectorsToggle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxConnectorsToggle.Image = (Image)resources.GetObject("UxConnectorsToggle.Image");
            UxConnectorsToggle.ImageTransparentColor = Color.Magenta;
            UxConnectorsToggle.Margin = new Padding(5, 1, 0, 2);
            UxConnectorsToggle.Name = "UxConnectorsToggle";
            UxConnectorsToggle.Size = new Size(72, 22);
            UxConnectorsToggle.Text = "Connectors";
            UxConnectorsToggle.CheckedChanged += UxToggle_CheckChanged;
            // 
            // UxEventsToggle
            // 
            UxEventsToggle.Checked = true;
            UxEventsToggle.CheckOnClick = true;
            UxEventsToggle.CheckState = CheckState.Indeterminate;
            UxEventsToggle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxEventsToggle.Image = (Image)resources.GetObject("UxEventsToggle.Image");
            UxEventsToggle.ImageTransparentColor = Color.Magenta;
            UxEventsToggle.Margin = new Padding(5, 1, 0, 2);
            UxEventsToggle.Name = "UxEventsToggle";
            UxEventsToggle.Size = new Size(45, 22);
            UxEventsToggle.Text = "Events";
            UxEventsToggle.CheckedChanged += UxToggle_CheckChanged;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Margin = new Padding(5, 0, 0, 0);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Margin = new Padding(5, 1, 0, 2);
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(39, 22);
            toolStripLabel2.Text = "Zoom";
            // 
            // UxZoomOption
            // 
            UxZoomOption.AutoSize = false;
            UxZoomOption.DropDownStyle = ComboBoxStyle.DropDownList;
            UxZoomOption.DropDownWidth = 60;
            UxZoomOption.Items.AddRange(new object[] { "50%", "100%", "200%", "300%", "400%", "500%" });
            UxZoomOption.Name = "UxZoomOption";
            UxZoomOption.Size = new Size(60, 23);
            UxZoomOption.SelectedIndexChanged += UxZoomOption_SelectedIndexChanged;
            // 
            // UxResetView
            // 
            UxResetView.DisplayStyle = ToolStripItemDisplayStyle.Text;
            UxResetView.Image = (Image)resources.GetObject("UxResetView.Image");
            UxResetView.ImageTransparentColor = Color.Magenta;
            UxResetView.Name = "UxResetView";
            UxResetView.Size = new Size(67, 22);
            UxResetView.Text = "Reset View";
            UxResetView.Click += UxResetView_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { UxTotalMapsLabel, UxMapOffsetLabel });
            statusStrip1.Location = new Point(0, 559);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(802, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // UxTotalMapsLabel
            // 
            UxTotalMapsLabel.Name = "UxTotalMapsLabel";
            UxTotalMapsLabel.Size = new Size(64, 17);
            UxTotalMapsLabel.Text = "Total Maps";
            // 
            // UxMapOffsetLabel
            // 
            UxMapOffsetLabel.Margin = new Padding(50, 3, 0, 2);
            UxMapOffsetLabel.Name = "UxMapOffsetLabel";
            UxMapOffsetLabel.Size = new Size(137, 17);
            UxMapOffsetLabel.Text = "Selected Map Offset: 0x0";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(802, 581);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Map Viewer";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)UxMapCanvas).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox UxMapList;
        private SplitContainer splitContainer1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel UxTotalMapsLabel;
        private ToolStripStatusLabel UxMapOffsetLabel;
        private ToolStrip toolStrip1;
        private ToolStripButton UxBG3Toggle;
        private ToolStripButton UxBG1Toggle;
        private ToolStripButton UxBG2Toggle;
        private ToolStripButton UxConnectorsToggle;
        private ToolStripButton UxEventsToggle;
        private ToolStripLabel toolStripLabel1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripLabel toolStripLabel2;
        private ToolStripComboBox UxZoomOption;
        private ToolStripButton UxBG0Toggle;
        private Canvas UxMapCanvas;
        private ToolStripButton UxResetView;
    }
}
