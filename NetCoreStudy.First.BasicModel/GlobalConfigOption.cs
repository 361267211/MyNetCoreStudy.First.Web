namespace NetCoreStudy.First.BasicModel
{

    /// <summary>
    /// 全局的配置-支持热加载-和配置中心匹配
    /// </summary>

    public static  class GlobalConfigOption
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public static DbContext DbContext { get; set; }
        public static DbContext FondDbContext { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public static Cap Cap { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static ServiceRegist ServiceRegist { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static GrpcRegist GrpcRegist { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static RedisServer RedisServer { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static JwtAuth JwtAuth { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static Exceptionless Exceptionless { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static Serilog Serilog { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static SM2Key SM2Key { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static string FabioUrl { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static OldSite OldSite { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static string ApiUrl { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static string GrpcUrl { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static string ApiPort { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static string GrpcPort { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public static AppBaseConfig AppBaseConfig { get; set; }
    }

    public class DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public string DbConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MigrationAssembly { get; set; }
    }

    public class RabbitMQ
    {
        /// <summary>
        /// 
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VirtualHost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }

    public class Cap
    {
        /// <summary>
        /// 
        /// </summary>
        public string TenantName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CapDbConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RabbitMQ RabbitMQ { get; set; }
    }

    public class ServiceRegist
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConsulAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 用户Api服务
        /// </summary>
        public string ServiceTags { get; set; }
    }

    public class GrpcRegist
    {
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConsulAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceTags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CloudUrl { get; set; }
    }

    public class RedisServer
    {
        /// <summary>
        /// 
        /// </summary>
        public string RedisConnection { get; set; }
    }

    public class JwtAuth
    {
        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SecKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ExpireSeconds { get; set; }
    }

    public class Exceptionless
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
    }

    public class Serilog
    {
        /// <summary>
        /// 
        /// </summary>
        public string MinLevel { get; set; }
    }

    public class SM2Key
    {
        /// <summary>
        /// 
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PublicKey { get; set; }
    }

    public class OldSite
    {
        /// <summary>
        /// 
        /// </summary>
        public string OldSiteUri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SiteUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Aid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }
    }

    public class AppBaseConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string RestApiGateway { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GrpcGateway { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string JwtIdentifyCenter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AppRouteCode { get; set; }
    }
 
}