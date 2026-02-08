using Blake2Fast;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


        /// <summary>
        /// 将集合拆分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <param name="maxRangeKeyCount"></param>
        /// <returns></returns>
        public static List<List<T>> RangeKey<T>(List<T> keys, MaxRangeKeyCount maxRangeKeyCount)
        {
            List<List<T>> rangeKey = new List<List<T>>();
            int point = 0;
            int keyCount = keys.Count;
            while (point != keyCount)
            {

                if (maxRangeKeyCount == MaxRangeKeyCount.Count1000)
                {
                    if (keyCount - point >= 1000)
                    {
                        rangeKey.Add(keys.GetRange(point, 1000));
                        point += 1000;
                    }
                    else if (keyCount - point >= 500)
                    {
                        rangeKey.Add(keys.GetRange(point, 500));
                        point += 500;
                    }
                    else if (keyCount - point >= 200)
                    {
                        rangeKey.Add(keys.GetRange(point, 200));
                        point += 200;
                    }
                    else if (keyCount - point >= 100)
                    {
                        rangeKey.Add(keys.GetRange(point, 100));
                        point += 100;
                    }
                    else if (keyCount - point >= 50)
                    {
                        rangeKey.Add(keys.GetRange(point, 50));
                        point += 50;
                    }
                    else if (keyCount - point >= 20)
                    {
                        rangeKey.Add(keys.GetRange(point, 20));
                        point += 20;
                    }
                    else if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count500)
                {
                    if (keyCount - point >= 500)
                    {
                        rangeKey.Add(keys.GetRange(point, 500));
                        point += 500;
                    }
                    else if (keyCount - point >= 200)
                    {
                        rangeKey.Add(keys.GetRange(point, 200));
                        point += 200;
                    }
                    else if (keyCount - point >= 100)
                    {
                        rangeKey.Add(keys.GetRange(point, 100));
                        point += 100;
                    }
                    else if (keyCount - point >= 50)
                    {
                        rangeKey.Add(keys.GetRange(point, 50));
                        point += 50;
                    }
                    else if (keyCount - point >= 20)
                    {
                        rangeKey.Add(keys.GetRange(point, 20));
                        point += 20;
                    }
                    else if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count200)
                {
                    if (keyCount - point >= 200)
                    {
                        rangeKey.Add(keys.GetRange(point, 200));
                        point += 200;
                    }
                    else if (keyCount - point >= 100)
                    {
                        rangeKey.Add(keys.GetRange(point, 100));
                        point += 100;
                    }
                    else if (keyCount - point >= 50)
                    {
                        rangeKey.Add(keys.GetRange(point, 50));
                        point += 50;
                    }
                    else if (keyCount - point >= 20)
                    {
                        rangeKey.Add(keys.GetRange(point, 20));
                        point += 20;
                    }
                    else if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count100)
                {
                    if (keyCount - point >= 100)
                    {
                        rangeKey.Add(keys.GetRange(point, 100));
                        point += 100;
                    }
                    else if (keyCount - point >= 50)
                    {
                        rangeKey.Add(keys.GetRange(point, 50));
                        point += 50;
                    }
                    else if (keyCount - point >= 20)
                    {
                        rangeKey.Add(keys.GetRange(point, 20));
                        point += 20;
                    }
                    else if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count50)
                {
                    if (keyCount - point >= 50)
                    {
                        rangeKey.Add(keys.GetRange(point, 50));
                        point += 50;
                    }
                    else if (keyCount - point >= 20)
                    {
                        rangeKey.Add(keys.GetRange(point, 20));
                        point += 20;
                    }
                    else if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count20)
                {
                    if (keyCount - point >= 20)
                    {
                        rangeKey.Add(keys.GetRange(point, 20));
                        point += 20;
                    }
                    else if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count10)
                {
                    if (keyCount - point >= 10)
                    {
                        rangeKey.Add(keys.GetRange(point, 10));
                        point += 10;
                    }
                    else if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count5)
                {
                    if (keyCount - point >= 5)
                    {
                        rangeKey.Add(keys.GetRange(point, 5));
                        point += 5;
                    }
                    else if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count2)
                {
                    if (keyCount - point >= 2)
                    {
                        rangeKey.Add(keys.GetRange(point, 2));
                        point += 2;
                    }
                    else
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
                else if (maxRangeKeyCount == MaxRangeKeyCount.Count1)
                {
                    if (keyCount - point > 0)
                    {
                        rangeKey.Add(keys.GetRange(point, 1));
                        point++;
                    }
                }
            }
            return rangeKey;
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


        static Random random = new Random();


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
            return random.Next(min, max);

        }

        public static string GetRandomText()
        {
            Char[] charData = {'0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
            'P','Q','R','S','T','U','V','W','X','Y','Z'};

            int count = random.Next(1, 50);
            char[] data = new char[count];
            for (int a = 0; a < count; a++)
            {
                int index = random.Next(0, charData.Length);
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
        /// 从流计算 BLAKE2b 的 8 字节（64 位）哈希值（unsigned long）。
        /// </summary>
        /// <param name="stream">输入数据流（可读）</param>
        /// <param name="encoding">如流里是文本，则指定其编码；若不是文本可传 null 并直接 Update 字节</param>
        /// <returns>哈希值（unsigned long）</returns>
        public static async Task<ulong> ComputeBlake2b64FromStreamAsync(Stream stream, Encoding encoding = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("Stream must be readable.", nameof(stream));

            // 创建增量哈希器
            var hasher = Blake2b.CreateIncrementalHasher(8);
            // 注意：这里传入 8 表示我们希望输出 8 字节（64 位）哈希  
            // （如果库版本的 CreateIncrementalHasher 默认输出最大长度，需要检查重载版本是否支持指定输出长度）

            // 用缓冲区读流
            byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if (encoding != null)
                    {
                        // 如果你传入了编码，假设流里是文本内容，将字节按 encoding 解码然后再哈希字符串？（通常不必要）
                        // 更合理的是：不要传 encoding，直接对原始字节做哈希
                        // 下面是示意，如果你真的有文本流并要按字符处理的话
                        string chunk = encoding.GetString(buffer, 0, bytesRead);
                        hasher.Update(Encoding.UTF8.GetBytes(chunk));
                    }
                    else
                    {
                        // 直接对字节更新哈希
                        hasher.Update(buffer.AsSpan(0, bytesRead));
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            // 完成哈希，得到输出字节数组
            byte[] hashBytes = hasher.Finish();

            if (hashBytes.Length != 8)
                throw new InvalidOperationException($"Expected 8 bytes in hashBytes but got {hashBytes.Length}");

            // 把 8 字节转换成 unsigned long（假设 big-endian）
            ulong value =
                  ((ulong)hashBytes[0] << 56)
                | ((ulong)hashBytes[1] << 48)
                | ((ulong)hashBytes[2] << 40)
                | ((ulong)hashBytes[3] << 32)
                | ((ulong)hashBytes[4] << 24)
                | ((ulong)hashBytes[5] << 16)
                | ((ulong)hashBytes[6] << 8)
                | ((ulong)hashBytes[7]);

            return value;
        }

        /// <summary>
        /// 如果你需要 signed long (long)，可以这样转换：
        /// </summary>
        public static async Task<long> ComputeBlake2b64SignedFromStreamAsync(Stream stream, Encoding encoding = null)
        {
            ulong long1 = await ComputeBlake2b64FromStreamAsync(stream, encoding);
            return unchecked((long)long1);
        }





        /// <summary>
        /// 方便地对某个字符串（通过 MemoryStream）使用上述方法。
        /// </summary>
        public static async Task<ulong> ComputeBlake2b64FromStringAsync(string input, Encoding encoding = null)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            encoding ??= Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(input);
            return await ComputeBlake2b64FromBytesAsync(bytes);
        }

        /// <summary>
        /// 如果你需要 signed long (long)，可以这样转换：
        /// </summary>
        public static async Task<long> ComputeBlake2b64SignedFromStreamAsync(string input, Encoding encoding = null)
        {
            ulong long1 = await ComputeBlake2b64FromStringAsync(input, encoding);
            return unchecked((long)long1);
        }


        public static async Task<ulong> ComputeBlake2b64FromBytesAsync(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            using (var ms = new MemoryStream(input))
            {
                return await ComputeBlake2b64FromStreamAsync(ms, encoding: null);
            }
        }

        /// <summary>
        /// 如果你需要 signed long (long)，可以这样转换：
        /// </summary>
        public static async Task<long> ComputeBlake2b64SignedFromBytesAsync(byte[] input)
        {
            ulong long1 = await ComputeBlake2b64FromBytesAsync(input);
            return unchecked((long)long1);
        }





    }
}
