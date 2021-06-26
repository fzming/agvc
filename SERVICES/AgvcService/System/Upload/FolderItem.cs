namespace AgvcService.System.Upload
{
    /// <summary>
    ///     目录条目
    /// </summary>
    public class FolderItem
    {
        public string FileName;
        public string FileType;
        public int Number;
        public int Size;

        public FolderItem(string filename, string fileType, int size, int number)
        {
            FileName = filename;
            FileType = fileType;
            Size = size;
            Number = number;
        }
    }
}