using WebService.Models.Bases;

namespace WebAPIUnitTests.TestMocks
{
    public class MockEntity : AModelWithID
    {
        public string S { get; set; }
        public int I { get; set; }
        public bool B { get; set; }
    }
}