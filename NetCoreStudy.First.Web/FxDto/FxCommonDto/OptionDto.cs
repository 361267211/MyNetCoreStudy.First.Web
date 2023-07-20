namespace NetCoreStudy.First.Web.FxDto.FxCommonDto
{
    public class OptionDto
    {
        public OptionDto(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; set; }
        public string Value { get; set; }
    }
}
