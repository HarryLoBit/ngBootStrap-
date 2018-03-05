using System;
using System.Collections.Generic;
using System.Data;
using Chloe.Core;
using Chloe.InternalExtensions;

namespace Chloe.Infrastructure
{
    public static class MappingTypeSystem
    {
        static readonly object _lockObj = new object();

        static readonly Dictionary<Type, MappingTypeInfo> _defaultTypeInfos;
        static readonly Dictionary<Type, MappingTypeInfo> _typeInfos;

        static MappingTypeSystem()
        {
            Dictionary<Type, MappingTypeInfo> defaultTypeInfos = new Dictionary<Type, MappingTypeInfo>();
            SetItem(defaultTypeInfos, typeof(byte), DbType.Byte);
            SetItem(defaultTypeInfos, typeof(sbyte), DbType.SByte);
            SetItem(defaultTypeInfos, typeof(short), DbType.Int16);
            SetItem(defaultTypeInfos, typeof(ushort), DbType.UInt16);
            SetItem(defaultTypeInfos, typeof(int), DbType.Int32);
            SetItem(defaultTypeInfos, typeof(uint), DbType.UInt32);
            SetItem(defaultTypeInfos, typeof(long), DbType.Int64);
            SetItem(defaultTypeInfos, typeof(ulong), DbType.UInt64);
            SetItem(defaultTypeInfos, typeof(float), DbType.Single);
            SetItem(defaultTypeInfos, typeof(double), DbType.Double);
            SetItem(defaultTypeInfos, typeof(decimal), DbType.Decimal);
            SetItem(defaultTypeInfos, typeof(bool), DbType.Boolean);
            SetItem(defaultTypeInfos, typeof(string), DbType.String);
            SetItem(defaultTypeInfos, typeof(Guid), DbType.Guid);
            SetItem(defaultTypeInfos, typeof(DateTime), DbType.DateTime);
            SetItem(defaultTypeInfos, typeof(DateTimeOffset), DbType.DateTimeOffset);
            SetItem(defaultTypeInfos, typeof(TimeSpan), DbType.Time);
            SetItem(defaultTypeInfos, typeof(byte[]), DbType.Binary);

            _typeInfos = Utils.Clone(defaultTypeInfos);
            _defaultTypeInfos = Utils.Clone(defaultTypeInfos);
        }

        static void SetItem(Dictionary<Type, MappingTypeInfo> map, Type type, DbType mapDbType, IDbValueConverter dbValueConverter = null)
        {
            map[type] = new MappingTypeInfo(type, mapDbType, dbValueConverter);
        }

        /// <summary>
        /// 配置映射的类型。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbTypeToMap">类型 type 对应的 DbType。如果是扩展的 DbType，务必对原生的 System.Data.IDataParameter 进行包装，拦截 IDataParameter.DbType 属性的 setter 以对 dbTypeToMap 处理。</param>
        /// <param name="dbValueConverter">指定一个转换器。可为 null。</param>
        public static void Configure(Type type, DbType dbTypeToMap, IDbValueConverter dbValueConverter)
        {
            Utils.CheckNull(type);

            type = type.GetUnderlyingType();
            lock (_lockObj)
            {
                SetItem(_typeInfos, type, dbTypeToMap, dbValueConverter);
            }
        }
        public static void Configure(Type type, DbType dbTypeToMap, Func<object, object> dbValueConverter = null)
        {
            Configure(type, dbTypeToMap, dbValueConverter == null ? null : new DbValueConverter(dbValueConverter));
        }

        public static DbType? GetDbType(Type type)
        {
            if (type == null)
                return null;

            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                underlyingType = Enum.GetUnderlyingType(underlyingType);

            MappingTypeInfo mappingTypeInfo;
            if (_typeInfos.TryGetValue(underlyingType, out mappingTypeInfo))
                return mappingTypeInfo.MapDbType;

            return null;
        }
        public static bool IsMappingType(Type type)
        {
            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                return true;

            return _typeInfos.ContainsKey(underlyingType);
        }
        public static bool IsMappingType(Type type, out MappingTypeInfo mappingTypeInfo)
        {
            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                underlyingType = Enum.GetUnderlyingType(underlyingType);

            return _typeInfos.TryGetValue(underlyingType, out mappingTypeInfo);
        }
    }

    public class MappingTypeInfo
    {
        public MappingTypeInfo(Type type, DbType mapDbType, IDbValueConverter dbValueConverter)
        {
            this.Type = type;
            this.MapDbType = mapDbType;
            this.DbValueConverter = dbValueConverter;
        }
        public Type Type { get; private set; }
        public DbType MapDbType { get; private set; }
        public IDbValueConverter DbValueConverter { get; private set; }
    }

    class DbValueConverter : IDbValueConverter
    {
        Func<object, object> _dbValueConverter;
        public DbValueConverter(Func<object, object> dbValueConverter)
        {
            this._dbValueConverter = dbValueConverter;
        }
        public object Convert(object readerValue)
        {
            return this._dbValueConverter(readerValue);
        }
    }
}
