namespace WebAPIUnitTests.ControllerTests.ResidentsControllerTests
{
    public interface IResidentsControllerTests
    {
        #region CREATE

        // AddMusicAsync(string, MultiPartFile) is not necessary since it just calls AddMedia(string, MultiPartFile, EMediaType)
        // AddVideoAsync(string, MultiPartFile) is not necessary since it just calls AddMedia(string, MultiPartFile, EMediaType)
        // AddImageAsync(string, MultiPartFile) is not necessary since it just calls AddMedia(string, MultiPartFile, EMediaType)

        // AddMusicAsync(string, string) is not necessary since it just calls AddMedia(string, string, EMediaType)
        // AddVideoAsync(string, string) is not necessary since it just calls AddMedia(string, string, EMediaType)
        // AddImageAsync(string, string) is not necessary since it just calls AddMedia(string, string, EMediaType)

        void AddMediaNullId();
        void AddMediaBadId();
        void AddMediaNullData();
        void AddMediaNullFile();
        void AddMediaWithData();

        void AddMediaNullUrl();
        void AddMediaWithUrl();

        void AddNullColor();
        void AddColor();

        #endregion CREATE


        #region READ

        void GetByBadTag();
        void GetByTagNullProperties();
        void GetByTagEmptyProperties();
        void GetByTagBadProperties();
        void GetByTag();

        void GetRandomElementFromPropertyWithBadTag();
        void GetRandomElementFromPropertyNullPropertyName();
        void GetRandomElementFromPropertyBadPropertyName();
        void GetRandomElementFromProperty();

        void GetPropertyBadTag();
        void GetPropertyNullPropertyName();
        void GetPropertyBadPropertyName();
        void GetProperty();

        #endregion READ


        #region DELETE

        // RemoveMusicAsync(string, string) is not necessary since it just calls the RemoveMedia(string, string, EMediaType) method
        // RemoveVideoAsync(string, string) is not necessary since it just calls the RemoveMedia(string, string, EMediaType) method
        // RemoveImageAsync(string, string) is not necessary since it just calls the RemoveMedia(string, string, EMediaType) method

        void RemoveMediaNullResidentId();
        void RemoveMediaBadResidentId();
        void RemoveMediaNullMediaId();
        void RemoveMediaBadMediaId();
        void RemoveMedia();

        void RemoveColorNullResidentId();
        void RemoveColorBadResidentId();
        void RemoveColorNullColor();
        void RemoveColorBadColor();
        void RemoveColor();

        #endregion DELETE
    }
}