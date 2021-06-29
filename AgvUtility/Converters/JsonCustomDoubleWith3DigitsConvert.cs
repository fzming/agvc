namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(保留3位)
    /// </summary>
    public class JsonCustomDoubleWith3DigitsConvert : JsonCustomDoubleConvert
    {
        public override int Digits => 3;
    }
}