using Utility.Extensions;

namespace CoreData
{
    public class UploadOption
    {
        /// <summary>
        ///     初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public UploadOption()
        {
            UpYun = true;
            AllowMaxSize = 2; //最大2MB
            AllowExtensions = new[]
            {
                //images
                "png", "jpg", "jpeg", "gif", "bmp",
                //offices
                "xls", "xlsx", "doc", "docx", "pdf",
                //zips
                "zip", "rar"
            }.JoinToString(",");
        }

        /// <summary>
        ///     是否上传至又拍云
        /// </summary>
        public bool UpYun { get; set; }

        /// <summary>
        ///     限制最大文件大小
        /// </summary>
        public int AllowMaxSize { get; set; }

        /// <summary>
        ///     允许上传的文件格式,以“，”进行分割
        /// </summary>
        public string AllowExtensions { get; set; }

        #region 上传图片自动切图

        /*
         * 1. AllowMaxWidth = AllowMaxHeight时，将使用正方形截图 （不会变形）
         * 2. AllowMaxWidth，AllowMaxHeight只传递一项时：将自动按等比例切图（不会变形）
         * 3. AllowMaxWidth,AllowMaxHeight都传递时，按指定宽高切图（可能会变形）
         */

        /// <summary>
        ///     图片最大宽度 (0 为不限制)
        /// </summary>
        public int AllowMaxWidth { get; set; }

        /// <summary>
        ///     图片最大高度 (0 为不限制)
        /// </summary>
        public int AllowMaxHeight { get; set; }

        #endregion
    }
}