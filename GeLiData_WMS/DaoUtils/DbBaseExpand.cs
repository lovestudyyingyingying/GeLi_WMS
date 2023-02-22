using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMSUtils
{
    public static class DbBaseExpand
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

      
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        public static Expression<Func<T, bool>> SelectorExpToBool<T>(this Expression<Func<T, object>> propertyExp, object value)
        {
            var member = (MemberExpression)propertyExp.Body;
            string propertyName = member.Member.Name;
            Type type = propertyExp.Body.Type;
            ParameterExpression parameter = (ParameterExpression)member.Expression;
            ConstantExpression constant = Expression.Constant(value, type);//创建常数
                                                                           //ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
                                                                           //var member = GetPropertySelector(parameter, propertyName);
            var exp = Expression.Equal(member, constant);
            var addExp = Expression.Lambda<Func<T, bool>>(exp, parameter);
            return addExp;

        }
    }
}
