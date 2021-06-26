namespace Protocol.Request
{
    public class Response : Base
    {
        /// <summary>
        ///     AGVC 是否同意接受任務，True 表示同意
        /// </summary>
        public bool Agree { get; set; }

        /// <summary>
        ///     日志
        /// </summary>
        public string Log { get; set; }
    }
}