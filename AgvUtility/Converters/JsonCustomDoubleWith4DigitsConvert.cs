namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(保留4位)
    /// </summary>
    public class JsonCustomDoubleWith4DigitsConvert : JsonCustomDoubleConvert
    {
        public override int Digits => 4;
    }
}