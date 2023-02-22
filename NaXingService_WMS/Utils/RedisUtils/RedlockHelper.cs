using RedLockNet;
using RedLockNet.SERedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.RedisUtils
{
    public class RedlockHelper
    {
        /// <summary>
        /// 分布式锁
        /// </summary>
        private RedLockFactory _redLockFactory;
        public RedlockHelper(RedLockFactory redLockFactory)
        {
            _redLockFactory = redLockFactory;
        }

        public void Lock(Action action, string val, string key, TimeSpan? expiryTime = null
            , TimeSpan? waitTime = null, TimeSpan? retryTime = null)
        {
            string resourceName = string.Format("Redlock:{0}:{1}", key, val);
            using (var locker=_redLockFactory.CreateLock(resourceName, expiryTime ?? TimeSpan.FromSeconds(10)
                , waitTime ?? TimeSpan.FromSeconds(2), retryTime ?? TimeSpan.FromMilliseconds(500)))
            {
                action.Invoke();
            }

        }
        ///// <summary>
        ///// 用法：用using ()包裹方法，方便自动回收Lock
        ///// </summary>
        ///// <param name="key">锁名称</param>
        ///// <param name="expiryTime">锁的超时时间</param>
        ///// <param name="waitTime">等待锁的时间</param>
        ///// <param name="retryTime">间隔多少秒检测一次锁</param>
        ///// <returns>是否得到锁，True则执行</returns>
        //public IRedLock CreateLock(string key, TimeSpan? expiryTime = null
        //    , TimeSpan? waitTime = null, TimeSpan? retryTime = null)
        //{
        //    return _redLockFactory.CreateLock(key, expiryTime ?? TimeSpan.FromSeconds(5)
        //        , waitTime ?? TimeSpan.FromSeconds(2), retryTime ?? TimeSpan.FromMilliseconds(500));
        //}
    }
}
