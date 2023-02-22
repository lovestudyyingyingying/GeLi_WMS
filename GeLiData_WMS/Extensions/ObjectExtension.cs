using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMS.Extensions
{
    /// <summary>
    /// Object拓展方法，.Net4.0以上
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 根据属性名获取属性值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="name">属性名</param>
        /// <returns>属性的值</returns>
        public static object GetPropertyValue<T>(this T t, string name)
        {
            Type type = t.GetType();
            PropertyInfo p = type.GetProperty(name);
            if (p == null)
            {
                throw new Exception(String.Format("该类型没有名为{0}的属性", name));
            }

            var param_obj = Expression.Parameter(typeof(T));
            var param_val = Expression.Parameter(typeof(object));

            // 原有的，非string类型会报转换object错误
            //转成真实类型，防止Dynamic类型转换成object
            //var body_obj = Expression.Convert(param_obj, type);

            //var body = Expression.Property(body_obj, p);
            //var getValue = Expression.Lambda<Func<T, object>>(body, param_obj).Compile();

            //转成真实类型，防止Dynamic类型转换成object
            Expression<Func<T, object>> result = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param_obj, p), typeof(object)), param_obj);
            var getValue = result.Compile();
            return getValue(t);
        }

        /// <summary>
        /// 根据属性名称设置属性的值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">对象</param>
        /// <param name="name">属性名</param>
        /// <param name="value">属性的值</param>
        public static void SetPropertyValue<T>(this T t, string name, object value)
        {
            Type type = t.GetType();
            PropertyInfo p = type.GetProperty(name);
            if (p == null)
            {
                throw new Exception(String.Format("该类型没有名为{0}的属性", name));
            }
            var param_obj = Expression.Parameter(type);
            var param_val = Expression.Parameter(typeof(object));
            var body_obj = Expression.Convert(param_obj, type);
            var body_val = Expression.Convert(param_val, p.PropertyType);

            //获取设置属性的值的方法
            var setMethod = p.GetSetMethod(true);

            //如果只是只读,则setMethod==null
            if (setMethod != null)
            {
                var body = Expression.Call(param_obj, p.GetSetMethod(), body_val);
                var setValue = Expression.Lambda<Action<T, object>>(body, param_obj, param_val).Compile();
                setValue(t, value);
            }
        }

        /// <summary>
        /// 获取类的属性名称
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propery"></param>
        /// <returns></returns>
        public static string GetPropName<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propery) where TSource : class
        {
            var body = propery.Body.ToString();
            return body.Substring(body.LastIndexOf(".") + 1);
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
        Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        /// <summary>
        /// 获取类的属性信息
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda) where TSource : class
        {
            return GetPropertyInfo(propertyLambda);
        }

        /// <summary>
        /// 获取类的属性名称 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static string NameOfProperty<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda) where TSource : class
        {
            PropertyInfo prodInfo = GetPropertyInfo(propertyLambda);
            return prodInfo.Name;
        }

        /// <summary>
        /// 比较--两个类型一样的实体类对象的值
        /// </summary>
        /// <param name="oneT"></param>
        /// <param name="twoT"></param>
        /// <returns></returns>
        public static bool CompareType<T>(T oneT, T twoT, Func<PropertyInfo,bool> noMapperFunc=null, bool isDeep = false)
        {
            bool result = true;//两个类型作比较时使用,如果有不一样的就false
            Type typeOne = oneT.GetType();
            Type typeTwo = twoT.GetType();
            //如果两个T类型不一样  就不作比较
            if (!typeOne.Equals(typeTwo)) { return false; }
            
            PropertyInfo[] pisOne = typeOne.GetProperties().Where(noMapperFunc).ToArray(); //获取所有公共属性(Public)
            PropertyInfo[] pisTwo = typeTwo.GetProperties().Where(noMapperFunc).ToArray();
            //如果长度为0返回false
            if (pisOne.Length <= 0 || pisTwo.Length <= 0)
            {
                return false;
            }
            //如果长度不一样，返回false
            if (!(pisOne.Length.Equals(pisTwo.Length))) { return false; }
            //遍历两个T类型，遍历属性，并作比较
            for (int i = 0; i < pisOne.Length; i++)
            {
                //获取属性名
                string oneName = pisOne[i].Name;
                string twoName = pisTwo[i].Name;
                
                //获取属性的值
                object oneValue = pisOne[i].GetValue(oneT, null);
                object twoValue = pisTwo[i].GetValue(twoT, null);
                //Debug.WriteLine(oneName+"---  "+ oneValue + "---  "+ twoValue);
                if (pisOne[i].PropertyType==typeof(DateTime))
                {
                    oneValue = ((DateTime)oneValue).ToString("G");
                    twoValue = ((DateTime)twoValue).ToString("G");
                }
                //比较,只比较值类型
                if ((pisOne[i].PropertyType.IsValueType || pisOne[i].PropertyType.Name.StartsWith("String")) && (pisTwo[i].PropertyType.IsValueType || pisTwo[i].PropertyType.Name.StartsWith("String")))
                {
                    if (oneName.Equals(twoName))
                    {
                        if (oneValue == null)
                        {
                            if (twoValue != null)
                            {
                                result = false;
                                break; //如果有不一样的就退出循环
                            }
                        }
                        else if (oneValue != null)
                        {
                            if (twoValue != null)
                            {
                                if (!oneValue.Equals(twoValue))
                                {
                                    result = false;
                                    break; //如果有不一样的就退出循环
                                }
                            }
                            else if (twoValue == null)
                            {
                                result = false;
                                break; //如果有不一样的就退出循环
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                else
                {
                    if (!isDeep)
                        continue;
                    //如果对象中的属性是实体类对象，递归遍历比较
                    bool b = CompareType(oneValue, twoValue);
                    if (!b) { result = b; break; }
                }
            }
            return result;
        }

        public static T ChangeByObject<T>(this T obj1, T obj2)
        {
            PropertyInfo[] propertyInfos_Obj1 = obj1.GetType().GetProperties().Where(u => u.Name.ToUpper() != "ID").ToArray();
            PropertyInfo[] propertyInfos_Obj2 = obj2.GetType().GetProperties().Where
                (u => u.Name.ToUpper() != "ID").ToArray(); ;

            foreach (var temp in propertyInfos_Obj2)
            {
                object value = temp.GetValue(obj2);
                PropertyInfo pd2 = propertyInfos_Obj1.First(u => u.Name == temp.Name); ;
                pd2.SetValue(obj1, value);
            }
            return obj1;
        }

    }
}
