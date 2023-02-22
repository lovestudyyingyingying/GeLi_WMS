using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.RedisUtils
{
    public class RedisBase
    {
        private static ConnectionMultiplexer db = null;
        private static string key = string.Empty;

        private int DbNumber { get; }
        public RedisBase(int dbnum = 0) : this(dbnum, null)
        {

        }

        public RedisBase(int dbnum, string connectionString)
        {
            DbNumber = dbnum;
            db = string.IsNullOrWhiteSpace(connectionString) ? RedisManager.Instance : RedisManager.GetConForMap(connectionString);
        }

        #region 辅助方法
        /// <summary>
        /// 添加名称
        /// </summary>
        /// <param name="old"></param>
        /// <returns></returns>
        public string AddKey(string old)
        {
            var fixkey = key ?? RedisConfig.Key();
            return fixkey + old;
        }
        public bool RemoveKey(string key)
        {
            return db.GetDatabase().KeyDelete(key);
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T DoSave<T>(Func<IDatabase, T> func)
        {
            return func(db.GetDatabase(DbNumber));
        }
        public string ConvertJson<T>(T val)
        {
            return val is string ? val.ToString() : JsonConvert.SerializeObject(val);
        }

        public T ConvertObj<T>(RedisValue val)
        {
            return JsonConvert.DeserializeObject<T>(val);
        }

        public List<T> ConvertList<T>(RedisValue[] val)
        {
            List<T> result = new List<T>();
            foreach (var item in val)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        public RedisKey[] ConvertRedisKeys(List<string> val)
        {
            return val.Select(k => (RedisKey)k).ToArray();
        }
        #endregion
    }
}
