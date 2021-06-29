namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(保留5位)
    /// </summary>
    public class JsonCustomDoubleWith5DigitsConvert : JsonCustomDoubleConvert
    {
        public override int Digits => 5;
    }
}