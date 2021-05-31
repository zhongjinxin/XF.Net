using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XF.Core.Const;


namespace XF.Core.Extensions
{
    public static class EntityProperties
    {
        public static bool ValidationRquiredValueForDbType(this PropertyInfo propertyInfo, object value, out string message)
        {
            if (value == null || value?.ToString()?.Trim() == "")
            {
                message = $"{propertyInfo.GetDisplayName()}不能为空";
                return false;
            }
            var result = propertyInfo.GetProperWithDbType().ValidationVal(value, propertyInfo);
            message = result.Item2;
            return result.Item1;
        }
        public static string GetDisplayName(this PropertyInfo property)
        {
            string displayName = property.GetTypeCustomValue<DisplayAttribute>(x => new { x.Name });
            if (string.IsNullOrEmpty(displayName))
            {
                return property.Name;
            }
            return displayName;
        }
        public static string GetTypeCustomValue<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
        {
            var propertyKeyValues = member.GetTypeCustomValues(expression);
            if (propertyKeyValues == null || propertyKeyValues.Count == 0)
            {
                return null;
            }
            return propertyKeyValues.First().Value ?? "";
        }
        public static object GetTypeCustomAttributes(this PropertyInfo propertyInfo, Type type, out bool asType)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(type, false);
            if (attributes.Length == 0)
            {
                asType = false;
                return new string[0];
            }
            asType = true;
            return attributes[0];
        }
        public static object GetTypeCustomAttributes(this MemberInfo member, Type type)
        {
            object[] obj = member.GetCustomAttributes(type, false);
            if (obj.Length == 0) return null;
            return obj[0];
        }
        public static Dictionary<string, string> GetTypeCustomValues<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
        {
            var attr = member.GetTypeCustomAttributes(typeof(TEntity));
            if (attr == null)
            {
                return null;
            }

            string[] propertyName = expression.GetExpressionProperty();
            Dictionary<string, string> propertyKeyValues = new Dictionary<string, string>();

            foreach (PropertyInfo property in attr.GetType().GetProperties())
            {
                if (propertyName.Contains(property.Name))
                {
                    propertyKeyValues[property.Name] = (property.GetValue(attr) ?? string.Empty).ToString();
                }
            }
            return propertyKeyValues;
        }
        private static readonly Dictionary<Type, string> ProperWithDbType = new Dictionary<Type, string>() {
            {  typeof(string),SqlDbTypeName.NVarChar },
            { typeof(DateTime),SqlDbTypeName.DateTime},
            {typeof(long),SqlDbTypeName.BigInt },
            {typeof(int),SqlDbTypeName.Int},
            { typeof(decimal),SqlDbTypeName.Decimal },
            { typeof(float),SqlDbTypeName.Float },
            { typeof(double),SqlDbTypeName.Double },
            {  typeof(byte),SqlDbTypeName.Int },//类型待完
            { typeof(Guid),SqlDbTypeName.UniqueIdentifier}
        };
        public static string GetProperWithDbType(this PropertyInfo propertyInfo)
        {
            bool result = ProperWithDbType.TryGetValue(propertyInfo.PropertyType, out string value);
            if (result)
            {
                return value;
            }
            return SqlDbTypeName.NVarChar;
        }
        public static (bool, string, object) ValidationVal(this string dbType, object value, PropertyInfo propertyInfo = null)
        {
            if (string.IsNullOrEmpty(dbType))
            {
                dbType = propertyInfo != null ? propertyInfo.GetProperWithDbType() : SqlDbTypeName.NVarChar;
            }
            dbType = dbType.ToLower();
            string val = value?.ToString();
            //验证长度
            string reslutMsg = string.Empty;
            if (dbType == SqlDbTypeName.Int || dbType == SqlDbTypeName.BigInt)
            {
                if (!value.IsInt())
                    reslutMsg = "只能为有效整数";
            }
            else if (dbType == SqlDbTypeName.DateTime
                || dbType == SqlDbTypeName.Date
                || dbType == SqlDbTypeName.SmallDateTime
                || dbType == SqlDbTypeName.SmallDate
                )
            {
                if (!value.IsDate())
                    reslutMsg = "必须为日期格式";
            }
            else if (dbType == SqlDbTypeName.Float || dbType == SqlDbTypeName.Decimal || dbType == SqlDbTypeName.Double)
            {
                //string formatString = string.Empty;
                //if (propertyInfo != null)
                //    formatString = propertyInfo.GetTypeCustomValue<DisplayFormatAttribute>(x => x.DataFormatString);
                //if (string.IsNullOrEmpty(formatString))
                //    throw new Exception("请对字段" + propertyInfo?.Name + "添加DisplayFormat属性标识");

                if (!val.IsNumber(null))
                {
                    // string[] arr = (formatString ?? "10,0").Split(',');
                    // reslutMsg = $"整数{arr[0]}最多位,小数最多{arr[1]}位";
                    reslutMsg = "不是有效数字";
                }
            }
            else if (dbType == SqlDbTypeName.UniqueIdentifier)
            {
                if (!val.IsGuid())
                {
                    reslutMsg = propertyInfo.Name + "Guid不正确";
                }
            }
            else if (propertyInfo != null
                && (dbType == SqlDbTypeName.VarChar
                || dbType == SqlDbTypeName.NVarChar
                || dbType == SqlDbTypeName.NChar
                || dbType == SqlDbTypeName.Char
                || dbType == SqlDbTypeName.Text))
            {

                //默认nvarchar(max) 、text 长度不能超过20000
                if (val.Length > 20000)
                {
                    reslutMsg = $"字符长度最多【20000】";
                }
                else
                {
                    int length = propertyInfo.GetTypeCustomValue<MaxLengthAttribute>(x => new { x.Length }).GetInt();
                    if (length == 0) { return (true, null, null); }
                    //判断双字节与单字段
                    else if (length < 8000 &&
                        ((dbType.Substring(0, 1) != "n"
                        && Encoding.UTF8.GetBytes(val.ToCharArray()).Length > length)
                         || val.Length > length)
                         )
                    {
                        reslutMsg = $"最多只能【{length}】个字符。";
                    }
                }
            }
            if (!string.IsNullOrEmpty(reslutMsg) && propertyInfo != null)
            {
                reslutMsg = propertyInfo.GetDisplayName() + reslutMsg;
            }
            return (reslutMsg == "" ? true : false, reslutMsg, value);
        }
    }
}
