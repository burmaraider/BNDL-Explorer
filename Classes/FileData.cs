using System.IO;

namespace BNDL_Explorer.Classes
{
    public class FileData
    {
        public FileData(bool skipped)
        {
            this.skipped = skipped;
            offset = 0;
            chunkSize = 0;
            stringOffset = 0;
        }

        public void SetName(string name)
        {
            fullName = name;
            nameAndDirs = name.Split('\\');
            nameOnly = Path.GetFileName(name);
            extension = Path.GetExtension(name);
        }

        public bool skipped;
        public string nameOnly;
        public string fullName;
        public string[] nameAndDirs;
        public string extension;
        public long offset;
        public int stringOffset;
        public uint chunkSize;
        public uint chunkSizeReported;
    }
}
