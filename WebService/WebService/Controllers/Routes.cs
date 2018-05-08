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
            internal const string ControllerRoute = Route + "/[controller]";

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
            public const string GetAllLastLocations = "lastlocations";
        }

        public static class Media
        {
            public const string GetOneFileWithExtension = ById + "/file.{extension}";
            public const string GetFile = ById + "/file";
        }

        public static class ReceiverModules
        {
            // unused
            private const string MacAddressRegex = @"^([[A-f0-9]]{{2}}[[:-\\.]]){{5}}[[A-f0-9]]{{2}}$";
            // unused
            private const string ByMac = "{mac:regex(" + MacAddressRegex + ")}";
            private const string ByName = "byName/{name}";

            public const string GetOneByName = ByName;
            public const string DeleteByName = ByName;
        }

        public static class Residents
        {
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
            public const string UpdateLastRecordedPosition = ByTag + "/lastRecordedPosition";

            public const string RemoveMusic = ById + "/Music/{musicId}";
            public const string RemoveVideo = ById + "/Videos/{videoId}";
            public const string RemoveImage = ById + "/Images/{imageId}";
            public const string RemoveColor = ById + "/Colors";

            public const string RemoveTag = ById + "/{tag}";
        }

        public static class Tokens
        {
            public const string CreateTokenTemplate = "";
        }

        public static class Users
        {
            public const string GetByName = "{userName}";
            public const string GetPropertyByName = "{userName}/{propertyName}";
        }
    }
}