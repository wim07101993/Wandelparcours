using WebService.Models.Bases;

namespace WebService.Tests.TestModels
{
    public class TestEntity : AModelWithID
    {
        public string S { get; set; }
        public int I { get; set; }
        public bool B { get; set; }
    }
}