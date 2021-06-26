namespace AgvcService.System.Upload
{
    /// <summary>
    /// 目录条目
    /// </summary>
    public class FolderItem
    {
        public string FileName;
        public string FileType;
        public int Size;
        public int Number;
        public FolderItem(string filename, string fileType, int size, int number)
        {
            this.FileName = filename;
            this.FileType = fileType;
            this.Size = size;
            this.Number = number;
        }
    }
}