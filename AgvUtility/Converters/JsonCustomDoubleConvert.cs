using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utility.Converters
{
    /// <summary>
    /// 自定义数值类型序列化转换器(默认保留3位)
    /// </summary>
    public class JsonCustomDoubleConvert : CustomCreationConverter<double>
    {
        /// <summary>
        /// 序列化后保留小数位数
        /// </summary>
        public virtual int Digits { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        public JsonCustomDoubleConvert():this(3)
        {
           
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="digits">序列化后保留小数位数</param>
        public JsonCustomDoubleConvert(int digits)
        {
            this.Digits = digits;
        }

        /// <summary>
        /// 重载是否可写
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        /// 重载创建方法
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override double Create(Type objectType)
        {
            return 0.0;
        }

        /// <summary>
        /// 重载序列化方法
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null|| value == DBNull.Value)
            {
                writer.WriteNull();
            }
            else
            {
                // var formatter = ((double)value).ToString("N" + Digits);
               writer.WriteValue(Math.Round((double) value, Digits));
            }

        }
    }
}