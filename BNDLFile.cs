using SixLabors.ImageSharp.Formats.Tga;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNDL_Explorer
{
    class BNDLFile
    {//------------------------------------------------------------
     //-----------       Created with 010 Editor        -----------
     //------         www.sweetscape.com/010editor/          ------
     //
     // File    : C:\Users\jeff_\Pictures\crate_07_d.tex
     // Address : 0 (0x0)
     // Size    : 12 (0xC)
     //------------------------------------------------------------

        public Int32 marker, version, table, unk1, filesToSkip, fileCount;
        public List<FileData> bndleFileData = new List<FileData>();
        public string fileOpened;

        public byte[] TEXR = new byte[] { 0x54, 0x45, 0x58, 0x52, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00 };

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

        public List<string[]> ReadBNDL(string filePath)
        {
            FileStream bndlReader;
            fileOpened = filePath;

            List<string[]> returnData = new List<string[]>();

            bndlReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] temp = new byte[4];

            //Read the HEADER
            bndlReader.Read(temp, 0, 4);
            marker = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            version = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            table = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            unk1 = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            filesToSkip = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            fileCount = BitConverter.ToInt32(temp, 0);

            string tempstring = "";
            byte[] temp2 = new byte[4];

            Int32 offset = 0;
            int t = 0;
            int counter = 0;
            while (bndlReader.Read(temp2, 0, 4) > 0)
            {
                //Have we hit the end of the table of contents?
                if (offset == table)
                {
                    break;
                }
                offset += 4;

                if(filesToSkip > 0)
                    counter = filesToSkip;
                
                tempstring += System.Text.Encoding.ASCII.GetString(temp2);
                //End of data hit
                if (temp2[3] == 0x0)
                {
                    if (t >= filesToSkip)
                    {
                        tempstring = Regex.Replace(tempstring, "\0", String.Empty);
                        string[] item = tempstring.Split('\\');
                    
                        //Add to our file data list
                        FileData tempData = new FileData(item.Last());
                    
                        bndleFileData.Add(tempData);
                        returnData.Add(item);
                    }
                    else
                    {
                        t++;
                    }
                        tempstring = "";
                }
             }

            Int32 chunkSize = 0;

            if(filesToSkip > 0)
                bndlReader.Position += filesToSkip * 4;

            //Loop through each file to get the offsets AND the data length
            for (int i = 0; i < fileCount; i++)
            {
                //FILE
                //DWORD - UNKNOWN
                //CHUNKSIZE

                //Read the next data chunk size
                bndlReader.Read(temp2, 0, 4);

                //Convert to Int32
                chunkSize = BitConverter.ToInt32(temp2, 0);

                //Now we have chunksize, lets update our bndlFileData
                //Make a temporary struct
                FileData tempStruct = new FileData(bndleFileData[i].fileName, (int)bndlReader.Position, chunkSize);

                //Assign the temp struct to our list
                bndleFileData[i] = tempStruct;


                //Move ahead to the next set of data
                bndlReader.Position += chunkSize + 4;
            }
            bndlReader.Close();

            return returnData;
        }

        public List<string[]> ReadLVBNDL(string filePath)
        {
            FileStream bndlReader;
            fileOpened = filePath;

            List<string[]> returnData = new List<string[]>();

            bndlReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] temp = new byte[4];

            //
            //Read the HEADER
            bndlReader.Read(temp, 0, 4);
            marker = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            version = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            fileCount = BitConverter.ToInt32(temp, 0);

            bndlReader.Read(temp, 0, 4);
            table = BitConverter.ToInt32(temp, 0);

            string tempstring = "";
            byte[] temp2 = new byte[4];

            Int32 offset = 0;
            while (bndlReader.Read(temp2, 0, 4) > 0)
            {
                //Have we hit the end of the table of contents?
                if (offset == table)
                {
                    break;
                }
                offset += 4;
                tempstring += System.Text.Encoding.ASCII.GetString(temp2);
                //End of data hit
                if (temp2[3] == 0x0)
                {
                    tempstring = Regex.Replace(tempstring, "\0", String.Empty);
                    string[] item = tempstring.Split('\\');

                    //Add to our file data list
                    FileData tempData = new FileData(item.Last());
                    bndleFileData.Add(tempData);

                    returnData.Add(item);

                    tempstring = "";
                }
            }

            Int32 chunkSize = 0;
            //Step back 4 bytes
            bndlReader.Position -= 4;

            //Loop through each file to get the offsets AND the data length
            for (int i = 0; i < fileCount; i++)
            {
                //FILE
                //CHUNKSIZE

                //Read the next data chunk size
                bndlReader.Read(temp2, 0, 4);

                //Convert to Int32
                chunkSize = BitConverter.ToInt32(temp2, 0);

                //Now we have chunksize, lets update our bndlFileData
                //Make a temporary struct
                FileData tempStruct = new FileData(bndleFileData[i].fileName, (int)bndlReader.Position, chunkSize);

                //Assign the temp struct to our list
                bndleFileData[i] = tempStruct;

                //Move ahead to the next set of data
                bndlReader.Position += chunkSize;
            }
            //Close the file we are done with it.
            bndlReader.Close();
            return returnData;
        }

        public byte[] extractTexture(string fileName)
        {
            //find our index, this is fast enough
            var value = bndleFileData.Find(item => item.fileName == fileName);

            if (value.fileName != null)
            {
                byte[] tempImage = new byte[value.fileChunkSize];
                //Open the file
                FileStream bndlReader;
                bndlReader = new FileStream(fileOpened, FileMode.Open, FileAccess.Read)
                {
                    //Set our offset (account for bullshit TEXR header on .tex/.dds)
                    Position = value.fileOffset
                };
                //read the data!
                bndlReader.Read(tempImage, 0, value.fileChunkSize);
                //Close the file
                bndlReader.Close();
                //Spit it back
                return tempImage;
            }
            return null;
        }

        public bool ReplaceTexture(string fileInBndl, byte[] fileOpened)
        {

            //find our index, this is fast enough
            var value = bndleFileData.Find(item => item.fileName == fileInBndl);

            if (value.fileName != null)
            {
                //Store the filesize offset so that we can update the final size of the file...Then update the rest of the file... weeee
                int fileSizeOffset = value.fileOffset - 4;

                int newSizeOffset = fileOpened.Length - value.fileChunkSize;

                foreach (var temp in bndleFileData)
                {
                    int newSize = temp.fileOffset + newSizeOffset;
                    FileData tempStruct = new FileData(temp.fileName, temp.fileOffset, newSize);
                    
                }

            }

                //Read texture from file
                //If it is a DDS insert the .tex header


                //If it is TEX continue on


                //Default return
                return false;

        }
    }
}
