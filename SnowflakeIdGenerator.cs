using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo
{
    public class SnowflakeIdGenerator
    {
        private const long Twepoch = 1288834974657L; // Twitter 起始时间戳（可改为项目起始时间）
        private const int WorkerIdBits = 5;  // 机器 ID 位数（5 位 = 32 台机器）
        private const int DatacenterIdBits = 5; // 数据中心 ID 位数（5 位 = 32 个数据中心）
        private const int SequenceBits = 12; // 每毫秒 4096 个序列号

        private const long MaxWorkerId = (1L << WorkerIdBits) - 1; // 31
        private const long MaxDatacenterId = (1L << DatacenterIdBits) - 1; // 31
        private const long SequenceMask = (1L << SequenceBits) - 1; // 4095

        private const int WorkerIdShift = SequenceBits; // 12 位
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits; // 12+5=17 位
        private const int TimestampShift = SequenceBits + WorkerIdBits + DatacenterIdBits; // 12+5+5=22 位

        private readonly long _workerId; // 机器 ID
        private readonly long _datacenterId; // 数据中心 ID
        private long _sequence = 0L; // 每毫秒内的计数
        private long _lastTimestamp = -1L; // 上次生成 ID 的时间戳

        private static readonly object LockObj = new(); // 线程锁

        public SnowflakeIdGenerator(long workerId, long datacenterId)
        {
            if (workerId < 0 || workerId > MaxWorkerId)
                throw new ArgumentException($"Worker ID 不能超过 {MaxWorkerId}");
            if (datacenterId < 0 || datacenterId > MaxDatacenterId)
                throw new ArgumentException($"Datacenter ID 不能超过 {MaxDatacenterId}");

            _workerId = workerId;
            _datacenterId = datacenterId;
        }

        public long NextId()
        {
            lock (LockObj)
            {
                long timestamp = GetCurrentTimestamp();

                // 如果时间回退，抛出异常
                if (timestamp < _lastTimestamp)
                    throw new Exception($"系统时钟回退，拒绝生成 ID！时间差：{_lastTimestamp - timestamp}ms");

                if (timestamp == _lastTimestamp)
                {
                    // 同一毫秒内，序列号递增
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        // 序列号溢出，等待下一毫秒
                        timestamp = WaitNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    // 新的毫秒，序列号重置
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                // 生成 ID
                return ((timestamp - Twepoch) << TimestampShift) |
                       (_datacenterId << DatacenterIdShift) |
                       (_workerId << WorkerIdShift) |
                       _sequence;
            }
        }

        private static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private static long WaitNextMillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }
    }
}
