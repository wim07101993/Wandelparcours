using System;

namespace WebService.Controllers
{
    internal static class Routes
    {
        private const string PropertyName = "{propertyName}";

        private const string ById = "{id:length(24)}";
        private const string PropertyById = ById + "/" + PropertyName;


        public static class RestBase
        {
            internal const string Route = "api/v1";

            public const string Create = "";
            public const string AddItemToList = PropertyById;

            public const string GetAll = "";
            public const string GetOne = ById;
            public const string GetProperty = PropertyById;

            public const string Update = "";
            public const string UpdateProperty = PropertyById;

            public const string Delete = ById;
        }


        public static class Locations
        {
            public const string Route = "api/v1/[controller]";

            public const string GetAllLastLocations = "lastlocations";

            [Obsolete]
            public const string RouteOld = "api/v1/location";

            [Obsolete]
            public const string GetLastLocationOneResident = "residents/{id:length(24)}/lastlocation";

            [Obsolete]
            public const string GetlastLocationOneResidentByTag = "bytag/residents/{tag}/lastlocation";

            [Obsolete]
            public const string GetAllLastLocationsOld = "residents/lastlocation";

            [Obsolete]
            public const string SetLastLocation = "{id:length(24)}/lastlocation";

            [Obsolete]
            public const string SetlastLocationByTag = "{tag}/lastlocation/bytag";
        }


        public static class ReceiverModules
        {
            public const string Route = RestBase.Route + "/[Controller]";

            private const string MacAddressRegex = @"^([[A-f0-9]]{{2}}[[:-\\.]]){{5}}[[A-f0-9]]{{2}}$";
            private const string ByMac = "{mac:regex(" + MacAddressRegex + ")}";

            public const string GetOneByMac = ByMac;
            public const string DeleteByMac = ByMac;

            [Obsolete]
            public const string DeleteByMacOld = "bymac/" + ByMac;

            [Obsolete]
            public const string GetOneByMacOld = "bymac/" + ByMac;
        }


        public static class Residents
        {
            public const string Route = RestBase.Route + "/[Controller]";

            private const string ByTag = "{tag}";
            private const string PropertyByTag = ByTag + "/" + PropertyName;

            public const string AddMusicData = ById + "/Music/data";
            public const string AddVideoData = ById + "/Videos/data";
            public const string AddImageData = ById + "/Images/data";
            public const string AddColor = ById + "/Colors/data";

            public const string AddMusicUrl = ById + "/Music/url";
            public const string AddVideoUrl = ById + "/Videos/url";
            public const string AddImageUrl = ById + "/Images/url";

            public const string GetPicture = ById + "/picture";
            public const string AddTag = ById + "/tags";
            public const string GetByTag = ByTag;

            public const string GetRandomElementFromProperty = PropertyByTag + "/random";
            public const string GetPropertyByTag = PropertyByTag;
            public const string UpdatePicture = ById + "/picture";

            public const string RemoveMusic = ById + "/Music/{musicId}";
            public const string RemoveVideo = ById + "/Videos/{videoId}";
            public const string RemoveImage = ById + "/Images/{imageId}";
            public const string RemoveColor = ById + "/Colors";

            public const string RemoveTag = ById + "/{tag}";

            [Obsolete]
            private const string ByTagOld = "byTag/" + ByTag;

            [Obsolete]
            private const string PropertyByTagOld = ByTagOld + "/" + PropertyName;

            [Obsolete]
            public const string GetByTagOld = ByTagOld;

            [Obsolete]
            public const string GetRandomElementFromPropertyOld = PropertyByTagOld + "/random";

            [Obsolete]
            public const string GetPropertyByTagOld = PropertyByTagOld;
        }
    }
}