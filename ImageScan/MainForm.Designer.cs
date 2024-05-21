
namespace ImageScan
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            uxOBJPalette = new System.Windows.Forms.RadioButton();
            uxBGPalette = new System.Windows.Forms.RadioButton();
            uxAddress = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            uxImageWidth = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            uxBackColor = new System.Windows.Forms.PictureBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            uxResultsPrio = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            uxScanNow = new System.Windows.Forms.Button();
            label7 = new System.Windows.Forms.Label();
            uxCurrentResult = new System.Windows.Forms.NumericUpDown();
            uxResultsFound = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            uxText = new System.Windows.Forms.TextBox();
            uxTextFormat = new System.Windows.Forms.CheckBox();
            uxTileSize = new System.Windows.Forms.NumericUpDown();
            label4 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            uxSize = new System.Windows.Forms.Label();
            uxTileIndex = new System.Windows.Forms.NumericUpDown();
            button1 = new System.Windows.Forms.Button();
            uxImage = new DoubleBufferedPanel();
            UxEnableTileIndexOption = new System.Windows.Forms.CheckBox();
            panel1 = new System.Windows.Forms.Panel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uxImageWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)uxBackColor).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uxCurrentResult).BeginInit();
            ((System.ComponentModel.ISupportInitialize)uxTileSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)uxTileIndex).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(uxOBJPalette);
            groupBox1.Controls.Add(uxBGPalette);
            groupBox1.Location = new System.Drawing.Point(2, 48);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(117, 47);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Palette";
            // 
            // uxOBJPalette
            // 
            uxOBJPalette.AutoSize = true;
            uxOBJPalette.Location = new System.Drawing.Point(59, 19);
            uxOBJPalette.Name = "uxOBJPalette";
            uxOBJPalette.Size = new System.Drawing.Size(45, 19);
            uxOBJPalette.TabIndex = 1;
            uxOBJPalette.Text = "OBJ";
            uxOBJPalette.UseVisualStyleBackColor = true;
            uxOBJPalette.CheckedChanged += OnPaletteCheckedChanged;
            // 
            // uxBGPalette
            // 
            uxBGPalette.AutoSize = true;
            uxBGPalette.Checked = true;
            uxBGPalette.Location = new System.Drawing.Point(13, 19);
            uxBGPalette.Name = "uxBGPalette";
            uxBGPalette.Size = new System.Drawing.Size(40, 19);
            uxBGPalette.TabIndex = 0;
            uxBGPalette.TabStop = true;
            uxBGPalette.Text = "BG";
            uxBGPalette.UseVisualStyleBackColor = true;
            uxBGPalette.CheckedChanged += OnPaletteCheckedChanged;
            // 
            // uxAddress
            // 
            uxAddress.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            uxAddress.Location = new System.Drawing.Point(2, 19);
            uxAddress.MaxLength = 10;
            uxAddress.Name = "uxAddress";
            uxAddress.Size = new System.Drawing.Size(117, 21);
            uxAddress.TabIndex = 2;
            uxAddress.Text = "0x0";
            uxAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            uxAddress.KeyUp += OnAddressKeyUp;
            uxAddress.Leave += OnAddressLeave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(2, 1);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(49, 15);
            label1.TabIndex = 3;
            label1.Text = "Address";
            // 
            // uxImageWidth
            // 
            uxImageWidth.Location = new System.Drawing.Point(2, 116);
            uxImageWidth.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            uxImageWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            uxImageWidth.Name = "uxImageWidth";
            uxImageWidth.Size = new System.Drawing.Size(117, 23);
            uxImageWidth.TabIndex = 4;
            uxImageWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            uxImageWidth.Value = new decimal(new int[] { 4, 0, 0, 0 });
            uxImageWidth.ValueChanged += OnImageWidthValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(2, 98);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(84, 15);
            label2.TabIndex = 5;
            label2.Text = "Width (in tiles)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(2, 184);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(62, 15);
            label3.TabIndex = 6;
            label3.Text = "Back color";
            // 
            // uxBackColor
            // 
            uxBackColor.BackColor = System.Drawing.Color.Black;
            uxBackColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            uxBackColor.Cursor = System.Windows.Forms.Cursors.Cross;
            uxBackColor.Location = new System.Drawing.Point(2, 202);
            uxBackColor.Name = "uxBackColor";
            uxBackColor.Size = new System.Drawing.Size(117, 23);
            uxBackColor.TabIndex = 7;
            uxBackColor.TabStop = false;
            uxBackColor.MouseClick += OnBackColorMouseClick;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(uxResultsPrio);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(uxScanNow);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(uxCurrentResult);
            groupBox3.Controls.Add(uxResultsFound);
            groupBox3.Controls.Add(label6);
            groupBox3.Location = new System.Drawing.Point(125, 1);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(148, 156);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "Bitmap scan";
            // 
            // uxResultsPrio
            // 
            uxResultsPrio.AutoSize = true;
            uxResultsPrio.Location = new System.Drawing.Point(56, 82);
            uxResultsPrio.Name = "uxResultsPrio";
            uxResultsPrio.Size = new System.Drawing.Size(13, 15);
            uxResultsPrio.TabIndex = 14;
            uxResultsPrio.Text = "0";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(2, 82);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(48, 15);
            label5.TabIndex = 13;
            label5.Text = "Priority:";
            // 
            // uxScanNow
            // 
            uxScanNow.Location = new System.Drawing.Point(15, 20);
            uxScanNow.Name = "uxScanNow";
            uxScanNow.Size = new System.Drawing.Size(118, 23);
            uxScanNow.TabIndex = 12;
            uxScanNow.Text = "Scan now";
            uxScanNow.UseVisualStyleBackColor = true;
            uxScanNow.Click += OnScanNowClick;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(2, 103);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(47, 15);
            label7.TabIndex = 11;
            label7.Text = "Current";
            // 
            // uxCurrentResult
            // 
            uxCurrentResult.Location = new System.Drawing.Point(6, 121);
            uxCurrentResult.Name = "uxCurrentResult";
            uxCurrentResult.Size = new System.Drawing.Size(136, 23);
            uxCurrentResult.TabIndex = 10;
            uxCurrentResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            uxCurrentResult.ValueChanged += OnCurrentResultValueChanged;
            // 
            // uxResultsFound
            // 
            uxResultsFound.AutoSize = true;
            uxResultsFound.Location = new System.Drawing.Point(56, 64);
            uxResultsFound.Name = "uxResultsFound";
            uxResultsFound.Size = new System.Drawing.Size(13, 15);
            uxResultsFound.TabIndex = 1;
            uxResultsFound.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(6, 64);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(44, 15);
            label6.TabIndex = 0;
            label6.Text = "Found:";
            // 
            // uxText
            // 
            uxText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            uxText.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            uxText.Location = new System.Drawing.Point(12, 274);
            uxText.MaxLength = 2147000000;
            uxText.Multiline = true;
            uxText.Name = "uxText";
            uxText.ReadOnly = true;
            uxText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            uxText.Size = new System.Drawing.Size(533, 91);
            uxText.TabIndex = 11;
            // 
            // uxTextFormat
            // 
            uxTextFormat.AutoSize = true;
            uxTextFormat.Checked = true;
            uxTextFormat.CheckState = System.Windows.Forms.CheckState.Checked;
            uxTextFormat.Location = new System.Drawing.Point(173, 238);
            uxTextFormat.Name = "uxTextFormat";
            uxTextFormat.Size = new System.Drawing.Size(100, 19);
            uxTextFormat.TabIndex = 12;
            uxTextFormat.Text = "Show as bytes";
            uxTextFormat.UseVisualStyleBackColor = true;
            uxTextFormat.CheckedChanged += OnTextFormatCheckedChanged;
            // 
            // uxTileSize
            // 
            uxTileSize.Location = new System.Drawing.Point(2, 160);
            uxTileSize.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            uxTileSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            uxTileSize.Name = "uxTileSize";
            uxTileSize.Size = new System.Drawing.Size(117, 23);
            uxTileSize.TabIndex = 13;
            uxTileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            uxTileSize.Value = new decimal(new int[] { 8, 0, 0, 0 });
            uxTileSize.ValueChanged += OnTileSizeValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(2, 142);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(47, 15);
            label4.TabIndex = 14;
            label4.Text = "Tile size";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(2, 242);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(30, 15);
            label8.TabIndex = 15;
            label8.Text = "Size:";
            // 
            // uxSize
            // 
            uxSize.AutoSize = true;
            uxSize.Location = new System.Drawing.Point(38, 242);
            uxSize.Name = "uxSize";
            uxSize.Size = new System.Drawing.Size(13, 15);
            uxSize.TabIndex = 16;
            uxSize.Text = "0";
            // 
            // uxTileIndex
            // 
            uxTileIndex.Enabled = false;
            uxTileIndex.Location = new System.Drawing.Point(147, 160);
            uxTileIndex.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            uxTileIndex.Name = "uxTileIndex";
            uxTileIndex.Size = new System.Drawing.Size(120, 23);
            uxTileIndex.TabIndex = 17;
            uxTileIndex.ValueChanged += uxTileIndex_ValueChanged;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(216, 203);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(57, 23);
            button1.TabIndex = 18;
            button1.Text = "png";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // uxImage
            // 
            uxImage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            uxImage.BackColor = System.Drawing.Color.Gray;
            uxImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            uxImage.Location = new System.Drawing.Point(12, 12);
            uxImage.Name = "uxImage";
            uxImage.Size = new System.Drawing.Size(256, 256);
            uxImage.TabIndex = 20;
            // 
            // UxEnableTileIndexOption
            // 
            UxEnableTileIndexOption.AutoSize = true;
            UxEnableTileIndexOption.Location = new System.Drawing.Point(129, 164);
            UxEnableTileIndexOption.Name = "UxEnableTileIndexOption";
            UxEnableTileIndexOption.Size = new System.Drawing.Size(15, 14);
            UxEnableTileIndexOption.TabIndex = 21;
            UxEnableTileIndexOption.UseVisualStyleBackColor = true;
            UxEnableTileIndexOption.CheckedChanged += UxEnableTileIndexOption_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            panel1.Controls.Add(UxEnableTileIndexOption);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(uxTileIndex);
            panel1.Controls.Add(uxSize);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(uxTileSize);
            panel1.Controls.Add(uxTextFormat);
            panel1.Controls.Add(groupBox3);
            panel1.Controls.Add(uxBackColor);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(uxImageWidth);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(uxAddress);
            panel1.Controls.Add(groupBox1);
            panel1.Location = new System.Drawing.Point(272, 11);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(277, 258);
            panel1.TabIndex = 22;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(557, 377);
            Controls.Add(panel1);
            Controls.Add(uxImage);
            Controls.Add(uxText);
            MinimumSize = new System.Drawing.Size(573, 416);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ROM Image Viewer";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)uxImageWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)uxBackColor).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)uxCurrentResult).EndInit();
            ((System.ComponentModel.ISupportInitialize)uxTileSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)uxTileIndex).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton uxOBJPalette;
        private System.Windows.Forms.RadioButton uxBGPalette;
        private System.Windows.Forms.TextBox uxAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown uxImageWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox uxBackColor;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown uxCurrentResult;
        private System.Windows.Forms.Label uxResultsFound;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox uxText;
        private System.Windows.Forms.CheckBox uxTextFormat;
        private System.Windows.Forms.NumericUpDown uxTileSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button uxScanNow;
        private System.Windows.Forms.Label uxResultsPrio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label uxSize;
        private System.Windows.Forms.NumericUpDown uxTileIndex;
        private System.Windows.Forms.Button button1;
        private DoubleBufferedPanel uxImage;
        private System.Windows.Forms.CheckBox UxEnableTileIndexOption;
        private System.Windows.Forms.Panel panel1;
    }
}

