/*********************************************************
* 名    称：SmartCapExtensions.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：用于生成Cap消息事件描述信息
* 更新历史：
*
* *******************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NetCoreStudy.Core.Cap
{
    /// <summary>
    /// 用于获取服务内所有发布事件描述信息
    /// </summary>
    public static class SmartCapExtensions
    {
        /// <summary>
        /// 替换Tenant名称
        /// </summary>
        /// <param name="oriStr"></param>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public static string ReplaceTenant(this string oriStr, string tenantName)
        {
            var splitArray = oriStr.Split(".", StringSplitOptions.None);
            if (splitArray.Length <= 0)
            {
                return "";
            }
            splitArray[0] = tenantName;
            return string.Join(".", splitArray);
        }

        /// <summary>
        /// 获取服务类所有事件描述信息
        /// </summary>
        /// <returns></returns>
        public static List<SmartCapEventIntroduction> CollectCapPublishEventIntroduction()
        {
            var assemblys = System.AppDomain.CurrentDomain.GetAssemblies();
            var intros = new List<SmartCapEventIntroduction>();
            foreach (var assembly in assemblys)
            {
                var publishEventTypes = assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(SmartCapPublishEventBase)) && x != typeof(SmartCapPublishEventBase));
                intros.AddRange(publishEventTypes.Select(x =>
                {
                    return CollectIntroduction(x);
                }));
            }
            return intros;
        }
        /// <summary>
        /// 获取某个事件描述信息
        /// </summary>
        /// <param name="capEventType"></param>
        /// <returns></returns>
        public static SmartCapEventIntroduction CollectIntroduction(Type capEventType)
        {
            var eventIntro = new SmartCapEventIntroduction();
            var eventType = capEventType;
            eventIntro.ServiceEventName = eventType.Name;
            var fieldInfos = eventType.GetFields().Where(x => x.IsLiteral);//获取所有常量字段
            foreach (var item in fieldInfos)
            {
                var descAttr = (DescriptionAttribute)Attribute.GetCustomAttribute(item, typeof(DescriptionAttribute));
                var eventDesc = new SmartCapEventDes
                {
                    Name = item.Name,
                    Value = item.GetValue(null).ToString(),
                    Description = descAttr?.Description,
                    BindMsg = new SmartCapEventMsg(),
                };
                var msgAttr = (SmartCapEventBindAttribute)Attribute.GetCustomAttribute(item, typeof(SmartCapEventBindAttribute));
                if (msgAttr != null && msgAttr.MsgType != null)
                {
                    var msgFields = msgAttr.MsgType.GetProperties().ToList().Select(x =>
                    {
                        var eventField = new SmartCapEventField
                        {
                            FieldName = x.Name,
                            FieldType = x.PropertyType.ToString(),
                        };
                        var descAttr = (DescriptionAttribute)Attribute.GetCustomAttribute(x, typeof(DescriptionAttribute));
                        eventField.FieldDesc = descAttr?.Description;
                        return eventField;
                    }).ToList();

                    var bindMsg = new SmartCapEventMsg
                    {
                        Name = msgAttr.MsgType.Name,
                        Fields = msgFields
                    };
                    eventDesc.BindMsg = bindMsg;
                }
                eventIntro.EventList.Add(eventDesc);
            }
            return eventIntro;
        }




    }
}
