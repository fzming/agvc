namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(无小数位)
    /// </summary>
    public class JsonCustomDoubleWith0DigitsConvert : JsonCustomDoubleConvert
    {
        public override int Digits => 0;
    }
}