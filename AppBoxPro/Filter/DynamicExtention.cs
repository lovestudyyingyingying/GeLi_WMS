using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppBoxPro
{
    public static class DynamicExtention
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, Filter[] filters)
        {
            var param = DynamicLinq.CreateLambdaParam<T>("c");
            Expression body = Expression.Constant(true); //初始默认一个true  
            foreach (var filter in filters)
            {
                body = body.AndAlso(param.GenerateBody<T>(filter)); //这里可以根据需要自由组合  
            }
            var lambda = param.GenerateTypeLambda<T>(body); //最终组成lambda  
           
            return query.Where(lambda);
        }
    }
}

