using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace LokiSoundExplorer
{
    using ExtensionMethods;
    using NAudio.Wave;
    public partial class Form1 : Form
    {
        public byte[] fileBuffer;
        WaveOut _waveOut = new WaveOut();
        LokiSound ls = new LokiSound();

        public Form1()
        {
            InitializeComponent();

            LoadSound();
        }

        public Form1(byte[] b)
        {
            InitializeComponent();
            fileBuffer = b;
            LoadSound();
        }

        private void LoadSound()
        {
            ls = null;
            listView1.Items.Clear();
            //GC.Collect();
            ls = new LokiSound();

            if (ls.ReadSoundFile(fileBuffer))
            {
                int i = 0;
                foreach (var item in ls.unknownTable)
                {
                    ListViewItem lItem = new ListViewItem();

                    lItem.Text = "Sound# " + (i + 1).ToString();

                    if (item.chan.Any())
                        lItem.SubItems.Add(item.chan[0].sample_rate.ToString());
                    else
                        lItem.SubItems.Add("Unknown");

                    lItem.SubItems.Add(item.bit_depth.ToString());

                    string fileSize = "";
                    //calc size
                    if (item.chan.Any())
                        fileSize = ls.waveFiles[i].wavChannels[0].data_length.ToString();
                    else
                        fileSize = "0";

                    fileSize = fileSize.ToSize(MyExtensions.SizeUnits.KB);

                    lItem.SubItems.Add(fileSize + " KB");
                    listView1.Items.Add(lItem);
                    i++;
                }

                detailGroupBox.Enabled = true;
                actionGroupBox.Enabled = true;
            }
            fileBuffer = null;
            GC.Collect();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
            {
                var item = sender as ListView;
                int selectedIndex = listView1.SelectedIndices[0];

                float time = ls.waveFiles[selectedIndex].wavChannels[0].data_length / (ls.waveFiles[selectedIndex].wavChannels[0].sample_rate * ls.waveFiles[selectedIndex].wavChannels[0].channel_count * ls.waveFiles[selectedIndex].wavChannels[0].bit_depth / 8);

                TimeSpan t = TimeSpan.FromSeconds(time);

                lengthLabel.Text = "Length: " + t.ToString(@"mm\:ss");
                bitDepthLabel.Text = "Bit Depth: " + ls.waveFiles[selectedIndex].wavChannels[0].bit_depth.ToString();
                bitRateLabel.Text = "Bitrate: " + ls.waveFiles[selectedIndex].wavChannels[0].sample_rate.ToString();
                fileSizeLabel.Text = "Size: " + listView1.Items[selectedIndex].SubItems[3].Text;

                foreach (Control gb in actionGroupBox.Controls)
                    gb.Enabled = true;

            }
            else
                foreach (Control gb in actionGroupBox.Controls)
                    gb.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].buf), 
                         new WaveFormat((int)ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].sample_rate,
                         ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].bit_depth,
                         ls.waveFiles[listView1.SelectedIndices[0]].wavChannels[0].channel_count));

            VolumeWaveProvider16 volumeWaveProvider = new VolumeWaveProvider16(provider);
            volumeWaveProvider.Volume = volumeSlider1.Volume;

            _waveOut.Init(volumeWaveProvider);
            _waveOut.Play();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ls = null;
            fileBuffer = null;
            listView1.Items.Clear();
            GC.Collect();
            this.Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            extractAll_Click(this, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _waveOut.Stop();
        }

        private void closeStripMenuItem_Click(object sender, EventArgs e)
        {
            ls = null;
            listView1.Items.Clear();
            GC.Collect();

            lengthLabel.Text = "Length: ";
            bitDepthLabel.Text = "Bit Depth:";
            bitRateLabel.Text = "Bitrate: ";
            fileSizeLabel.Text = "Size: ";

            actionGroupBox.Enabled = false;
            foreach (Control item in actionGroupBox.Controls)
                item.Enabled = false;

        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            byte[] buffer = ls.ExtractSound(listView1.SelectedIndices[0]);

            if(buffer != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "WAV file |*.wav";
                saveFileDialog.FileName = "Sound " + (listView1.SelectedIndices[0]+1);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, buffer);
                    buffer = null;
                    GC.Collect();
                }
            }
        }

        private void extractAll_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WAV file |*.wav";
            saveFileDialog.FileName = "Select a path to save all files";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string savePath = Path.GetDirectoryName(saveFileDialog.FileName);

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    byte[] buffer = ls.ExtractSound(i);

                    if (buffer != null)
                        File.WriteAllBytes(savePath + "\\Sound " + (i + 1) + ".wav", buffer);
                    buffer = null;
                    GC.Collect();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ls = null;
            fileBuffer = null;
            listView1.Items.Clear();
            _waveOut.Stop();
            GC.Collect();
        }
    }
}
