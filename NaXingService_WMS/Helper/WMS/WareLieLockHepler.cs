using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Helper.WMS
{
    public class WareLieLockHepler
    {
        RedisHelper redisHelper = new RedisHelper();
        string keyPrefix = "WareLieLock";

        public static string lockType_PreIn = "PreIn";
        public static string lockType_PreOut = "PreOut";
        public static string lockType_PreMove = "PreMove";

        public List<string> GetLockLies(bool isIn)
        {
            string key = isIn ? lockType_PreIn : lockType_PreOut;
            return redisHelper.SortedSetRangeByRank<string>(key, keyPrefix);
        }

        public List<string> GetLockLieBatchNo(string lieName,bool isIn)
        {
            string key = isIn ? lockType_PreIn : lockType_PreOut;
            
            return redisHelper.SortedSetRangeByRank<string>(key, keyPrefix);
        }

        public double GetLockLieCount(string lieName, bool isIn)
        {
            string key = isIn ? lockType_PreIn : lockType_PreOut;
            key = $"{key}:{lieName}";
            double allCount = 0;
            List<string> batchNoList = GetLockLieBatchNo(lieName, isIn);
            foreach (var batchNo in batchNoList)
            {
                allCount += redisHelper.SortedSetGet($"{key}:{lieName}", batchNo, keyPrefix);
            }
            return allCount;
        }

        public double GetLockLieBatchCount(string lieName, string batchNo, bool isIn)
        {
            string key = isIn ? lockType_PreIn : lockType_PreOut;
            return redisHelper.SortedSetGet($"{key}:{lieName}", batchNo, keyPrefix);
        }


        public double LockLie(string lieName,string batchNo,bool isIn, long count = 1)
        {
            string key = isIn ? lockType_PreIn : lockType_PreOut;

            return redisHelper.SortedSetIncrement(
                $"{key}:{lieName}", batchNo, TimeSpan.FromHours(24), count, keyPrefix);
        }

        public double UnLockLie(string lieName, string batchNo, bool isIn, long count = 1)
        {
            string key = isIn ? lockType_PreIn : lockType_PreOut;
            key = $"{key}:{lieName}";
            double value;
            if ((value=redisHelper.SortedSetDecrement(
                key, batchNo, TimeSpan.FromHours(24), count, keyPrefix))==-1)
            {
                redisHelper.SortedSetRemove(key, batchNo, keyPrefix);
                value = 0;
            }
            return value;
        }


        //public long LockLie(string lieName, long count = 1)
        //{
        //    return redisHelper.StringIncrement(lieName,TimeSpan.FromHours(24), count, keyPrefix);
        //}

        //public long UnLockLie(string lieName, long count = 1)
        //{
        //    return redisHelper.StringDecrement(lieName, TimeSpan.FromHours(24), count, keyPrefix);
        //}

    }


}
