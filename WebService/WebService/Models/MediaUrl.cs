using WebService.Models.Bases;

namespace WebService.Models
{
    public class MediaUrl : AModelWithID
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
    }
}