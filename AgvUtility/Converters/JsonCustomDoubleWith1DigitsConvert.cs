namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(保留1位)
    /// </summary>
    public class JsonCustomDoubleWith1DigitsConvert : JsonCustomDoubleConvert
    {
        public override int Digits => 1;
    }
}