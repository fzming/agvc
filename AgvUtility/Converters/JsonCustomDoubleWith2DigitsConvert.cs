namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(保留2位)
    /// </summary>
    public class JsonCustomDoubleWith2DigitsConvert : JsonCustomDoubleConvert
    {
        public override int Digits => 2;
    }
}