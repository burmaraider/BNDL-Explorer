using BNDL_Explorer.Classes;
using System;
using System.Collections.Generic;
using System.IO;

namespace BNDL_Explorer
{
    class BNDLFile
    {
        public static int bndlHeaderLength = 24;
        public static int lvbndlHeaderLength = 16;
        public static byte[] TEXR = new byte[] { 0x54, 0x45, 0x58, 0x52, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00 };

        public string marker;
        public int version, tableSize, unk1, filesToSkip, fileCount;

        public List<FileData> bndleFileData = null;
        public string bndlFilename;

        private readonly byte[] tempBytes = new byte[256];

        private static byte[] CreateTable(string[] files, ref List<int> offsets)
        {  
            int fullSize = 0;
            offsets = new List<int>(files.Length);

            foreach (string name in files)
            {
                offsets.Add(fullSize);

                int lenPlusEnd = name.Length + 1;
                int padding = lenPlusEnd % 4;
                if (padding > 0)
                    padding = 4 - padding;

                fullSize += lenPlusEnd + padding;
            }

            byte[] result = new byte[fullSize];
            for (int i = 0; i < files.Length; i++)
                System.Text.Encoding.ASCII.GetBytes(files[i]).CopyTo(result, offsets[i]);

            return result;
        }

        public static void WriteBNDL(Stream stream, string[] files, string[] fullnameFiles, string[] filesToSkip)
        {
            List<int> tableOffsetsSkipped = null;
            List<int> tableOffsets = null;
           
            byte[] tableBytesSkipped = CreateTable(filesToSkip, ref tableOffsetsSkipped);
            byte[] tableBytes = CreateTable(files, ref tableOffsets);
           
            byte[] headerBytes = new byte[bndlHeaderLength];

            System.Text.Encoding.ASCII.GetBytes("BNDL").CopyTo(headerBytes, 0);
            BitConverter.GetBytes(3).CopyTo(headerBytes, 4);
            BitConverter.GetBytes(tableBytesSkipped.Length + tableBytes.Length).CopyTo(headerBytes, 8);
            BitConverter.GetBytes(0).CopyTo(headerBytes, 12);
            BitConverter.GetBytes(filesToSkip.Length).CopyTo(headerBytes, 16);
            BitConverter.GetBytes(files.Length).CopyTo(headerBytes, 20);

            stream.Write(headerBytes, 0, bndlHeaderLength);
            stream.Write(tableBytesSkipped, 0, tableBytesSkipped.Length);
            stream.Write(tableBytes, 0, tableBytes.Length);

            for (int i = 0; i < filesToSkip.Length; i++)
                stream.Write(BitConverter.GetBytes(tableOffsetsSkipped[i]), 0, 4);

            byte[] twoInts = new byte[8];
            for (int i = 0; i < files.Length; i++)
            {
                FileStream fileReader = new FileStream(fullnameFiles[i], FileMode.Open, FileAccess.Read);

                BitConverter.GetBytes(tableBytesSkipped.Length + tableOffsets[i]).CopyTo(twoInts, 0);
                BitConverter.GetBytes((uint)fileReader.Length).CopyTo(twoInts, 4);
                stream.Write(twoInts, 0, 8);
                fileReader.CopyTo(stream);

                fileReader.Close();
            }

        }

        public static void WriteLVBNDL(Stream stream, string[] files, string[] fullnameFiles)
        {
            List<int> tableOffsets = null;
            byte[] tableBytes = CreateTable(files, ref tableOffsets);
            byte[] headerBytes = new byte[lvbndlHeaderLength];

            System.Text.Encoding.ASCII.GetBytes("LVRS").CopyTo(headerBytes, 0);
            BitConverter.GetBytes(1).CopyTo(headerBytes, 4);
            BitConverter.GetBytes(files.Length).CopyTo(headerBytes, 8);
            BitConverter.GetBytes(tableBytes.Length).CopyTo(headerBytes, 12);

            stream.Write(headerBytes, 0, lvbndlHeaderLength);
            stream.Write(tableBytes, 0, tableBytes.Length);

            foreach (string name in fullnameFiles)
            {
                FileStream fileReader = new FileStream(name, FileMode.Open, FileAccess.Read);

                stream.Write(BitConverter.GetBytes((uint)fileReader.Length), 0, 4);
                fileReader.CopyTo(stream);

                fileReader.Close();
            }
        }

        private int ReadInt(Stream stream)
        {
            stream.Read(tempBytes, 0, 4);
            return BitConverter.ToInt32(tempBytes, 0);
        }

        private uint ReadUInt(Stream stream)
        {
            stream.Read(tempBytes, 0, 4);
            return BitConverter.ToUInt32(tempBytes, 0);
        }

        private List<int[]> IndexTable(byte[] table)
        {
            List<int[]> result = new List<int[]>(fileCount);

            int i = 0;
            int nextStringStart = 0;

            while (true)
            {
                if (i == table.Length)
                    break;

                if (table[i] == 0)
                {
                    result.Add(new int[] { nextStringStart, i - nextStringStart });
                    i++;

                    int padding = (nextStringStart + i) % 4;
                    if (padding > 0)
                    {
                        padding = 4 - padding;
                        i = nextStringStart = i + padding;
                    }
                    else
                    {
                        nextStringStart = i;
                    }
                }
                else
                {
                    i++;
                }
            }

            return result;
        }

        private string GetStringFromTable(byte[] table, int offset)
        {
            int i = offset;
            while (true)
            {
                if (table[i] == 0) break;
                i++;
            }
            return System.Text.Encoding.ASCII.GetString(table, offset, i - offset);
        }

        private string GetStringFromTable(byte[] table, int[] indexPair)
        {
            return System.Text.Encoding.ASCII.GetString(table, indexPair[0], indexPair[1]);
        }

        public void ReadBNDL(string filePath)
        {
            FileStream fileReader;
            bndlFilename = filePath;

            fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] headerBytes = new byte[bndlHeaderLength];
            fileReader.Read(headerBytes, 0, bndlHeaderLength);

            marker = System.Text.Encoding.ASCII.GetString(headerBytes, 0, 4);
            version = BitConverter.ToInt32(headerBytes, 4);
            tableSize = BitConverter.ToInt32(headerBytes, 8);
            unk1 = BitConverter.ToInt32(headerBytes, 12);
            filesToSkip = BitConverter.ToInt32(headerBytes, 16);
            fileCount = BitConverter.ToInt32(headerBytes, 20);

            bndleFileData = new List<FileData>(fileCount + filesToSkip);

            byte[] tableBytes = new byte[tableSize];
            fileReader.Read(tableBytes, 0, tableSize);

            for (int i = 0; i < filesToSkip; i++)
            {
                FileData item = new FileData(true)
                {
                    stringOffset = ReadInt(fileReader)
                };
                item.SetName(GetStringFromTable(tableBytes, item.stringOffset));

                bndleFileData.Add(item);
            }

            byte[] twoInts = new byte[8];

            for (int i = 0; i < fileCount; i++)
            {
                fileReader.Read(twoInts, 0, 8);

                FileData item = new FileData(false)
                {
                    stringOffset = BitConverter.ToInt32(twoInts, 0),
                    chunkSizeReported = BitConverter.ToUInt32(twoInts, 4),                  
                    offset = fileReader.Position
                };
                item.chunkSize = item.chunkSizeReported;
                item.SetName(GetStringFromTable(tableBytes, item.stringOffset));

                if (i == fileCount - 1)
                {
                    if (fileReader.Position + item.chunkSizeReported > fileReader.Length)
                        item.chunkSize = (uint)(fileReader.Length - fileReader.Position);
                }

                bndleFileData.Add(item);
                fileReader.Position += item.chunkSize;
            }

            fileReader.Close();
        }

        public void ReadLVBNDL(string filePath)
        {
            FileStream fileReader;
            bndlFilename = filePath;

            fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] headerBytes = new byte[lvbndlHeaderLength];
            fileReader.Read(headerBytes, 0, lvbndlHeaderLength);

            marker = System.Text.Encoding.ASCII.GetString(headerBytes, 0, 4);
            version = BitConverter.ToInt32(headerBytes, 4);
            fileCount = BitConverter.ToInt32(headerBytes, 8);
            tableSize = BitConverter.ToInt32(headerBytes, 12);

            bndleFileData = new List<FileData>(fileCount);

            byte[] tableBytes = new byte[tableSize];
            fileReader.Read(tableBytes, 0, tableSize);

            List<int[]> tableIndices = IndexTable(tableBytes);

            for (int i = 0; i < fileCount; i++)
            {
                uint currChunkSize = ReadUInt(fileReader);

                FileData item = new FileData(false)
                {
                    offset = fileReader.Position,
                    chunkSize = currChunkSize
                };
                item.SetName(GetStringFromTable(tableBytes, tableIndices[i]));

                bndleFileData.Add(item);
                fileReader.Position += item.chunkSize;
            }

            fileReader.Close();
        }

        public void ExtractTextureToStream(Stream fileWriter, FileData item)
        {
            ExtractFileToStream(fileWriter, item, TEXR.Length);
        }

        public void ExtractFileToStream(Stream fileWriter, FileData item, int skipBytes = 0)
        {
            byte[] buffer = ExtractFileToArray(item, skipBytes);
            fileWriter.Write(buffer, 0, buffer.Length);
        }

        public byte[] ExtractTextureToArray(FileData item)
        {
            return ExtractFileToArray(item, TEXR.Length);
        }

        public byte[] ExtractFileToArray(FileData item, int skipBytes = 0)
        {
            FileStream bndlReader = new FileStream(bndlFilename, FileMode.Open, FileAccess.Read)
            {
                Position = item.offset + skipBytes
            };
            int size = (int)item.chunkSize - skipBytes;
            byte[] buffer = new byte[size];
            bndlReader.Read(buffer, 0, size);
            bndlReader.Close();

            return buffer;
        }

        public void ExtractFileToStreamBuffered(Stream fileWriter, FileData item, int bufferSize)
        {
            FileStream bndlReader = new FileStream(bndlFilename, FileMode.Open, FileAccess.Read)
            {
                Position = item.offset
            };

            byte[] buffer = new byte[bufferSize];
            int counter = (int)item.chunkSize / bufferSize;
            int lastBuffer = (int)item.chunkSize % bufferSize;

            for (int i = 0; i < counter; i++)
            {
                bndlReader.Read(buffer, 0, bufferSize);
                fileWriter.Write(buffer, 0, buffer.Length);
            }

            if (lastBuffer > 0)
            {
                bndlReader.Read(buffer, 0, lastBuffer);
                fileWriter.Write(buffer, 0, lastBuffer);
            }
            
            bndlReader.Close();
        }
    }
}
