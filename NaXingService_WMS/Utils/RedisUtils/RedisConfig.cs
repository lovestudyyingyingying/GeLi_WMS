using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.RedisUtils
{
    public sealed class RedisConfig
    {
        public static readonly string config = ConfigurationManager.AppSettings["RedisConfig"];
        public static readonly string redisKey = ConfigurationManager.AppSettings["RedisKey"] ?? string.Empty;
        public static string Config()
        {
            return config;
        }
        public static string Key()
        {
            return redisKey;
        }
    }
}
