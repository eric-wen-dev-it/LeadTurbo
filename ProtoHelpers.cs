using LeadTurbo.VirtualDatabase;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo
{
    public static class ProtoHelpers
    {
        public static byte[] Serialize<T>(T obj)
        {
            if (obj == null)
                return null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            using (var ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }


        public static FileInfo Serialize<T>(T obj, string fileName)
        {
            if (obj == null)
            {
                return null;
            }
            if (File.Exists(fileName))
            {

                string backFileName = $"{fileName}.{DateTime.Now:yyMMddHHmmss}Back";

                File.Copy(fileName, backFileName, true);
            }


            // 使用 FileMode.Create 自动覆盖并截断旧文件
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Serializer.Serialize(fs, obj);
            }
            return new FileInfo(fileName);
            
        }

        public static T Deserialize<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default(T);
            }

            FileInfo result = new FileInfo(fileName);

            using (FileStream ms = result.OpenRead())
            {
                return Serializer.Deserialize<T>(ms);

            }
        }


        public static string GenerateProto(IEnumerable<Type> protoTypes, string packageName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("syntax = \"proto2\";");  // protobuf-net 默认生成 proto2 风格 :contentReference[oaicite:1]{index=1}
            sb.AppendLine();
            sb.AppendLine($"package {packageName};");
            sb.AppendLine();

            foreach (var type in protoTypes)
            {
                // 调用 protobuf-net 的 GetSchema 方法 (属于 RuntimeTypeModel) 输出 schema 描述
                string schema = RuntimeTypeModel.Default.GetSchema(type);
                sb.AppendLine(schema);
                sb.AppendLine();
            }
            return sb.ToString();
        }



    }
}
