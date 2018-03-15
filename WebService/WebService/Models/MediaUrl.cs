using System.Collections.Generic;
using System.Linq;
using WebService.Controllers;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class MediaUrl : AModelWithID
    {
        private string _url;

        public string Url
        {
            get => _url ?? typeof(MediaController)
                       .GetMethods()
                       .FirstOrDefault(x => x.Name == nameof(MediaController.GetAsync) && x.GetParameters().Length == 2)
                       .GetUrl<MediaController>(new Dictionary<string, string> {{"id", Id.ToString()}});
            set => _url = value;
        }
    }
}