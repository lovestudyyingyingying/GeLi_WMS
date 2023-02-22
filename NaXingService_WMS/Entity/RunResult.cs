using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity
{
    public class RunResult<T>
    {
       
        public RunResult(string message, int code, T data)
        {
            this.message = message;
            this.code = code;
            this.desc = data;
        }

        public RunResult()
        {
        }

        public static RunResult<string> True()
        {
            return new RunResult<string>(ResultMsg.success, ResultMsg.successCode, string.Empty);
        }
        //public static RunResult<string> True(string str) { 
        //    return new RunResult<string>(ResultMsg.success,ResultMsg.successCode, str);
        //}
        public static RunResult<T> True(T obj)
        {
            return new RunResult<T>(ResultMsg.success, ResultMsg.successCode, obj);
        }

        public static RunResult<string> False()
        {
            return new RunResult<string>(ResultMsg.fail, ResultMsg.failCode, string.Empty);
        }
        //public static RunResult<string> False(string str)
        //{
        //    return new RunResult<string>(ResultMsg.fail, ResultMsg.failCode, str);
        //}
        public static RunResult<T> False(T obj)
        {
            return new RunResult<T>(ResultMsg.fail, ResultMsg.failCode, obj);
        }
        /// <summary>
        /// 接口返回结果
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 接口返回结果代码 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 接口返回结果数据
        /// </summary>
        public T desc { get; set; }

        public override string ToString()
        {
            return $"执行结果：{message}\r\n结果代码：{code}\r\n返回数据：{desc}";
        }
    }

    public class ResultMsg
    {
        public static string success = "success";
        public static string fail = "fail";

        public static int successCode = 1000;
        public static int failCode = 1001;

        public static string Emt = string.Empty;
    }
}
