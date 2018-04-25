namespace WebService.Controllers
{
    internal static class Routes
    {
        #region BASE REST CONSTROLLER TEMPLATES

        public const string Create = "";
        public const string AddItemToList = "{id:length(24)}/{propertyName}";

        public const string GetAll = "";
        public const string GetOne = "{id:length(24)}";
        public const string GetProperty = "{id:length(24)}/{propertyName}";

        public const string Update = "";
        public const string UpdateProperty = "{id:length(24)}/{propertyName}";

        public const string Delete = "{id:length(24)}";

        #endregion BASE REST CONSTROLLER TEMPLATES


        #region RECEIVER MODULES

        public const string GetOneByMac = "bymac/{mac}";
        public const string DeleteByMac = "bymac/{mac}";

        #endregion RECEIVER MODULES
        
        #region RESIDENTS

        public const string AddMusicData = "{residentId:length(24)}/Music/data";
        public const string AddVideoData = "{residentId:length(24)}/Videos/data";
        public const string AddImageData = "{residentId:length(24)}/Images/data";
        public const string AddColor = "{residentId:length(24)}/Colors/data";

        public const string AddMusicUrl = "{residentId:length(24)}/Music/url";
        public const string AddVideoUrl = "{residentId:length(24)}/Videos/url";
        public const string AddImageUrl = "{residentId:length(24)}/Images/url";

        public const string GetPicture = "{residentId:length(24)}/picture";
        public const string AddTag = "{residentId:length(24)}/tags";
        public const string GetByTag = "byTag/{tag}";
        public const string GetRandomElementFromProperty = "byTag/{tag}/{propertyName}/random";
        public const string GetPropertyByTag = "byTag/{tag}/{propertyName}";

        public const string UpdatePicture = "{id}/picture";

        public const string RemoveMusic = "{residentId:length(24)}/Music/{musicId}";
        public const string RemoveVideo = "{residentId:length(24)}/Videos/{videoId}";
        public const string RemoveImage = "{residentId:length(24)}/Images/{imageId}";
        public const string RemoveColor = "{residentId:length(24)}/Colors";

        public const string RemoveTag = "{residentId:length(24)}/{tag}";

        #endregion RESIDENTS
    }
}