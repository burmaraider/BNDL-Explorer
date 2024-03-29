﻿namespace BNDL_Explorer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TreeViewMain = new System.Windows.Forms.TreeView();
            this.imageListMain = new System.Windows.Forms.ImageList(this.components);
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bundleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxFileInfo = new System.Windows.Forms.GroupBox();
            this.openSound = new System.Windows.Forms.Button();
            this.buttonExtractFile = new System.Windows.Forms.Button();
            this.textBoxFileSkipped = new System.Windows.Forms.TextBox();
            this.labelFileSkipped = new System.Windows.Forms.Label();
            this.textBoxFileSize = new System.Windows.Forms.TextBox();
            this.textBoxFileOffset = new System.Windows.Forms.TextBox();
            this.labelFileSize = new System.Windows.Forms.Label();
            this.textBoxFileStringOffset = new System.Windows.Forms.TextBox();
            this.labelFileOffset = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.labelFileStringOffset = new System.Windows.Forms.Label();
            this.labelFileName = new System.Windows.Forms.Label();
            this.groupBoxTextureInfo = new System.Windows.Forms.GroupBox();
            this.textBoxTextureFormat = new System.Windows.Forms.TextBox();
            this.extractTextureButton = new System.Windows.Forms.Button();
            this.pictureBoxDDS = new System.Windows.Forms.PictureBox();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.colorDialogDDS = new System.Windows.Forms.ColorDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain.SuspendLayout();
            this.groupBoxFileInfo.SuspendLayout();
            this.groupBoxTextureInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDDS)).BeginInit();
            this.SuspendLayout();
            // 
            // TreeViewMain
            // 
            this.TreeViewMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TreeViewMain.HideSelection = false;
            this.TreeViewMain.ImageIndex = 0;
            this.TreeViewMain.ImageList = this.imageListMain;
            this.TreeViewMain.Location = new System.Drawing.Point(10, 26);
            this.TreeViewMain.Name = "TreeViewMain";
            this.TreeViewMain.SelectedImageIndex = 0;
            this.TreeViewMain.Size = new System.Drawing.Size(270, 475);
            this.TreeViewMain.TabIndex = 1;
            this.TreeViewMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewMain_AfterSelect);
            // 
            // imageListMain
            // 
            this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
            this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMain.Images.SetKeyName(0, "folder.png");
            this.imageListMain.Images.SetKeyName(1, "image.png");
            this.imageListMain.Images.SetKeyName(2, "document.png");
            // 
            // menuStripMain
            // 
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.bundleToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStripMain.Size = new System.Drawing.Size(981, 24);
            this.menuStripMain.TabIndex = 2;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem,
            this.asdToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // asdToolStripMenuItem
            // 
            this.asdToolStripMenuItem.Name = "asdToolStripMenuItem";
            this.asdToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.asdToolStripMenuItem.Text = "asd";
            this.asdToolStripMenuItem.Click += new System.EventHandler(this.asdToolStripMenuItem_Click);
            // 
            // bundleToolStripMenuItem
            // 
            this.bundleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractAllFilesToolStripMenuItem});
            this.bundleToolStripMenuItem.Name = "bundleToolStripMenuItem";
            this.bundleToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.bundleToolStripMenuItem.Text = "Bundle";
            // 
            // extractAllFilesToolStripMenuItem
            // 
            this.extractAllFilesToolStripMenuItem.Enabled = false;
            this.extractAllFilesToolStripMenuItem.Name = "extractAllFilesToolStripMenuItem";
            this.extractAllFilesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.extractAllFilesToolStripMenuItem.Text = "Extract All Files";
            this.extractAllFilesToolStripMenuItem.Click += new System.EventHandler(this.ExtractAllFilesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // groupBoxFileInfo
            // 
            this.groupBoxFileInfo.Controls.Add(this.openSound);
            this.groupBoxFileInfo.Controls.Add(this.buttonExtractFile);
            this.groupBoxFileInfo.Controls.Add(this.textBoxFileSkipped);
            this.groupBoxFileInfo.Controls.Add(this.labelFileSkipped);
            this.groupBoxFileInfo.Controls.Add(this.textBoxFileSize);
            this.groupBoxFileInfo.Controls.Add(this.textBoxFileOffset);
            this.groupBoxFileInfo.Controls.Add(this.labelFileSize);
            this.groupBoxFileInfo.Controls.Add(this.textBoxFileStringOffset);
            this.groupBoxFileInfo.Controls.Add(this.labelFileOffset);
            this.groupBoxFileInfo.Controls.Add(this.textBoxFileName);
            this.groupBoxFileInfo.Controls.Add(this.labelFileStringOffset);
            this.groupBoxFileInfo.Controls.Add(this.labelFileName);
            this.groupBoxFileInfo.Location = new System.Drawing.Point(285, 26);
            this.groupBoxFileInfo.Name = "groupBoxFileInfo";
            this.groupBoxFileInfo.Size = new System.Drawing.Size(287, 199);
            this.groupBoxFileInfo.TabIndex = 2;
            this.groupBoxFileInfo.TabStop = false;
            this.groupBoxFileInfo.Text = "File Info";
            // 
            // openSound
            // 
            this.openSound.Enabled = false;
            this.openSound.Location = new System.Drawing.Point(9, 166);
            this.openSound.Name = "openSound";
            this.openSound.Size = new System.Drawing.Size(121, 27);
            this.openSound.TabIndex = 5;
            this.openSound.Text = "Open Sound";
            this.openSound.UseVisualStyleBackColor = true;
            this.openSound.Click += new System.EventHandler(this.openSound_Click);
            // 
            // buttonExtractFile
            // 
            this.buttonExtractFile.Enabled = false;
            this.buttonExtractFile.Location = new System.Drawing.Point(160, 166);
            this.buttonExtractFile.Name = "buttonExtractFile";
            this.buttonExtractFile.Size = new System.Drawing.Size(121, 27);
            this.buttonExtractFile.TabIndex = 4;
            this.buttonExtractFile.Text = "Extract File";
            this.buttonExtractFile.UseVisualStyleBackColor = true;
            this.buttonExtractFile.Click += new System.EventHandler(this.ButtonExtractFile_Click);
            // 
            // textBoxFileSkipped
            // 
            this.textBoxFileSkipped.Location = new System.Drawing.Point(171, 49);
            this.textBoxFileSkipped.Name = "textBoxFileSkipped";
            this.textBoxFileSkipped.ReadOnly = true;
            this.textBoxFileSkipped.Size = new System.Drawing.Size(110, 20);
            this.textBoxFileSkipped.TabIndex = 3;
            // 
            // labelFileSkipped
            // 
            this.labelFileSkipped.AutoSize = true;
            this.labelFileSkipped.Location = new System.Drawing.Point(6, 51);
            this.labelFileSkipped.Name = "labelFileSkipped";
            this.labelFileSkipped.Size = new System.Drawing.Size(65, 13);
            this.labelFileSkipped.TabIndex = 2;
            this.labelFileSkipped.Text = "File Skipped";
            // 
            // textBoxFileSize
            // 
            this.textBoxFileSize.Location = new System.Drawing.Point(171, 98);
            this.textBoxFileSize.Name = "textBoxFileSize";
            this.textBoxFileSize.ReadOnly = true;
            this.textBoxFileSize.Size = new System.Drawing.Size(110, 20);
            this.textBoxFileSize.TabIndex = 1;
            // 
            // textBoxFileOffset
            // 
            this.textBoxFileOffset.Location = new System.Drawing.Point(171, 122);
            this.textBoxFileOffset.Name = "textBoxFileOffset";
            this.textBoxFileOffset.ReadOnly = true;
            this.textBoxFileOffset.Size = new System.Drawing.Size(110, 20);
            this.textBoxFileOffset.TabIndex = 1;
            // 
            // labelFileSize
            // 
            this.labelFileSize.AutoSize = true;
            this.labelFileSize.Location = new System.Drawing.Point(6, 100);
            this.labelFileSize.Name = "labelFileSize";
            this.labelFileSize.Size = new System.Drawing.Size(46, 13);
            this.labelFileSize.TabIndex = 0;
            this.labelFileSize.Text = "File Size";
            // 
            // textBoxFileStringOffset
            // 
            this.textBoxFileStringOffset.Location = new System.Drawing.Point(171, 73);
            this.textBoxFileStringOffset.Name = "textBoxFileStringOffset";
            this.textBoxFileStringOffset.ReadOnly = true;
            this.textBoxFileStringOffset.Size = new System.Drawing.Size(110, 20);
            this.textBoxFileStringOffset.TabIndex = 1;
            // 
            // labelFileOffset
            // 
            this.labelFileOffset.AutoSize = true;
            this.labelFileOffset.Location = new System.Drawing.Point(6, 124);
            this.labelFileOffset.Name = "labelFileOffset";
            this.labelFileOffset.Size = new System.Drawing.Size(54, 13);
            this.labelFileOffset.TabIndex = 0;
            this.labelFileOffset.Text = "File Offset";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(136, 24);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.ReadOnly = true;
            this.textBoxFileName.Size = new System.Drawing.Size(145, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // labelFileStringOffset
            // 
            this.labelFileStringOffset.AutoSize = true;
            this.labelFileStringOffset.Location = new System.Drawing.Point(6, 76);
            this.labelFileStringOffset.Name = "labelFileStringOffset";
            this.labelFileStringOffset.Size = new System.Drawing.Size(84, 13);
            this.labelFileStringOffset.TabIndex = 0;
            this.labelFileStringOffset.Text = "File String Offset";
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(6, 27);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(54, 13);
            this.labelFileName.TabIndex = 0;
            this.labelFileName.Text = "File Name";
            // 
            // groupBoxTextureInfo
            // 
            this.groupBoxTextureInfo.Controls.Add(this.textBoxTextureFormat);
            this.groupBoxTextureInfo.Controls.Add(this.extractTextureButton);
            this.groupBoxTextureInfo.Controls.Add(this.pictureBoxDDS);
            this.groupBoxTextureInfo.Location = new System.Drawing.Point(578, 26);
            this.groupBoxTextureInfo.Name = "groupBoxTextureInfo";
            this.groupBoxTextureInfo.Size = new System.Drawing.Size(397, 475);
            this.groupBoxTextureInfo.TabIndex = 1;
            this.groupBoxTextureInfo.TabStop = false;
            this.groupBoxTextureInfo.Text = "Texture Info";
            // 
            // textBoxTextureFormat
            // 
            this.textBoxTextureFormat.Location = new System.Drawing.Point(6, 445);
            this.textBoxTextureFormat.Name = "textBoxTextureFormat";
            this.textBoxTextureFormat.ReadOnly = true;
            this.textBoxTextureFormat.Size = new System.Drawing.Size(258, 20);
            this.textBoxTextureFormat.TabIndex = 5;
            this.textBoxTextureFormat.Visible = false;
            // 
            // extractTextureButton
            // 
            this.extractTextureButton.Location = new System.Drawing.Point(269, 441);
            this.extractTextureButton.Name = "extractTextureButton";
            this.extractTextureButton.Size = new System.Drawing.Size(121, 27);
            this.extractTextureButton.TabIndex = 3;
            this.extractTextureButton.Text = "Extract Texture";
            this.extractTextureButton.UseVisualStyleBackColor = true;
            this.extractTextureButton.Visible = false;
            this.extractTextureButton.Click += new System.EventHandler(this.ExtractTextureButton_Click);
            // 
            // pictureBoxDDS
            // 
            this.pictureBoxDDS.BackColor = System.Drawing.Color.LightGray;
            this.pictureBoxDDS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxDDS.Location = new System.Drawing.Point(6, 19);
            this.pictureBoxDDS.Name = "pictureBoxDDS";
            this.pictureBoxDDS.Size = new System.Drawing.Size(384, 416);
            this.pictureBoxDDS.TabIndex = 0;
            this.pictureBoxDDS.TabStop = false;
            this.pictureBoxDDS.Visible = false;
            this.pictureBoxDDS.DoubleClick += new System.EventHandler(this.PictureBoxDDS_DoubleClick);
            // 
            // statusStripMain
            // 
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Location = new System.Drawing.Point(0, 508);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(981, 22);
            this.statusStripMain.SizingGrip = false;
            this.statusStripMain.TabIndex = 3;
            // 
            // colorDialogDDS
            // 
            this.colorDialogDDS.Color = System.Drawing.Color.LightGray;
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.windowToolStripMenuItem.Text = "Window";
            this.windowToolStripMenuItem.Click += new System.EventHandler(this.windowToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 530);
            this.Controls.Add(this.groupBoxTextureInfo);
            this.Controls.Add(this.groupBoxFileInfo);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.TreeViewMain);
            this.Controls.Add(this.menuStripMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BNDL Explorer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.groupBoxFileInfo.ResumeLayout(false);
            this.groupBoxFileInfo.PerformLayout();
            this.groupBoxTextureInfo.ResumeLayout(false);
            this.groupBoxTextureInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDDS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView TreeViewMain;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxDDS;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxTextureInfo;
        private System.Windows.Forms.Button extractTextureButton;
        private System.Windows.Forms.ImageList imageListMain;
        private System.Windows.Forms.GroupBox groupBoxFileInfo;
        private System.Windows.Forms.TextBox textBoxFileSize;
        private System.Windows.Forms.TextBox textBoxFileOffset;
        private System.Windows.Forms.Label labelFileSize;
        private System.Windows.Forms.TextBox textBoxFileStringOffset;
        private System.Windows.Forms.Label labelFileOffset;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label labelFileStringOffset;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxTextureFormat;
        private System.Windows.Forms.TextBox textBoxFileSkipped;
        private System.Windows.Forms.Label labelFileSkipped;
        private System.Windows.Forms.ColorDialog colorDialogDDS;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button buttonExtractFile;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem bundleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractAllFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button openSound;
        private System.Windows.Forms.ToolStripMenuItem asdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
    }
}

