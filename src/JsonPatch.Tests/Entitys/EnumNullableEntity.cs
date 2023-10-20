namespace JsonPatch.Tests.Entitys
{
    public class EnumNullableEntity
    {
        public SampleNullableEnum? Foo { get; set; }
    }

    public enum SampleNullableEnum
    {
        FirstEnum,
        SecondEnum,
        ThirdEnum,
    }
}
