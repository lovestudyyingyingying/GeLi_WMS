using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils
{
    public static class AutoMapperHelper
    {
        ////undefined
        private static bool ConfigExist(Type srcType, Type destType)
        {
            ////undefined
            try
            {
                return Mapper.Configuration.FindMapper(new TypePair(srcType, destType)) != null;

            }
            catch
            {
                return false;
            }
        }

        private static bool ConfigExist<TSrc, TDest>()
        {
            try
            {
                ////undefined
                return Mapper.Configuration.FindMapper(new TypePair(typeof(TSrc), typeof(TDest))) != null;
            }
            catch
            {
                return false;
            }
        }

        public static T MapTo<T>(this object source)
        {
            ////undefined
            if (source == null)
            {
                ////undefined
                return default(T);
            }

            if (!ConfigExist(source.GetType(), typeof(T)))
            {
                ////undefined
                Mapper.Initialize(cfg => cfg.CreateMap(source.GetType(), typeof(T)));
            }

            return Mapper.Map<T>(source);
        }

        public static IList<T> MapTo<T>(this IEnumerable<T> source)
        {
            ////undefined
            foreach (var first in source)
            {
                ////undefined
                if (!ConfigExist(first.GetType(), typeof(T)))
                {
                    ////undefined
                    Mapper.Initialize(cfg => cfg.CreateMap(first.GetType(), typeof(T)));
                }

                break;
            }

            return Mapper.Map<IList<T>>(source);
        }

        public static IList<TDest> MapTo<TSource, TDest>(this IEnumerable<TSource> source)
        {
            ////undefined
            if (!ConfigExist<TSource, TDest>())
            {
                ////undefined
                Mapper.Initialize(cfg => cfg.CreateMap<TSource, TDest>());
            }

            return Mapper.Map<IList<TDest>>(source);
        }

        public static TDest MapTo<TSource, TDest>(this TSource source, TDest dest)
            where TSource : class
            where TDest : class
        {
            ////undefined
            if (source == null)
            {
                ////undefined
                return dest;
            }

            if (!ConfigExist<TSource, TDest>())
            {
                ////undefined
                Mapper.Initialize(cfg => cfg.CreateMap<TSource, TDest>());
            }

            return Mapper.Map<TDest>(source);
        }
    }
}
