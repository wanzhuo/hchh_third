using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ZRui.Web
{
    public class SettingBase : EntityBase
    {
        public string Flag { get; set; }
        public string GroupFlag { get; set; }
        public string Value { get; set; }
        public string Detail { get; set; }
        public SettingType SettingType { get; set; }
    }

    public enum SettingType
    {
        用户 = 0,
        系统 = 1
    }

    public static class SettingBaseDbContextExtention
    {
        public static T GetSettingValue<T>(this DbContext context, string flag)
        {
            return GetSettingValue<T>(context, flag, "", SettingType.用户);
        }

        public static T GetSettingValue<T>(this DbContext context, string flag, SettingType settingType)
        {
            return GetSettingValue<T>(context, flag, "", settingType);
        }

        public static T GetSettingValue<T>(this DbContext context, string flag, string groupFlag = "", SettingType settingType = SettingType.用户)
        {
            groupFlag = groupFlag ?? "";
            var value = context.Set<SettingBase>()
                .Where(m => m.GroupFlag == groupFlag)
                .Where(m => m.Flag == flag)
                .Where(m => m.SettingType == settingType)
                .Where(m => !m.IsDel)
                .Select(m => m.Value)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
        public static void SetSettingValue(this DbContext context, string flag, string value)
        {
            SetSettingValue(context, flag, value, "", SettingType.用户);
        }

        public static void SetSettingValue(this DbContext context, string flag, string value, string groupFlag)
        {
            SetSettingValue(context, flag, value, groupFlag, SettingType.用户);
        }

        public static void SetSettingValue(this DbContext context, string flag, string value, SettingType settingType)
        {
            SetSettingValue(context, flag, value, "", settingType);
        }
        public static void SetSettingValue(this DbContext context, string flag, string value, string groupFlag, SettingType settingType)
        {
            groupFlag = groupFlag ?? "";
            var model = context.Set<SettingBase>()
                .Where(m => m.GroupFlag == groupFlag)
                .Where(m => m.Flag == flag)
                .Where(m => !m.IsDel)
                .FirstOrDefault();
            if (model == null)
            {
                model = new SettingBase()
                {
                    GroupFlag = groupFlag,
                    Flag = flag,
                    SettingType = settingType
                };
                context.Add<SettingBase>(model);
            }

            model.Value = value;
            model.SettingType = SettingType.用户;
        }
    }
}
