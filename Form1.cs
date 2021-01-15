using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BNDL_Explorer.Forms;
using Pfim;


namespace BNDL_Explorer
{
    public struct FileData
    {
        public FileData(string name)
        {
            fileName = name;
            fileOffset = 0;
            fileChunkSize = 0;
        }
        public FileData(string name, Int32 offset, Int32 chunkSize)
        {
            fileName = name;
            fileOffset = offset;
            fileChunkSize = chunkSize;
        }
        public string fileName;
        public Int32 fileOffset;
        public Int32 fileChunkSize;
    }


    public partial class Form1 : Form
    {

        public ApplicationOptions opt = new ApplicationOptions();
        BNDLFile BNDLParser;


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileD = new OpenFileDialog(); //create object
            fileD.Filter = "FEAR 2 bndl|*.bndl|FEAR 2 lvbndl|*.lvbndl"; //define filter
            //fileD.ShowDialog(); //show dialog
            if (fileD.ShowDialog() != DialogResult.Cancel)
            {
                //clear all data
                treeView1.Nodes.Clear();

                //Clear everything if we need to
                if (BNDLParser != null)
                    BNDLParser.bndleFileData.Clear();

                //Parse this shit

                //Make a new object if we need to.
                if (BNDLParser == null)
                    BNDLParser = new BNDLFile();

                //dataholder for the ReadBNDL
                List<string[]> bndlTOC = new List<string[]>();

                //See what file type we are loading.
                if (fileD.FileName.Contains(".bndl"))
                    bndlTOC = BNDLParser.ReadBNDL(fileD.FileName);

                if (fileD.FileName.Contains(".lvbndl"))
                    BNDLParser.ReadLVBNDL(fileD.FileName);

                    AddTreeViewItem(bndlTOC);

                fileNameLabel.Text = String.Format("Filename: {0}",fileD.SafeFileName);
                fileNameLabel.Visible = true;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string tempLabelString = "";

            var value = BNDLParser.bndleFileData.Find(item => item.fileName == e.Node.Text.ToString());

            double fileKb = 0.0f;

            if(value.fileChunkSize < 1024)
            {
              //  fileKb = Math.Round((double)(value.fileChunkSize / 1024), 4);
                fileKb = (double)Decimal.Round(value.fileChunkSize / 1024);
                tempLabelString = string.Format("FileName: {0} | File Offset: {1} | File Size: {2} bytes ({3}KB)", value.fileName, value.fileOffset, value.fileChunkSize, fileKb.ToString());

            }
            else
            {
                fileKb = Math.Round((double)(value.fileChunkSize / 1024), 1);
                tempLabelString = string.Format("FileName: {0} | File Offset: {1} | File Size: {2} bytes ({3}KB)", value.fileName, value.fileOffset, value.fileChunkSize, String.Format("{0:#,##0}", fileKb));

            }


            fileInfoLabel.Text = tempLabelString;

            //Is this is .tex?
            if (value.fileName != null)
            {
                if (value.fileName.Contains(".tex"))
                {
                    //Setup an array to hold our data (account for bullshit TEXR header on .tex/.dds)
                    byte[] tempImage = new byte[value.fileChunkSize - 12];


                    //Open the file again
                    FileStream bndlReader;
                    bndlReader = new FileStream(BNDLParser.fileOpened, FileMode.Open, FileAccess.Read);
                    //Set our offset (account for bullshit TEXR header on .tex/.dds)
                    bndlReader.Position = value.fileOffset + 12;
                    //read the data!
                    int n = bndlReader.Read(tempImage, 0, value.fileChunkSize - 12);

                    //Make an image stream since we don't need to save the image
                    Stream imageStream = new MemoryStream(tempImage);

                    //SHOW IT
                    using (var image = Pfim.Pfim.FromStream(imageStream))
                    {
                        PixelFormat format;

                        switch (image.Format)
                        {
                            case Pfim.ImageFormat.Rgba32:
                                format = PixelFormat.Format32bppArgb;
                                break;
                            default:
                                //if no alpha, then assume 24bit
                                format = PixelFormat.Format24bppRgb;
                                break;
                        }


                        var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                        try
                        {
                            var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                            var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                            
                            pictureBox1.Image = bitmap;
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            //bitmap.Save(Path.ChangeExtension(@"converted", ".png"), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        finally
                        {
                            string[] temp = image.GetType().ToString().Split('.');
                            labelTexFormat.Text = String.Format("{0} ({1})", temp[1], image.Format.ToString());

                            labelTexName.Text = value.fileName;
                            labelTexSize.Text = String.Format("{0} x {1}", image.Width.ToString(), image.Height.ToString());
                            
                            handle.Free();
                        }
                    }

                    //Setup our GUI and shit
                    extractTextureButton.Visible = true;
                    button1.Visible = true;
                    labelTexSize.Visible = true;
                    labelTexName.Visible = true;
                    labelTexFormat.Visible = true;
                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    pictureBox1.Visible = true;
                    pleaseSelect.Visible = false;

                    bndlReader.Close();
                }
            }
            else
            {
                //Setup our GUI and shit
                labelTexSize.Visible = false;
                labelTexName.Visible = false;
                labelTexFormat.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                extractTextureButton.Visible = false;
                button1.Visible = false;
                pictureBox1.Visible = false;
                pleaseSelect.Visible = true;
            }

        }

        


        public Form1()
        {
            InitializeComponent();
        }

        public void AddToTree(string[] item)
        {
            TreeNodeCollection nodes = treeView1.Nodes;

            for (int i = 0; i < item.Length; i++)
            {
                if (nodes.Find(item[i], false).Length > 0)
                    nodes = nodes.Find(item[i], false)[0].Nodes;
                else
                    nodes = nodes.Add(item[i], item[i]).Nodes;
            }
        }

        public void AddTreeViewItem(List<string[]> items)
        {
            string[][] item = items.ToArray();

            for (int i = 0; i < item.Length; i++)
                AddToTree(item[i]);

            CallRecursive();
        }

        private void SetRecursive(TreeNode treeNode)
        {
            //do something with each node
            if (treeNode.Text.Contains(".tex"))
            {
                treeNode.SelectedImageIndex = 1;
                treeNode.ImageIndex = 1;
            }
            if (treeNode.Text.Contains(".anim"))
            {
                treeNode.SelectedImageIndex = 2;
                treeNode.ImageIndex = 2;
            }
            if (treeNode.Text.Contains(".snd"))
            {
                treeNode.SelectedImageIndex = 1;
                treeNode.ImageIndex = 1;
            }
            if (treeNode.Text.Contains(".mesh"))
            {
                treeNode.SelectedImageIndex = 2;
                treeNode.ImageIndex = 2;
            }
            if (treeNode.Text.Contains(".wldmsh"))
            {
                treeNode.SelectedImageIndex = 2;
                treeNode.ImageIndex = 2;
            }
            if (treeNode.Text.Contains(".tex"))
            {
                treeNode.SelectedImageIndex = 1;
                treeNode.ImageIndex = 1;
            }
            if (treeNode.Text.Contains(".inst"))
            {
                treeNode.SelectedImageIndex = 2;
                treeNode.ImageIndex = 2;
            }
            //System.Diagnostics.Debug.WriteLine(treeNode.Text);
            foreach (TreeNode tn in treeNode.Nodes)
            {
                SetRecursive(tn);
            }
        }

        // Call the procedure using the TreeView.
        public void CallRecursive()
        {
            // Access each node recursively.
            TreeNodeCollection nodes = treeView1.Nodes;
            foreach (TreeNode n in nodes)
            {
                SetRecursive(n);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void extractTextureButton_Click(object sender, EventArgs e)
        {
            byte[] temp = BNDLParser.extractTexture(labelTexName.Text);

            if(temp != null)
            {
                SaveFileDialog mySaveFileDialog = new SaveFileDialog();

                mySaveFileDialog.FileName = labelTexName.Text;
                mySaveFileDialog.Filter = "FEAR 2 Texture|*.tex|DDS Texture|*.dds";
                if(mySaveFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    // If the file name is not an empty string open it for saving.
                    if (mySaveFileDialog.FileName != "")
                    {
                        // Saves the Image via a FileStream created by the OpenFile method.
                        System.IO.FileStream fs =
                            (System.IO.FileStream)mySaveFileDialog.OpenFile();

                        switch (mySaveFileDialog.FilterIndex)
                        {
                            case 1:
                                //Do something else for .tex
                                break;
                            case 2:
                                byte[] newArray = new byte[temp.Length - 12];
                                Buffer.BlockCopy(temp, 12, newArray, 0, newArray.Length);

                                Array.Resize(ref temp, newArray.Length);
                                temp = newArray;
                                break;
                        }

                        // Write the data to the file, byte by byte.
                        for (int i = 0; i < temp.Length; i++)
                        {
                            fs.WriteByte(temp[i]);
                        }

                        // Set the stream position to the beginning of the file.
                        fs.Seek(0, SeekOrigin.Begin);

                        // Read and verify the data.
                        for (int i = 0; i < fs.Length; i++)
                        {
                            if (temp[i] != fs.ReadByte())
                            {
                                MessageBox.Show("Error writing data.");
                                return;
                            }
                        }
                        MessageBox.Show("File was successfully saved!");
                        fs.Close();
                    }
                }
            }
            else 
            {
                MessageBox.Show("Error writing data.");
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Options optionWindow = new Options())
            {
                optionWindow.SetParent(this);
                optionWindow.ShowDialog();
                pictureBox1.BackColor = opt.GetTexBackgroundColor();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] imageToOpen = null;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();

            myOpenFileDialog.FileName = labelTexName.Text;
            myOpenFileDialog.Filter = "FEAR 2 Texture|*.tex|DDS Texture|*.dds";
            if (myOpenFileDialog.ShowDialog() != DialogResult.Cancel)
            {
                // If the file name is not an empty string open it for saving.
                if (myOpenFileDialog.FileName != "")
                {
                    // Saves the Image via a FileStream created by the OpenFile method.
                    System.IO.FileStream fs =
                        (System.IO.FileStream)myOpenFileDialog.OpenFile();

                    switch (myOpenFileDialog.FilterIndex)
                    {
                        case 1:
                            //Do something else for .tex
                            byte[] bufferTex = File.ReadAllBytes(myOpenFileDialog.FileName);

                            break;
                        case 2:
                            //.dds texture
                            byte[] buffer = File.ReadAllBytes(myOpenFileDialog.FileName);
                            Array.Resize<byte>(ref imageToOpen, buffer.Length + 12);

                            Buffer.BlockCopy(BNDLParser.TEXR, 0, imageToOpen, 0, BNDLParser.TEXR.Length);
                            Buffer.BlockCopy(buffer, 0, imageToOpen, BNDLParser.TEXR.Length, buffer.Length);

                            if(BNDLParser.ReplaceTexture(labelTexName.Text, imageToOpen))
                            {
                                Debug.WriteLine("woo");
                            }

                            break;
                    }

                }
            }
        }
    }
}
