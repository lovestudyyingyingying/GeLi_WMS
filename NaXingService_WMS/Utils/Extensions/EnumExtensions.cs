using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription<TEnum>(this TEnum item)
            => item.GetType()
               .GetField(item.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false)
               .Cast<DescriptionAttribute>()
               .FirstOrDefault()?.Description ?? string.Empty;

        public static string GetEnumDescriptionByString<TEnum>(string numString)
        {
            Array array = Enum.GetValues(typeof(TEnum));
            string description = string.Empty;
            foreach (var item in array)
            {
                if (Convert.ToInt32(item) == Convert.ToInt32(numString))
                {
                    object[] objAttrs = item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                    description = item.ToString();

                    if (objAttrs != null && objAttrs.Length > 0)
                    {
                        DescriptionAttribute descAttr = objAttrs[0] as DescriptionAttribute;
                        description = descAttr.Description;
                        break;
                    }
                }
            }
            return description;

        }

        //public static void SeedEnumValues<T, TEnum>(this IDbSet<T> dbSet, Func<TEnum, T> converter)
        //    where T : class => Enum.GetValues(typeof(TEnum))
        //                           .Cast<object>()
        //                           .Select(value => converter((TEnum)value))
        //                           .ToList()
        //                           .ForEach(instance => dbSet.AddOrUpdate(instance));

    }
}
