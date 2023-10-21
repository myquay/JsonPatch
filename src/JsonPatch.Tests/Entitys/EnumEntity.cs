namespace JsonPatch.Tests.Entities
{
    public class EnumEntity
    {
        public SampleEnum Foo { get; set; }
    }

    public enum SampleEnum
    {
        FirstEnum,
        SecondEnum,
        ThirdEnum,
    }
}
