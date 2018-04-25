using System;

namespace WebService.Controllers
{
    internal static class Routes
    {
        private const string PropertyName = "{propertyName}";

        private const string ById = "{id:length(24)}";
        private const string PropertyById = ById + "/" + PropertyName;


        #region BASE REST CONSTROLLER TEMPLATES

        private const string BaseController = "api/v1";

        public const string Create = "";
        public const string AddItemToList = PropertyById;

        public const string GetAll = "";
        public const string GetOne = ById;
        public const string GetProperty = PropertyById;

        public const string Update = "";
        public const string UpdateProperty = PropertyById;

        public const string Delete = ById;

        #endregion BASE REST CONSTROLLER TEMPLATES


        #region RECEIVER MODULES

        public const string ReceiverModulesController = BaseController + "/[Controller]";

        private const string MacAddressRegex = @"^([[A-f0-9]]{{2}}[[:-\\.]]){{5}}[[A-f0-9]]{{2}}$";
        private const string ByMac = "{mac:regex(" + MacAddressRegex + ")}";

        public const string GetOneByMac = ByMac;

        [Obsolete]
        public const string GetOneByMacOld = "bymac/" + ByMac;

        public const string DeleteByMac = ByMac;

        [Obsolete]
        public const string DeleteByMacOld = "bymac/" + ByMac;

        #endregion RECEIVER MODULES


        #region RESIDENTS

        public const string ResidentsModulesController = BaseController + "/[Controller]";

        private const string ByTag = "{tag}";

        [Obsolete]
        private const string ByTagOld = "byTag/" + ByTag;

        private const string PropertyByTag = ByTag + "/" + PropertyName;
        private const string PropertyByTagOld = ByTagOld + "/" + PropertyName;


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

        [Obsolete]
        public const string GetByTagOld = ByTagOld;

        public const string GetRandomElementFromProperty = PropertyByTag + "/random";

        [Obsolete]
        public const string GetRandomElementFromPropertyOld = PropertyByTagOld + "/random";

        public const string GetPropertyByTag = PropertyByTag;

        [Obsolete]
        public const string GetPropertyByTagOld = PropertyByTagOld;

        public const string UpdatePicture = ById + "/picture";

        public const string RemoveMusic = ById + "/Music/{musicId}";
        public const string RemoveVideo = ById + "/Videos/{videoId}";
        public const string RemoveImage = ById + "/Images/{imageId}";
        public const string RemoveColor = ById + "/Colors";

        public const string RemoveTag = ById + "/{tag}";

        #endregion RESIDENTS
    }
}