using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }
        public static string ToSize(this String val, SizeUnits unit)
        {
            Int64 value = Convert.ToInt64(val);
            //return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0");
        }
    }
}

namespace LokiSoundExplorer
{
    public class MediaPlayer
    {
        System.Media.SoundPlayer soundPlayer;

        MemoryStream memoryStream = new MemoryStream();

        public MediaPlayer()
        {

        }
        public MediaPlayer(byte[] buffer)
        {
            var memoryStream = new MemoryStream(buffer, true);
            soundPlayer = new System.Media.SoundPlayer(memoryStream);
        }

        public void Play()
        {
            soundPlayer.Play();
        }

        public void Play(byte[] buffer)
        {
            soundPlayer.Stream.Seek(0, SeekOrigin.Begin);
            soundPlayer.Stream.Write(buffer, 0, buffer.Length);
            soundPlayer.Play();
        }
    }

    public class WavFileContainer
    {
        public List<LokiSound.WavFile> wavChannels = new List<LokiSound.WavFile>();
    }

    public class LokiSound
    {

        public struct Header
        {
            public uint version;
            public uint sound_count, offset, _offset, data_offset;
            public uint unk;
            public byte[] buf;
        }

        public struct index_table
        {
            public ushort a;
            public ushort b;
            public uint offset;
        }

        public struct channels
        {
            public int sample_rate;
            public short unk, unk2;
            public short extra_size;
            public short reserved_union;
            public uint channel_mask;
            public byte[] sub_format;
        }

        public struct unk
        {
            public int channel_count;
            public short unk1, unk2;
            public int unk3;
            public int bit_depth;
            public List<channels> chan;
        }

        public struct WavFile
        {
            public uint size;
            public uint unk, unk1, unk2, unk3;
            public uint data_length;
            public ushort format_tag, channel_count;
            public uint sample_rate;
            public uint bytes_per_second;
            public short block_align, bit_depth;
            public short extra_size;
            public short reserved_union;
            public uint channel_mask;
            public byte[] sub_format;
            public byte[] buf;
        }

        public List<index_table> iTable = new List<index_table>();
        public List<unk> unknownTable = new List<unk>();
        public List<WavFileContainer> waveFiles = new List<WavFileContainer>();
        public Header header;

        public bool ReadSoundFile(byte[] buffer)
        {
            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryReader ms = new BinaryReader(memoryStream);

            header.version = ms.ReadUInt32();
            header.sound_count = ms.ReadUInt32();
            header.offset = ms.ReadUInt32();
            header._offset = ms.ReadUInt32();
            header.data_offset = ms.ReadUInt32();
            header.buf = ms.ReadBytes(260);


            //read the index tables
            for (int i = 0; i < header.sound_count; i++)
                iTable.Add(new index_table()
                {
                    a = ms.ReadUInt16(),
                    b = ms.ReadUInt16(),
                    offset = ms.ReadUInt32()
                });

            //read the soundinfo table
            for (int i = 0; i < header.sound_count; i++)
            {
                unk tempUnknownTable;

                tempUnknownTable.channel_count = ms.ReadInt32();
                tempUnknownTable.unk1 = ms.ReadInt16();
                tempUnknownTable.unk2 = ms.ReadInt16();
                tempUnknownTable.unk3 = ms.ReadInt32();
                tempUnknownTable.bit_depth = ms.ReadInt32();

                tempUnknownTable.chan = new List<channels>();

                for (int t = 0; t < tempUnknownTable.channel_count; t++)
                {
                    channels tempChannel;

                    tempChannel.sample_rate = ms.ReadInt32();
                    tempChannel.unk = ms.ReadInt16();
                    tempChannel.unk2 = ms.ReadInt16();
                    tempChannel.extra_size = ms.ReadInt16();
                    tempChannel.reserved_union = ms.ReadInt16();
                    tempChannel.channel_mask = ms.ReadUInt32();
                    tempChannel.sub_format = ms.ReadBytes(16);

                    tempUnknownTable.chan.Add(tempChannel);
                }
                unknownTable.Add(tempUnknownTable);
            }

            for (int i = 0; i < header.sound_count; i++)
            {
                WavFileContainer wc = new WavFileContainer();

                for (int t = 0; t < unknownTable[i].channel_count; t++)
                {
                    WavFile w;
                    w.size = ms.ReadUInt32();
                    w.unk = ms.ReadUInt32();
                    w.unk1 = ms.ReadUInt32();
                    w.unk2 = ms.ReadUInt32();
                    w.unk3 = ms.ReadUInt32();
                    w.data_length = ms.ReadUInt32();
                    w.format_tag = ms.ReadUInt16();
                    w.channel_count = ms.ReadUInt16();
                    w.sample_rate = ms.ReadUInt32();
                    w.bytes_per_second = ms.ReadUInt32();
                    w.block_align = ms.ReadInt16();
                    w.bit_depth = ms.ReadInt16();
                    w.extra_size = ms.ReadInt16();
                    w.reserved_union = ms.ReadInt16();
                    w.channel_mask = ms.ReadUInt32();
                    w.sub_format = ms.ReadBytes(16);
                    w.buf = ms.ReadBytes((int)w.data_length);

                    wc.wavChannels.Add(w);
                }

                waveFiles.Add(wc);
            }
            ms.Close();
            return true; // return true for now i guess..
        }

        public byte[] ExtractSound(int index)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(BitConverter.GetBytes(waveFiles[index].wavChannels[0].data_length + 36));
            bw.Write(Encoding.ASCII.GetBytes("WAVE"));
            bw.Write(Encoding.ASCII.GetBytes("fmt "));
            bw.Write(BitConverter.GetBytes((int)16));
            bw.Write(BitConverter.GetBytes((short)1));
            bw.Write(BitConverter.GetBytes((short)waveFiles[index].wavChannels[0].channel_count));
            bw.Write(BitConverter.GetBytes(waveFiles[index].wavChannels[0].sample_rate));
            bw.Write(BitConverter.GetBytes((int)(waveFiles[index].wavChannels[0].sample_rate * waveFiles[index].wavChannels[0].bit_depth / 8)));
            bw.Write(BitConverter.GetBytes(waveFiles[index].wavChannels[0].block_align));
            bw.Write(BitConverter.GetBytes(waveFiles[index].wavChannels[0].bit_depth));
            bw.Write(Encoding.ASCII.GetBytes("data"));
            bw.Write(BitConverter.GetBytes(waveFiles[index].wavChannels[0].data_length));
            bw.Write(waveFiles[index].wavChannels[0].buf);

            return ms.ToArray();
        }
    }
}
