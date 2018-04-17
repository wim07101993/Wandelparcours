using WebService.Models.Bases;

namespace WebAPIUnitTests.TestModels
{
    public class TestEntity : AModelWithID
    {
        public string S { get; set; }
        public int I { get; set; }
        public bool B { get; set; }
    }
}