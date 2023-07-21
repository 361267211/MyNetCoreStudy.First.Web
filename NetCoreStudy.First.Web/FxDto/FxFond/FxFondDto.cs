namespace NetCoreStudy.First.Web.FxDto.FxFond
{
    public class FxFondDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 支出方的Id
        /// </summary>
        public string ExContactId { get; set; }

        /// <summary>
        /// 收入方的Id
        /// </summary>
        public string InContactId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 活动金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public string FxFondEventId { get; set; }
    }
}
