using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_WMS.Models
{
    //public string success = "success";
    //public string success = "success";

    /// <summary>
    /// WebApi 通用返回值
    /// </summary>
    public class ApiResult<T>
    {


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
        public T data {get;set;}
        
       

    }

} 
