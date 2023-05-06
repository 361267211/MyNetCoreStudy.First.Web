/*********************************************************
 * 名    称：OptionRegister
 * 作    者：张祖琪
 * 联系方式：电话[13883914813],邮件[361267211@qq.com]
 * 创建时间：2021/8/05 16:57:45
 * 描    述：拓展方法，将配置中心的配置项绑定到全局静态变量 SiteGlobalConfig上
 *
 * 更新历史：
 *
 * *******************************************************/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NetCoreStudy.Core;
using NetCoreStudy.Core.Apollo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ApolloOption
{
    public static class OptionRegister
    {
        /// <summary>
        /// 初始化全局配置项并绑定Change事件 Apollo
        /// 将Option绑定到全局变量中
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <param name="ConfigType"></param>
        public static void ConfigInitAndBindChange(this IServiceCollection services, IConfiguration Configuration, Type ConfigType)
        {
            //根据本地SiteGlobalConfig的属性进行绑定
            PropertyInfo[] properties = ConfigType.GetProperties(/*System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public*/);

            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    //带有自定义特性的属性不进行赋值
                    if (property.GetCustomAttributes(typeof(IgnoreMappingAttribute), true).Length > 0)
                        continue;

                    //1.提取SiteGlobalConfig节点
                    var propertyType = property.PropertyType;
                    var propertyTypeName = property.Name;

                    //var method = typeof(ValueBinder.Microsoft.Extensions.DependencyInjection.ConfigureValueExtensions).GetMethod(nameof(ValueBinder.Microsoft.Extensions.DependencyInjection.ConfigureValueExtensions.ConfigureJsonValue), new Type[] { services.GetType(), typeof(IConfigurationSection) }).MakeGenericMethod(new Type[] { propertyType });
                    //method.Invoke(services, new object[] { services, Configuration.GetSection(propertyTypeName) });

                    //3.绑定配置值变化的事件
                    var baseMethod = typeof(OptionRegister).GetMethod(nameof(OptionRegister.BindOnchangeEvent));//提取基础方法
                    var genericMethod = baseMethod.MakeGenericMethod(new Type[] { propertyType });//绑定泛型
                    genericMethod.Invoke(services, new object[] { services, Configuration, ConfigType, propertyTypeName });//调用泛型方法

                }
            }
        }

        /// <summary>
        /// 赋予初值和绑定onchang事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <param name="ConfigType"></param>
        /// <param name="propertyTypeName"></param>
        /// <returns></returns>
        public static IServiceCollection BindOnchangeEvent<T>(this IServiceCollection services, IConfiguration Configuration, Type ConfigType, string propertyTypeName) // where T :class
        {
            //var at = Configuration.GetSection(propertyTypeName).Get(typeof(T));
            var optionValue = Configuration.GetSection(propertyTypeName).Value;

            if (optionValue == string.Empty || optionValue == "" || optionValue == null)
            {
                Console.WriteLine("本地配置节点多于APOLLO配置中心,请注意排查");
                return services;
            }

            if (typeof(T).Name == nameof(System.String))//字符类型特殊处理
            {
                var opt = optionValue;
                ConfigType.GetProperty(propertyTypeName).SetValue(typeof(T), opt, null);//siteglobalconfig 首次赋值
                Configuration.Bind(opt);//绑定OPTION                      
                                        //  services.Configure<T>(Configuration.GetSection(typeof(T).Name));
                ChangeToken.OnChange(
                    () =>
                    {
                        IChangeToken changeToken = Configuration.GetReloadToken();
                        return changeToken;
                    }
                    ,
                         () =>
                         {
                             Console.WriteLine("检测到文件夹有变化!");
                             var configValue = Configuration.GetSection(propertyTypeName).Value;
                             Configuration.Bind(configValue);
                             ConfigType.GetProperty(propertyTypeName).SetValue(typeof(T), configValue, null);//siteglobalconfig 监听事件赋值
                         }
                            );
                return services;
            }
            else
            {
                T opt = JsonConvert.DeserializeObject<T>(optionValue);
                //var jobj=typeof(JsonConvert).GetMethod(nameof(JsonConvert.DeserializeObject), 1, new Type[] { typeof(string) }); //反射调用泛型方法的示例
                ConfigType.GetProperty(propertyTypeName).SetValue(typeof(T), opt, null);//siteglobalconfig 首次赋值
                Configuration.Bind(opt);//绑定OPTION
                                        //  services.Configure<T>(Configuration.GetSection(typeof(T).Name));
                ChangeToken.OnChange(
                    () =>
                    {
                        IChangeToken changeToken = Configuration.GetReloadToken();
                        return changeToken;
                    }
                    ,
                         () =>
                         {
                             Console.WriteLine("检测到文件夹有变化!");
                             T configValue = JsonConvert.DeserializeObject<T>(Configuration.GetSection(propertyTypeName).Value);
                             Configuration.Bind(configValue);
                             // ConfigType.GetProperty(propertyTypeName).SetValue(typeof(T), configValue, null);//siteglobalconfig 监听事件赋值
                             ConfigType.GetProperty(propertyTypeName).SetValue(typeof(T), configValue, null);//siteglobalconfig 监听事件赋值
                         }
                            );
                return services;
            }
        }


        /// <summary>
        /// Consul配置中心的监听赋值事件
        /// </summary>
        public static void ConsulConfigInit(IConfigurationRoot configRoot, Type ConfigType)
        {
            //根据本地SiteGlobalConfig的属性进行绑定
            System.Reflection.PropertyInfo[] properties = ConfigType.GetProperties(/*System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public*/);
            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    //带有自定义特性的属性不进行赋值
                    if (property.GetCustomAttributes(typeof(IgnoreMappingAttribute), true).Length > 0)
                        continue;

                    //1.提取SiteGlobalConfig节点信息
                    var propertyType = property.PropertyType;  //取属性的类型
                    var propertyName = property.Name;  //取属性的名称
                    var configPath = propertyName;  //第一级的配置节点等于属性的名称

                    //2.调用方法取配置中心的值
                    var baseMethod = typeof(OptionRegister).GetMethod(nameof(OptionRegister.ConfigInit));//提取基础方法
                    var genericMethod = baseMethod.MakeGenericMethod(new Type[] { propertyType });//创建泛型方法，根据当前属性类型绑定泛型
                    var re = genericMethod.Invoke(null, new object[] { configRoot, propertyType, configPath });//调用递归方法，取得属性的值

                    //3.为全局SiteGlobalConfig赋值
                    ConfigType.GetProperty(propertyName).SetValue(ConfigType, re, null);//  赋值到 siteglobalconfig 对应的节点上
                }
            }
        }
        /// <summary>
        /// 递归获取节点值，向上传递
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configRoot"></param>
        /// <param name="propertyType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T ConfigInit<T>(IConfigurationRoot configRoot, Type propertyType, string path)
        {
            switch (propertyType.Name)
            {
                case nameof(String):
                    Object strObj = configRoot.GetSection(path).Value;
                    return (T)strObj;
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                    Object intObj = int.Parse(configRoot.GetSection(path).Value ?? "0");
                    return (T)intObj;
                case nameof(Boolean):
                    Object boolObj = Convert.ToBoolean(configRoot.GetSection(path).Value);
                    return (T)boolObj;
                case nameof(DateTime):
                    Object dateObj = Convert.ToDateTime(configRoot.GetSection(path).Value);
                    return (T)dateObj;
                default:
                    break;
            }
            //获取属性列表
            System.Reflection.PropertyInfo[] properties = propertyType.GetProperties(/*System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public*/);
            System.Type t = typeof(T);

            //是否是generic类的集合
            if (propertyType.GetGenericArguments().Length > 0)
            {
                var listConfig = configRoot.GetSection(path);
                var lists = listConfig.GetChildren();
                var Count = 0;
                foreach (var item in lists)
                {
                    Count++;
                }

                //获取list成员类型
                var listType = propertyType.GetGenericArguments()[0];
                var modelList = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { listType }));
                for (int i = 0; i < Count; i++)
                {               
                    var listPath = $"{path}:{i}";
                    //3.调用泛型方法递归赋值
                    var baseMethod = typeof(OptionRegister).GetMethod(nameof(OptionRegister.ConfigInit));//提取基础方法
                    var genericMethod = baseMethod.MakeGenericMethod(new Type[] { listType });//绑定泛型
                    Object result = genericMethod.Invoke(null, new object[] { configRoot, listType, listPath });//调用泛型方法         

                    var addMethod = modelList.GetType().GetMethod("Add");//调用list的Add方法添加成员
                    addMethod.Invoke(modelList, new object[] { result });
                }

                return (T)((Object)modelList);
            }


            //数组情况最特殊
            if (t.IsArray)
            {
                var elType = t.GetArrayElementType();

                var arrConfigRoot = configRoot.GetSection(path);
                var arrList = arrConfigRoot.GetChildren();
                var arrCount = 0;
                foreach (var el in arrList)
                {
                    arrCount++;
                }
                var arr = Array.CreateInstance(elType, arrCount);
                // arrList.GetEnumerator()
                foreach (var el in arrList)
                {
                    var arrElTypeName = el.Value;//数组成员的全名
                    var arrIndex = el.Key;//数组成员的下标
                    var elPath = path + ":" + arrIndex;

                    var baseMethod = typeof(OptionRegister).GetMethod(nameof(OptionRegister.ConfigInit));//提取基础方法
                    var genericMethod = baseMethod.MakeGenericMethod(new Type[] { elType });//绑定泛型
                    Object result = genericMethod.Invoke(null, new object[] { configRoot, elType, elPath });//调用泛型方法
                    arr.SetValue(result, int.Parse(arrIndex));
                }
                return (T)((Object)arr);
            }

            Assembly ass = Assembly.GetAssembly(t);//获取泛型的程序集
            Object _obj = null;
            _obj = Activator.CreateInstance<T>();// 或者这样也可以实例化
            foreach (var property in properties)
            {
                //1.提取SiteGlobalConfig节点
                var nextPropertyType = property.PropertyType;
                var nextpropertyTypeName = property.Name;
                var configPath = path + ":" + nextpropertyTypeName;
                switch (nextpropertyTypeName)
                {
                    case nameof(String):

                        var str = configRoot.GetSection(configPath).Value;
                        property.SetValue((T)_obj, str, null);//  赋值到 siteglobalconfig 对应的节点上
                        break;
                    case nameof(Int16):
                    case nameof(Int32):
                    case nameof(Int64):
                        _obj = Activator.CreateInstance<T>();// 或者这样也可以实例化
                        int num = int.Parse(configRoot.GetSection(configPath).Value);
                        property.SetValue((T)_obj, num, null);
                        break;
                    case nameof(Boolean):
                        _obj = Activator.CreateInstance<T>();// 或者这样也可以实例化
                        var bo = Convert.ToBoolean(configRoot.GetSection(configPath).Value);
                        property.SetValue((T)_obj, bo, null);//  赋值到 siteglobalconfig 对应的节点上
                        break;
                    case nameof(DateTime):
                        _obj = Activator.CreateInstance<T>();// 或者这样也可以实例化
                        var time = Convert.ToDateTime(configRoot.GetSection(configPath).Value);
                        property.SetValue((T)_obj, time, null);//  赋值到 siteglobalconfig 对应的节点上
                        break;
                    default:

                        //3.调用泛型方法递归赋值
                        var baseMethod = typeof(OptionRegister).GetMethod(nameof(OptionRegister.ConfigInit));//提取基础方法
                        var genericMethod = baseMethod.MakeGenericMethod(new Type[] { nextPropertyType });//绑定泛型
                        Object result = genericMethod.Invoke(null, new object[] { configRoot, nextPropertyType, configPath });//调用泛型方法
                        property.SetValue((T)_obj, result, null);//  赋值到 siteglobalconfig 对应的节点上
                        break;
                }
            }
            return (T)_obj;
        }

        /// <summary>
        /// 获取数组成员的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetArrayElementType(this Type type)
        {
            if (!type.IsArray)
            {
                return null;
            }
            string elTypeName = type.FullName.Replace("[]", string.Empty);
            Type elType = type.Assembly.GetType(elTypeName);
            return elType;
        }
    }
}
