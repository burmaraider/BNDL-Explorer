using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BNDL_Explorer.Classes;

namespace BNDL_Explorer
{
    using LokiSoundExplorer;
    public partial class MainForm : Form
    {
        private AskForm askForm;
        private BNDLFile BNDLParser;

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = Constants.openDialogBNDLFilter;

            if (openFileDialog.ShowDialog() != DialogResult.Cancel)
                OpenBundle(openFileDialog.FileName);
        }

        private void OpenBundle(string fileName)
        {
            TreeViewMain.Nodes.Clear();

            BNDLParser = new BNDLFile();

            if (Path.GetExtension(fileName) == Constants.extensions[Constants.SupportedExtensions.BNDL])
                BNDLParser.ReadBNDL(fileName);
            else if (Path.GetExtension(fileName) == Constants.extensions[Constants.SupportedExtensions.LVBNDL])
                BNDLParser.ReadLVBNDL(fileName);

            AddTreeViewItem(BNDLParser.bndleFileData);
            TreeViewMain.Sort();

            statusStripMain.Items.Clear();
            AddStatusItem(string.Format("Filename: {0}", Path.GetFileName(fileName)));
            AddStatusItem(string.Format("Marker: {0}", BNDLParser.marker));
            AddStatusItem(string.Format("Version: {0}", BNDLParser.version));
            AddStatusItem(string.Format("Table Size: {0}", BNDLParser.tableSize));
            AddStatusItem(string.Format("Unknown: {0}", BNDLParser.unk1));
            AddStatusItem(string.Format("Files to Skip: {0}", BNDLParser.filesToSkip));
            AddStatusItem(string.Format("File Count: {0}", BNDLParser.fileCount));

            extractAllFilesToolStripMenuItem.Enabled = true;
        }

        private void AddStatusItem(string text)
        {
            ToolStripStatusLabel item = new ToolStripStatusLabel(text)
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                BorderSides = ToolStripStatusLabelBorderSides.Right
            };

            statusStripMain.Items.Add(item);
        }

        private void TreeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileData item = (FileData)e.Node.Tag;

            if (item != null)
            {
                FileInfoVisibility(true);

                textBoxFileName.Text = item.nameOnly;
                textBoxFileSkipped.Text = item.skipped.ToString();
                textBoxFileStringOffset.Text = item.stringOffset.ToString();
                textBoxFileOffset.Text = item.offset.ToString();
                textBoxFileSize.Text = item.chunkSize == item.chunkSizeReported ? item.chunkSize.ToString() : string.Format("{0} ({1})", item.chunkSizeReported, item.chunkSize);

                if (item.extension == Constants.extensions[Constants.SupportedExtensions.TEX])
                {
                    MemoryStream imageStream = new MemoryStream(BNDLParser.ExtractTextureToArray(item));

                    try
                    {
                        using (Pfim.IImage image = Pfim.Pfim.FromStream(imageStream))
                        {
                            PixelFormat format = (image.Format == Pfim.ImageFormat.Rgba32) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;

                            GCHandle handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            try
                            {
                                IntPtr data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);

                                pictureBoxDDS.Image = new Bitmap(image.Width, image.Height, image.Stride, format, data); ;
                                pictureBoxDDS.SizeMode =
                                    (image.Width > pictureBoxDDS.Width || image.Height > pictureBoxDDS.Height) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
                            }
                            finally
                            {
                                textBoxTextureFormat.Text = String.Format("{0} {1} ({2} x {3})",
                                    image.GetType().ToString().Split('.')[1],
                                    image.Format.ToString(),
                                    image.Width, image.Height);

                                handle.Free();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        pictureBoxDDS.Visible = false;
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    TextureInfoVisibility(true);
                }
                else if(item.extension == Constants.extensions[Constants.SupportedExtensions.SND])
                {
                    openSound.Enabled = true;
                }
                else
                {
                    TextureInfoVisibility(false);
                    openSound.Enabled = false;
                }
            }
            else
            {
                FileInfoVisibility(false);
                TextureInfoVisibility(false);
                openSound.Enabled = false;
            }

        }

        private void FileInfoVisibility(bool visible)
        {
            if (!visible)
            {
                textBoxFileName.Clear();
                textBoxFileSkipped.Clear();
                textBoxFileStringOffset.Clear();
                textBoxFileOffset.Clear();
                textBoxFileSize.Clear();
            }

            buttonExtractFile.Enabled = visible;
        }
        private void TextureInfoVisibility(bool visible)
        {
            textBoxTextureFormat.Visible = visible;
            extractTextureButton.Visible = visible;
            //replaceTextureButton.Visible = visible;
            pictureBoxDDS.Visible = visible;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void AddToTree(FileData item)
        {
            TreeNodeCollection nodes = TreeViewMain.Nodes;
            string key = "";

            for (int i = 0; i < item.nameAndDirs.Length; i++)
            {
                key = key + "\\" + item.nameAndDirs[i];
                if (nodes.Find(key, false).Length > 0)
                {
                    nodes = nodes.Find(key, false)[0].Nodes;
                }
                else
                {
                    TreeNode newNode = nodes.Add(key, item.nameAndDirs[i]);
                    newNode.Tag = (i == item.nameAndDirs.Length - 1) ? item : null;
                    nodes = newNode.Nodes;
                }
            }
        }

        private void AddTreeViewItem(List<FileData> items)
        {
            foreach (FileData item in items)
                AddToTree(item);

            CallRecursive();
        }

        private void SetRecursive(TreeNode treeNode)
        {
            treeNode.SelectedImageIndex = treeNode.ImageIndex = (int)Constants.ImageListTypes.FOLDER;

            if (treeNode.Tag != null)
            {
                FileData data = (FileData)treeNode.Tag;

                if (data.extension == Constants.extensions[Constants.SupportedExtensions.TEX])
                    treeNode.SelectedImageIndex = treeNode.ImageIndex = (int)Constants.ImageListTypes.IMAGE;
                else
                    treeNode.SelectedImageIndex = treeNode.ImageIndex = (int)Constants.ImageListTypes.DOCUMENT;
            }

            foreach (TreeNode node in treeNode.Nodes)
                SetRecursive(node);
        }
  
        private void CallRecursive()
        {
            TreeNodeCollection nodes = TreeViewMain.Nodes;
            foreach (TreeNode n in nodes)
                SetRecursive(n);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ExtractTextureButton_Click(object sender, EventArgs e)
        {
            if (TreeViewMain.SelectedNode != null)
            {
                FileData item = (FileData)TreeViewMain.SelectedNode.Tag;

                saveFileDialog.FileName = textBoxFileName.Text;
                saveFileDialog.Filter = Constants.anyDialogDDSFilter;

                if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream fileWriter = (FileStream)saveFileDialog.OpenFile();

                    BNDLParser.ExtractTextureToStream(fileWriter, item);
                    fileWriter.Close();

                    MessageBox.Show(fileWriter.Name, "Texture extracted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string msg = string.Format("BNDL Explorer v{0}", Constants.appVersion);
            MessageBox.Show(msg, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PictureBoxDDS_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialogDDS.ShowDialog() != DialogResult.Cancel)
                pictureBoxDDS.BackColor = colorDialogDDS.Color;
        }

        private void ButtonExtractFile_Click(object sender, EventArgs e)
        {
            if (TreeViewMain.SelectedNode != null)
            {
                FileData item = (FileData)TreeViewMain.SelectedNode.Tag;

                saveFileDialog.FileName = textBoxFileName.Text;
                saveFileDialog.Filter = Constants.anyDialogAnyFilter;

                if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream fileWriter = (FileStream)saveFileDialog.OpenFile();

                    BNDLParser.ExtractFileToStream(fileWriter, item);
                    fileWriter.Close();

                    MessageBox.Show(fileWriter.Name, "File extracted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ExtractAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string currDir = Directory.GetCurrentDirectory();
            string rootDir = BNDLParser.bndlFilename + "_RootFolder";

            Directory.CreateDirectory(rootDir);         
            Directory.SetCurrentDirectory(rootDir); 

            foreach (FileData item in BNDLParser.bndleFileData)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(item.fullName));

                FileStream fileWriter = new FileStream(item.fullName, FileMode.OpenOrCreate, FileAccess.Write);
                BNDLParser.ExtractFileToStreamBuffered(fileWriter, item, Constants.streamBufferLength);
                fileWriter.Close();
            }

            Directory.SetCurrentDirectory(currDir);
            MessageBox.Show("Files extracted", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = pathList[0];

            if (Directory.Exists(path))
            {
                string[] fullnameFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                string[] files = new string[fullnameFiles.Length];
                for (int i = 0; i < files.Length; i++)
                    files[i] = fullnameFiles[i].Substring(path.Length + 1);

                if (askForm == null)
                    askForm = new AskForm();

                int modalResult = askForm.ShowModal("Bundle creation", "What type of bundle you wish to create?", "BNDL", true, "LVBNDL", true, "Abort", true);
                
                if (modalResult == 1)
                {
                    List<string> filesList = new List<string>(files.Length);
                    List<string> fullnameFilesList = new List<string>(fullnameFiles.Length);
                    List<string> filesToSkipList = new List<string>();

                    for (int i = 0; i < files.Length; i++)
                    {
                        if (new FileInfo(fullnameFiles[i]).Length > 0)
                        {
                            filesList.Add(files[i]);
                            fullnameFilesList.Add(fullnameFiles[i]);
                        }
                        else
                        {
                            filesToSkipList.Add(files[i]);
                        }
                    }

                    FileStream fileWriter = new FileStream(path + Constants.extensions[Constants.SupportedExtensions.BNDL], FileMode.OpenOrCreate, FileAccess.Write);
                    BNDLFile.WriteBNDL(fileWriter, filesList.ToArray(), fullnameFilesList.ToArray(), filesToSkipList.ToArray());
                    fileWriter.Close();
                }
                else if (modalResult == 2)
                {
                    FileStream fileWriter = new FileStream(path + Constants.extensions[Constants.SupportedExtensions.LVBNDL], FileMode.OpenOrCreate, FileAccess.Write);
                    BNDLFile.WriteLVBNDL(fileWriter, files, fullnameFiles);
                    fileWriter.Close();
                }
            }
            else if (File.Exists(path))
            {
                OpenBundle(path);
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeViewMain.Nodes.Clear();
            FileInfoVisibility(false);
            TextureInfoVisibility(false);
        }

        private void openSound_Click(object sender, EventArgs e)
        {
            if (TreeViewMain.SelectedNode != null)
            {
                FileData item = (FileData)TreeViewMain.SelectedNode.Tag;

                byte[] buffer = BNDLParser.ExtractFileToArray(item, 0);
                Form1 loki = new Form1(buffer);

                loki.ShowDialog();
                buffer = null;
                GC.Collect();
            }
        }
    }
}
