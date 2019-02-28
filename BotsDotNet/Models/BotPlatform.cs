using System;
using System.Collections.Generic;
using System.Text;

namespace BotsDotNet
{
    public struct BotPlatform : IConvertible
    {
        public const string Discord = "Discord";
        public const string Palringo = "Palringo";
        public const string PalringoV3 = "PalringoV3";
        public const string WebExTeams = "WebExTeams";

        private string _value;

        public BotPlatform(string value)
        {
            _value = value;
        }

        public static implicit operator string(BotPlatform platform)
        {
            return platform._value;
        }

        public static implicit operator BotPlatform(string value)
        {
            return new BotPlatform(value);
        }

        public static bool operator ==(BotPlatform platform, string value)
        {
            return platform._value == value;
        }

        public static bool operator !=(BotPlatform platform, string value)
        {
            return platform._value != value;
        }

        public static bool operator ==(BotPlatform platform, BotPlatform platform2)
        {
            return platform._value == platform2._value;
        }

        public static bool operator !=(BotPlatform platform, BotPlatform platform2)
        {
            return platform._value != platform2._value;
        }

        public static bool operator ==(string value, BotPlatform platform)
        {
            return platform._value == value;
        }

        public static bool operator !=(string value, BotPlatform platform)
        {
            return platform._value != value;
        }

        public override bool Equals(object obj)
        {
            if (obj is BotPlatform)
                return (BotPlatform)obj == this;

            if (obj is string)
                return (string)obj == _value;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            return _value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_value, typeof(BotPlatform), provider);
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.String;
        }
    }
}
