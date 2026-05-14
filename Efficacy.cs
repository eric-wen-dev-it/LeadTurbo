using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LeadTurbo
{
    /// <summary>
    /// MD5 哈希工具。Jiamparts.Core.Efficacy 的精简移植版，
    /// 保留 GetMD5ToGuid / GetMD5ToLong / GetMD5ToByte 实际在用的重载。
    /// </summary>
    public static class Efficacy
    {
        public static Guid GetMD5ToGuid(byte[] data)
        {
            return new Guid(GetMD5ToByte(data));
        }

        public static Guid GetMD5ToGuid(Stream data)
        {
            return new Guid(GetMD5ToByte(data));
        }

        public static Guid GetMD5ToGuid(string data)
        {
            return new Guid(GetMD5ToByte(data));
        }

        /// <summary>
        /// 截取 MD5 哈希的前 8 字节作为 long。用于需要"内容决定主键"
        /// 而 Entity.PrimaryKey 已从 Guid 改为 long 的场景。
        /// </summary>
        public static long GetMD5ToLong(Stream data)
        {
            return BitConverter.ToInt64(GetMD5ToByte(data), 0);
        }

        public static byte[] GetMD5ToByte(Stream data)
        {
            using MD5 md5 = MD5.Create();
            return md5.ComputeHash(data);
        }

        public static byte[] GetMD5ToByte(byte[] data)
        {
            using MD5 md5 = MD5.Create();
            return md5.ComputeHash(data);
        }

        public static byte[] GetMD5ToByte(string data)
        {
            int byteCount = Encoding.Unicode.GetByteCount(data);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(byteCount);
            try
            {
                Encoding.Unicode.GetBytes(data, 0, data.Length, buffer, 0);
                return MD5.HashData(new ReadOnlySpan<byte>(buffer, 0, byteCount));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
