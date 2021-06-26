namespace AgvcService.System.Upload
{
    public abstract class UploadModel
    {
        protected UploadModel()
        {
            Option = new UploadOption();
        }

        public UploadOption Option { get; set; }
    }
}