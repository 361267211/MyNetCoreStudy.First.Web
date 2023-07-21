using Castle.DynamicProxy;
//using Furion.DatabaseAccessor;
using Grpc.Core.Interceptors;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Distributed;
using NetCoreStudy.First.Common.FxCommonHelper;
using NetCoreStudy.First.Common.Redis;
using NetCoreStudy.First.Web.FxAttribute;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
//using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace NetCoreStudy.First.Web.AutofacIOC
{

    /// <summary>
    /// 使用autofac做的缓存装饰器:改装饰器用于删除缓存
    /// </summary>
    public class FxLogicCachingInterceptor : IInterceptor
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private protected CachingAttribute _cachingAttribute;
        private protected IDatabase _redisDb;
        private protected RedisMutex redisMutex;



        public FxLogicCachingInterceptor(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _connectionMultiplexer = connectionMultiplexer;
            _redisDb = _connectionMultiplexer.GetDatabase();

        }
        public void Intercept(IInvocation invocation)
        {

            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            _cachingAttribute = GetQCachingAttributeInfo(invocation.MethodInvocationTarget ?? invocation.Method);
            string redisKey = CustomCacheKey(invocation);
            this.redisMutex = new RedisMutex(_redisDb, $"lock:{redisKey}", "1");
            if (_cachingAttribute == null)
            {
                invocation.Proceed();//直接执行被拦截方法
                return;
            }

            string ParameterType = invocation.MethodInvocationTarget.ReturnType.Name;
            if (ParameterType == "Void" || ParameterType == "Task" || ParameterType == "ValueTask")
            {
                invocation.Proceed();//直接执行被拦截方法
            }
            else
            {
                bool isDefAsync = Attribute.IsDefined(invocation.MethodInvocationTarget, typeof(AsyncStateMachineAttribute));

                if (isDefAsync)
                {
                    //取异步方法的泛型参数类型
                    var generic = invocation.MethodInvocationTarget.ReturnType.GetGenericArguments()[0];
                    //通过反射获取异步缓存方法
                    MethodInfo mi = this.GetType().GetMethod("GetCacheAsync").MakeGenericMethod(new Type[] { generic });
                    //传入参数，执行method
                    mi.Invoke(this, new object[] { invocation, redisKey });
                }
                else
                {
                    //取异步方法的泛型参数类型
                    var generic = invocation.MethodInvocationTarget.ReturnType;
                    //通过反射获取异步缓存方法
                    MethodInfo mi = this.GetType().GetMethod("GetCacheSync").MakeGenericMethod(new Type[] { generic });
                    //传入参数，执行method
                    mi.Invoke(this, new object[] { invocation, redisKey });
                }
            }

        }

        #region 存、取缓存值
        public async Task GetCacheSync<T>(IInvocation invocation, string Name)
        {
            RedisData<T> Rcache = _cache.GetObject<RedisData<T>>(Name);
            //获取了非空缓存数据
            if (Rcache.Data != null)
            {
                //缓存不为空的时候将缓存结果给方法返回值，异步方法需要  Task.FromResult
                invocation.ReturnValue = Rcache.Data;
                return;
            }


            //缓存为空,执行原函数
            invocation.Proceed();

            //结果不为空是写入缓存
            _cache.SetObject(Name, (T)invocation.ReturnValue);
        }
        public async Task GetCacheAsync<T>(IInvocation invocation, string Name)
        {

            //第一次未获取缓存数据，尝试去加锁 自旋重试
            bool setnxSuccess;
            RedisData<T> Rcache = _cache.GetObject<RedisData<T>>(Name);

            do
            {
                Rcache = _cache.GetObject<RedisData<T>>(Name);

                if (Rcache != null)//缓存已被创建过，且有值
                {
                    if (TimeSpan.Compare(Rcache.LogicExpireTimeSpan, TimeSpan.FromTicks(DateTime.Now.Ticks)) == -1) //校验是否逻辑过期
                    {
                        //开启新的线程，执行正常逻辑重建数据
                        Task.Run(async () =>
                        {
                            //缓存过期，重新执行业务，重建缓存
                            invocation.Proceed();

                            //结果写入缓存
                            await SetCacheAsync((dynamic)invocation.ReturnValue, Name);
                        });
                    }

                    //缓存不为空的时候将缓存结果给方法返回值，异步方法需要  Task.FromResult
                    invocation.ReturnValue = Task.FromResult(Rcache.Data);
                    return;
                }

                setnxSuccess = this.redisMutex.AcquireLock();

                if (!setnxSuccess)
                {
                    Thread.Sleep(500);//休眠500ms
                }


            } while (!setnxSuccess);




            //缓存不存在，首次执行业务逻辑，获取结果
            invocation.Proceed();

            //结果不为空是写入缓存
            await SetCacheAsync((dynamic)invocation.ReturnValue, Name);
        }

        /// <summary>
        /// 将结果写入缓存
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task SetCacheAsync<TResult>(Task<TResult> task, string name)
        {
            var data = await task.ConfigureAwait(false);
            RedisData<TResult> Rcache = new RedisData<TResult>(data: data);

            await _cache.SetObjectAsync(name, Rcache);


            this.redisMutex.ReleaseLock();//释放锁
        }

        #endregion

        #region 处理Argument，返回key
        /// <summary>
        /// 自定义缓存的key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();//获取参数列表，最多三个

            string key = $"{typeName}:{methodName}:Resource:{_cachingAttribute.ResourceName}:";
            foreach (var param in methodArguments)
            {
                key = $"{key}{param}:";
            }

            return key.TrimEnd(':');
        }
        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected static string GetArgumentValue(object arg)
        {
            if (arg is DateTime || arg is DateTime?)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            if (arg is string || arg is ValueType || arg is Nullable)
                return arg.ToString();

            if (arg != null)
            {
                if (arg is Expression)
                {
                    var obj = arg as Expression;
                    var result = Resolve(obj);
                    return MD5Helper.MD5Encrypt16(result);
                }
                else if (arg.GetType().IsClass)
                {
                    return MD5Helper.MD5Encrypt16(Newtonsoft.Json.JsonConvert.SerializeObject(arg));
                }
            }
            return string.Empty;
        }

        private static string Resolve(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                expression = lambda.Body;
                return Resolve(expression);
            }
            if (expression is BinaryExpression)
            {
                BinaryExpression binary = expression as BinaryExpression;
                if (binary.Left is MemberExpression && binary.Right is ConstantExpression)//解析x=>x.Name=="123" x.Age==123这类
                    return ResolveFunc(binary.Left, binary.Right, binary.NodeType);
                if (binary.Left is MethodCallExpression && binary.Right is ConstantExpression)//解析x=>x.Name.Contains("xxx")==false这类的
                {
                    object value = (binary.Right as ConstantExpression).Value;
                    return ResolveLinqToObject(binary.Left, value, binary.NodeType);
                }
                if ((binary.Left is MemberExpression && binary.Right is MemberExpression)
                    || (binary.Left is MemberExpression && binary.Right is UnaryExpression))//解析x=>x.Date==DateTime.Now这种
                {
                    LambdaExpression lambda = Expression.Lambda(binary.Right);
                    Delegate fn = lambda.Compile();
                    ConstantExpression value = Expression.Constant(fn.DynamicInvoke(null), binary.Right.Type);
                    return ResolveFunc(binary.Left, value, binary.NodeType);
                }
            }
            if (expression is UnaryExpression)
            {
                UnaryExpression unary = expression as UnaryExpression;
                if (unary.Operand is MethodCallExpression)//解析!x=>x.Name.Contains("xxx")或!array.Contains(x.Name)这类
                    return ResolveLinqToObject(unary.Operand, false);
                if (unary.Operand is MemberExpression && unary.NodeType == ExpressionType.Not)//解析x=>!x.isDeletion这样的 
                {
                    ConstantExpression constant = Expression.Constant(false);
                    return ResolveFunc(unary.Operand, constant, ExpressionType.Equal);
                }
            }
            if (expression is MemberExpression && expression.NodeType == ExpressionType.MemberAccess)//解析x=>x.isDeletion这样的 
            {
                MemberExpression member = expression as MemberExpression;
                ConstantExpression constant = Expression.Constant(true);
                return ResolveFunc(member, constant, ExpressionType.Equal);
            }
            if (expression is MethodCallExpression)//x=>x.Name.Contains("xxx")或array.Contains(x.Name)这类
            {
                MethodCallExpression methodcall = expression as MethodCallExpression;
                return ResolveLinqToObject(methodcall, true);
            }
            var body = expression as BinaryExpression;
            //已经修改过代码body应该不会是null值了
            if (body == null)
                return string.Empty;
            var Operator = GetOperator(body.NodeType);
            var Left = Resolve(body.Left);
            var Right = Resolve(body.Right);
            string Result = string.Format("({0} {1} {2})", Left, Operator, Right);
            return Result;
        }

        private static string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                    return "and";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new Exception(string.Format("不支持{0}此种运算符查找！" + expressiontype));
            }
        }

        private static string ResolveFunc(Expression left, Expression right, ExpressionType expressiontype)
        {
            var Name = (left as MemberExpression).Member.Name;
            var Value = (right as ConstantExpression).Value;
            var Operator = GetOperator(expressiontype);
            return Name + Operator + Value ?? "null";
        }

        private static string ResolveLinqToObject(Expression expression, object value, ExpressionType? expressiontype = null)
        {
            var MethodCall = expression as MethodCallExpression;
            var MethodName = MethodCall.Method.Name;
            switch (MethodName)
            {
                case "Contains":
                    if (MethodCall.Object != null)
                        return Like(MethodCall);
                    return In(MethodCall, value);
                case "Count":
                    return Len(MethodCall, value, expressiontype.Value);
                case "LongCount":
                    return Len(MethodCall, value, expressiontype.Value);
                default:
                    throw new Exception(string.Format("不支持{0}方法的查找！", MethodName));
            }
        }

        private static string In(MethodCallExpression expression, object isTrue)
        {
            var Argument1 = (expression.Arguments[0] as MemberExpression).Expression as ConstantExpression;
            var Argument2 = expression.Arguments[1] as MemberExpression;
            var Field_Array = Argument1.Value.GetType().GetFields().First();
            object[] Array = Field_Array.GetValue(Argument1.Value) as object[];
            List<string> SetInPara = new List<string>();
            for (int i = 0; i < Array.Length; i++)
            {
                string Name_para = "InParameter" + i;
                string Value = Array[i].ToString();
                SetInPara.Add(Value);
            }
            string Name = Argument2.Member.Name;
            string Operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
            string CompName = string.Join(",", SetInPara);
            string Result = string.Format("{0} {1} ({2})", Name, Operator, CompName);
            return Result;
        }

        private static string Like(MethodCallExpression expression)
        {

            var Temp = expression.Arguments[0];
            LambdaExpression lambda = Expression.Lambda(Temp);
            Delegate fn = lambda.Compile();
            var tempValue = Expression.Constant(fn.DynamicInvoke(null), Temp.Type);
            string Value = string.Format("%{0}%", tempValue);
            string Name = (expression.Object as MemberExpression).Member.Name;
            string Result = string.Format("{0} like {1}", Name, Value);
            return Result;
        }

        private static string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
        {
            object Name = (expression.Arguments[0] as MemberExpression).Member.Name;
            string Operator = GetOperator(expressiontype);
            string Result = string.Format("len({0}){1}{2}", Name, Operator, value.ToString());
            return Result;
        }
        #endregion

        private CachingAttribute GetQCachingAttributeInfo(MethodInfo method)
        {
            return method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
        }
    }
}

