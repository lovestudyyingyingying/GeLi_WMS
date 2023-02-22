using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils
{
    public class RedisCacheHelper
    {
        private static readonly PooledRedisClientManager pool = null;
        private static readonly string[] redisHosts = null;
        public static int RedisMaxReadPool = int.Parse(ConfigurationManager.AppSettings["redis_max_read_pool"]);
        public static int RedisMaxWritePool = int.Parse(ConfigurationManager.AppSettings["redis_max_write_pool"]);

        static RedisCacheHelper()
        {
            var redisHostStr = ConfigurationManager.AppSettings["redis_server_session"];

            if (!string.IsNullOrEmpty(redisHostStr))
            {
                redisHosts = redisHostStr.Split(',');

                if (redisHosts.Length > 0)
                {
                    pool = new PooledRedisClientManager(redisHosts, redisHosts,
                        new RedisClientManagerConfig()
                        {
                            MaxWritePoolSize = RedisMaxWritePool,
                            MaxReadPoolSize = RedisMaxReadPool,
                            AutoStart = true
                        });
                }
            }
        }

        #region 字符串
        public static void Add<T>(string key, T value, DateTime expiry)
        {
            if (value == null)
            {
                return;
            }

            if (expiry <= DateTime.Now)
            {
                Remove(key);
                return;
            }

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Set(key, value, expiry - DateTime.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
            }
        }

        public static void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (value == null)
            {
                return;
            }

            if (slidingExpiration.TotalSeconds <= 0)
            {
                Remove(key);

                return;
            }

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Set(key, value, slidingExpiration);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
            }

        }

        public static long Incr(string key, DateTime expiry)
        {
            long obj = 0;
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj= r.IncrementValue(key);
                            r.ExpireEntryAt(key, expiry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
            }
            return obj;
        }

        public static long Decr(string key, DateTime expiry)
        {
            long obj = 0;
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj = r.DecrementValue(key);
                            r.ExpireEntryAt(key, expiry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
            }
            return obj;
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            T obj = default(T);

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj = r.Get<T>(key);
                        }
                    }
                }
            }
            catch(SerializationException ex)
            {
                Console.WriteLine(ex.Message);
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            string ret= r.Get<string>(key);
                            obj = JsonConvert.DeserializeObject<T>(ret);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "获取", key);
                Console.WriteLine(msg);
            }


            return obj;
        }

        public static void Remove(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Remove(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "删除", key);
            }

        }

        public static bool Exists(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            return r.ContainsKey(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "是否存在", key);
            }

            return false;
        }
        #endregion

        #region 队列
        /// <summary>
        /// 先进先出,FIFO
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="valueStr"></param>
        public static void AddList(string listId, string valueStr)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.EnqueueItemOnList(listId, valueStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "写入队列", listId + ":" + valueStr);
            }
        }
        /// <summary>
        /// 后进先出，LIFO
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="valueStr"></param>
        public static void AddListLeft(string listId, string valueStr)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.PushItemToList(listId, valueStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "写入队列", listId + ":" + valueStr);
            }
        }

        public static string GetValueInList(string listId)
        {
            string ret = string.Empty;
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            ret = r.DequeueItemFromList(listId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "读取队列", listId);
            }
            return ret;

        }


        #endregion

        #region 有序集合
        public static void AddSortSet(string setId, string valueStr)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.AddItemToSortedSet(setId, valueStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "写入有序集合", setId + ":" + valueStr);
            }
        }
        public static void AddSortSet(string setId, string valueStr, double score)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.AddItemToSortedSet(setId, valueStr, score);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "写入有序集合", setId + ":" + valueStr);
            }
        }

        public static List<string> GetStrValuesInSortSet(string setId)
        {
            List<string> ret = new List<string>();
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            ret = r.GetAllItemsFromSortedSet(setId);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "读取队列", setId);
            }
            return ret;

        }

        public static List<T> GetValuesInSortSet<T>(string setId)
        {
            List<T> obj = new List<T>();
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            List<string> ret = r.GetAllItemsFromSortedSet(setId);
                            foreach (string temp in ret)
                            {
                                obj.Add(JsonConvert.DeserializeObject<T>(temp));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "读取队列", setId);
            }
            return obj;

        }
        public static Dictionary<string, double> GetStrValuesWithScoreInSortSet(string setId)
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            //ret = r.GetAllItemsFromSortedSet(setId);
                            ret = (Dictionary<string, double>)r.GetAllWithScoresFromSortedSet(setId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "读取队列", setId);
            }
            return ret;

        }
        public static Dictionary<T, double> GetValuesWithScoreInSortSet<T>(string setId)
        {
            Dictionary<T, double> obj = new Dictionary<T, double>();
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            //ret = r.GetAllItemsFromSortedSet(setId);
                            Dictionary<string, double> ret = (Dictionary<string, double>)r.GetAllWithScoresFromSortedSet(setId);
                            foreach (KeyValuePair<string, double> temp in ret)
                            {
                                obj.Add(JsonConvert.DeserializeObject<T>(temp.Key), temp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "读取队列", setId);
            }
            return obj;

        }

        public static void RemoveValuesInSortSet<T>(string setId, T obj)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            string value = JsonConvert.SerializeObject(obj);
                            r.RemoveItemFromSortedSet(setId, value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "读取队列", setId);
            }
        }
        #endregion
    }
}
