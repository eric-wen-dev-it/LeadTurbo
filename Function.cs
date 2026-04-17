using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo
{
    public static class Function
    {
        /// <summary>
        /// 将集合按固定数目拆分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(List<T> keys, int count)
        {


            List<List<T>> rangeKey = new List<List<T>>();
            int point = 0;
            int keyCount = keys.Count;
            while (point != keyCount)
            {
                int rangeCount = keyCount - point;

                if (rangeCount >= count)
                {
                    rangeKey.Add(keys.GetRange(point, count));
                    point += count;
                }
                else
                {
                    rangeKey.Add(keys.GetRange(point, rangeCount));
                    point += rangeCount;
                }

            }
            return rangeKey;
        }

        /// <summary>
        /// 将集合按固定数目拆分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(T[] keys, int count)
        {
            List<T> keys1 = new List<T>(keys);

            return RangeKey<T>(keys1, count);
        }






        /// <summary>
        /// 将集合拆分
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(List<T> keys)
        {
            return RangeKey<T>(keys, MaxRangeKeyCount.Count50);
        }

        /// <summary>
        /// 将集合拆分
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(T[] keys)
        {
            return RangeKey<T>(new List<T>(keys), MaxRangeKeyCount.Count50);
        }


        /// <summary>
        /// 将集合拆分
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(T[] keys, MaxRangeKeyCount maxRangeKeyCount)
        {
            return RangeKey<T>(new List<T>(keys), maxRangeKeyCount);
        }


        public enum MaxRangeKeyCount
        {

            Count50,
            Count20,
            Count10,
            Count5,
            Count2,
            Count1,
            Count100,
            Count200,
            Count500,
            Count1000

        }

        // 所有标准块尺寸，降序排列
        private static readonly int[] AllChunkSizes = [1000, 500, 200, 100, 50, 20, 10, 5, 2, 1];

        private static int ToInt(MaxRangeKeyCount maxRangeKeyCount) => maxRangeKeyCount switch
        {
            MaxRangeKeyCount.Count1000 => 1000,
            MaxRangeKeyCount.Count500  => 500,
            MaxRangeKeyCount.Count200  => 200,
            MaxRangeKeyCount.Count100  => 100,
            MaxRangeKeyCount.Count50   => 50,
            MaxRangeKeyCount.Count20   => 20,
            MaxRangeKeyCount.Count10   => 10,
            MaxRangeKeyCount.Count5    => 5,
            MaxRangeKeyCount.Count2    => 2,
            MaxRangeKeyCount.Count1    => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(maxRangeKeyCount))
        };

        /// <summary>
        /// 将集合拆分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="maxRangeKeyCount"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(List<T> keys, MaxRangeKeyCount maxRangeKeyCount)
        {
            int maxSize = ToInt(maxRangeKeyCount);
            // 计算可用尺寸（所有 <= maxSize 的标准块尺寸），仅在循环前执行一次
            int[] sizes = AllChunkSizes.SkipWhile(s => s > maxSize).ToArray();

            var result = new List<List<T>>();
            int point = 0;
            int keyCount = keys.Count;
            while (point < keyCount)
            {
                int remaining = keyCount - point;
                // 贪心取最大可用块：从大到小找第一个 <= remaining 的尺寸
                int chunkSize = 1;
                foreach (int s in sizes)
                {
                    if (s <= remaining) { chunkSize = s; break; }
                }
                result.Add(keys.GetRange(point, chunkSize));
                point += chunkSize;
            }
            return result;
        }




        //public static bool ValidateBindings(DependencyObject parent)
        //{
        //    bool valid = true;
        //    LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
        //    while (localValues.MoveNext())
        //    {
        //        LocalValueEntry entry = localValues.Current;
        //        if (BindingOperations.IsDataBound(parent, entry.Property))
        //        {
        //            Binding binding = BindingOperations.GetBinding(parent, entry.Property);
        //            foreach (ValidationRule rule in binding.ValidationRules)
        //            {
        //                ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
        //                if (!result.IsValid)
        //                {
        //                    BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
        //                    Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
        //                    valid = false;
        //                }
        //            }
        //        }
        //    }


        //    for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
        //        if (!ValidateBindings(child))
        //        {
        //            valid = false;
        //        }

        //    }

        //    return valid;
        //}


        public static void RandomOrder<T>(T[] array)
        {

            for (int a = 0; a < array.Length; a++)
            {
                int index = GetRandom(0, array.Length);
                T value = array[a];
                array[a] = array[index];
                array[index] = value;
            }
        }


        public static void DeleteSubDirectory(string path)
        {
            IEnumerable<String> directories = Directory.EnumerateDirectories(path);
            foreach (String directory in directories)
            {
                DeleteSubDirectory(directory);

            }



            IEnumerable<String> files = Directory.EnumerateFiles(path);
            foreach (String file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    //LogRecord.WriteDebugLog(ex.Message);

                }

            }

            directories = Directory.EnumerateDirectories(path);
            foreach (String directory in directories)
            {
                try
                {
                    Directory.Delete(directory);
                }
                catch (Exception ex)
                {
                    //LogRecord.WriteDebugLog(ex.Message);

                }

            }



        }







        public static int GetRandom(int min, int max)
        {
            return Random.Shared.Next(min, max);

        }

        public static string GetRandomText()
        {
            Char[] charData = {'0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
            'P','Q','R','S','T','U','V','W','X','Y','Z'};

            int count = Random.Shared.Next(1, 50);
            char[] data = new char[count];
            for (int a = 0; a < count; a++)
            {
                int index = Random.Shared.Next(0, charData.Length);
                data[a] = charData[index];
            }
            return new string(data);


        }

        const int M = int.MaxValue;
        const int seed = 131;// 31 131 1313 13131 131313 etc..  
        // BKDR Hash Function  
        public static int BKDRHash(byte[] str)
        {
            int hash = 0;
            for (int a = 0; a < str.Length; a++)
            {
                hash = unchecked(hash * seed + str[a]);

            }
            //int Return = hash % M;
            return hash;
        }


        /// <summary>
        /// 转换为26进制
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="digital">输出数位长度</param>
        /// <returns></returns>
        public static string ToBase26(uint value, int digital)
        {
            Char[] charData = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
            'P','Q','R','S','T','U','V','W','X','Y','Z'};
            Stack<char> stackChar = new Stack<char>();
            while (value > 0)
            {
                uint index = value % 26;
                value /= 26;
                stackChar.Push(charData[index]);
            }

            int count = digital - stackChar.Count;

            for (int a = 0; a < count; a++)
            {
                stackChar.Push('A');

            }

            StringBuilder sb1 = new StringBuilder();

            while (stackChar.Count > 0)
            {
                sb1.Append(stackChar.Pop());

            }

            return sb1.ToString();
        }

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        //public static BitmapImage RawToBitmapImage(Stream rawStream)
        //{
        //    using (MagickImage magickImage = new MagickImage(rawStream))
        //    {
        //        MemoryStream memoryStream = new MemoryStream();
        //        magickImage.Write(memoryStream, MagickFormat.Jpg);
        //        memoryStream.Position = 0;
        //        BitmapImage bitmapImage = new BitmapImage();
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = memoryStream;
        //        bitmapImage.EndInit();
        //        bitmapImage.Freeze();
        //        return bitmapImage;

        //    }
        //}

        public static string DescriptionOfDateSpan(DateTime neededDateOfDescription)
        {
            int[,] theFirstDayInWeekTable = new int[,]
                {
                    {0,1,2,3,4,5,6},
                    {6,0,1,2,3,4,5},
                    {5,6,0,1,2,3,4},
                    {4,5,6,0,1,2,3},
                    {3,4,5,6,0,1,2},
                    {2,3,4,5,6,0,1},
                    {1,2,3,4,5,6,0},
                };


            DateTime thisWeekOfFirstDay = DateTime.Now.AddDays(-theFirstDayInWeekTable[1, (int)DateTime.Now.DayOfWeek]).Date;
            DateTime thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            DateTime lastMonth = thisMonth.AddMonths(-1);
            DateTime inTrimester = lastMonth.AddMonths(-1);


            if (neededDateOfDescription.Date == DateTime.Now.Date)
            {
                return "Today";
            }
            else if (neededDateOfDescription.Date == DateTime.Now.AddDays(-1).Date)
            {
                return "Yesterday";
            }
            else if (neededDateOfDescription.Date >= thisWeekOfFirstDay)
            {
                return "This week";
            }
            else if (neededDateOfDescription.Date >= thisWeekOfFirstDay.AddDays(-7))
            {
                return "Last week";
            }
            else if (neededDateOfDescription.Date >= thisMonth)
            {
                return "This month";
            }
            else if (neededDateOfDescription.Date >= lastMonth)
            {
                return "Last month";
            }
            else if (neededDateOfDescription.Date >= inTrimester)
            {
                return "In trimester";
            }
            else
            {
                return "Earlier";
            }


        }
        public static string GetRuntimeDirectory(string path)
        {
            //ForLinux
            if (IsLinuxRunTime())
                return GetLinuxDirectory(path);
            //ForWindows
            if (IsWindowRunTime())
                return GetWindowDirectory(path);
            return path;
        }

        //OSPlatform.Windows监测运行环境
        public static bool IsWindowRunTime()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        //OSPlatform.Linux运行环境
        public static bool IsLinuxRunTime()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static string GetLinuxDirectory(string path)
        {
            string pathTemp = Path.Combine(path);
            return pathTemp.Replace("\\", "/");
        }
        public static string GetWindowDirectory(string path)
        {
            string pathTemp = Path.Combine(path);
            return pathTemp.Replace("/", "\\");
        }



        /// <summary>
        /// 计算指定日期所属财务年的起始与结束日期
        /// </summary>
        /// <param name="date">输入日期</param>
        /// <param name="fiscalStartMonth">财年起始月份（1–12）</param>
        /// <returns>Tuple：Item1 = 财年开始日期，Item2 = 财年结束日期</returns>
        public static (DateTime Start, DateTime End) GetFiscalYearDates(DateTime date, int fiscalStartMonth)
        {
            // 如果日期早于当年财年开始月，则财年从“上一年”的fiscalStartMonth开始
            int startYear = date.Month < fiscalStartMonth ? date.Year - 1 : date.Year;
            DateTime start = new DateTime(startYear, fiscalStartMonth, 1);
            DateTime end = start.AddYears(1).AddDays(-1);
            return (start, end);
        }

        /// <summary>
        /// 计算字节序列的 XxHash3 64 位哈希（非加密哈希，用于索引/去重）。
        /// </summary>
        public static ulong ComputeXxHash3FromBytes(ReadOnlySpan<byte> input)
        {
            return XxHash3.HashToUInt64(input);
        }

        /// <summary>
        /// 计算字符串的 XxHash3 64 位哈希。
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="encoding">字节编码，默认 UTF-8</param>
        public static ulong ComputeXxHash3FromString(string input, Encoding encoding = null)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            encoding ??= Encoding.UTF8;
            return XxHash3.HashToUInt64(encoding.GetBytes(input));
        }

        /// <summary>
        /// 计算流内容的 XxHash3 64 位哈希。
        /// </summary>
        public static ulong ComputeXxHash3FromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Stream must be readable.", nameof(stream));
            var hasher = new XxHash3();
            hasher.Append(stream);
            return hasher.GetCurrentHashAsUInt64();
        }

        public static long ComputeXxHash3SignedFromBytes(ReadOnlySpan<byte> input)
            => unchecked((long)ComputeXxHash3FromBytes(input));

        public static long ComputeXxHash3SignedFromString(string input, Encoding encoding = null)
            => unchecked((long)ComputeXxHash3FromString(input, encoding));

        public static long ComputeXxHash3SignedFromStream(Stream stream)
            => unchecked((long)ComputeXxHash3FromStream(stream));






    }
}
