namespace WebAPIUnitTests.ControllerTests.ResidentsControllerTests
{
    public interface IResidentsControllerTests
    {
        #region CREATE

        // CreateAsync(Resident) is not necessary since it just calls the base method

        // AddMusicAsync(string, MultiPartFile) is not necessary since it just calls AddMedia(string, MultiPartFile, EMediaType)
        // AddMusicAsync(string, string) is not necessary since it just calls AddMedia(string, string, EMediaType)

        // AddVideoAsync(string, MultiPartFile) is not necessary since it just calls AddMedia(string, MultiPartFile, EMediaType)
        // AddVideoAsync(string, string) is not necessary since it just calls AddMedia(string, string, EMediaType)

        // AddImageAsync(string, MultiPartFile) is not necessary since it just calls AddMedia(string, MultiPartFile, EMediaType)
        // AddImageAsync(string, string) is not necessary since it just calls AddMedia(string, string, EMediaType)

        void AddColorNullIdNullData();
        void AddColorNullIdWithData();

        void AddColorBadIdNullData();
        void AddColorBadIdWithData();

        void AddColorExistingIdNullData();
        void AddColorExistingIdWithData();


        void AddMediaNullIdNullData();
        void AddMediaNullIdNullFile();
        void AddMediaNullIdWithData();

        void AddMediaBadIdNullData();
        void AddMediaBadIdNullFile();
        void AddMediaBadIdWithData();

        void AddMediaExistingIdNullData();
        void AddMediaExistingIdNullFile();
        void AddMediaExistingIdWithData();


        void AddMediaNullIdNullUrl();
        void AddMediaNullIdWithUrl();

        void AddMediaBadIdNullUrl();
        void AddMediaBadIdWithUrl();

        void AddMediaExistingIdNullUrl();
        void AddMediaExistingIdWithUrl();

        #endregion CREATE


        #region READ

        // GetAsync(string[]) is not necessary since it just calls the base method
        // GetAsync(string, string[]) is not necessary since it just calls the base method
        // GetAsync(int, string[]) is not necessary since it just calls the base method
        // GetPropertyAsync(string, string) is not necessary since it just calls the base method

        void GetRandomMediaBadTagNullMediaType();
        void GetRandomMediaBadTagBadMediaType();
        void GetRandomMediaBadTagExistingMediaType();

        void GetRandomMediaExistingTagNullMediaType();
        void GetRandomMediaExistingTagBadMediaType();
        void GetRandomMediaExistingTagExistingMediaTyp();

        #endregion READ


        #region UPDATE

        void UpdateNullItemNullProperties();
        void UpdateNullItemEmptyProperties();
        void UpdateNullItemSomeProperties();

        void UpdateBadItemNullProperties();
        void UpdateBadItemEmptyProperties();
        void UpdateBadItemSomeProperties();

        void UpdateExistingItemNullProperties();
        void UpdateExistingItemEmptyProperties();
        void UpdateExistingItemSomeProeprties();

        #endregion UPDATE


        #region DELETE

        // DeleteAsync(string) is not necessary since it just calls the base method
        // RemoveVideoAsync(string, string) is not necessary since it just calls the RemoveMedia(string, string, EMediaType) method
        // RemoveMusicAsync(string, string) is not necessary since it just calls the RemoveMedia(string, string, EMediaType) method
        // RemoveImageAsync(string, string) is not necessary since it just calls the RemoveMedia(string, string, EMediaType) method


        void RemoveColorNullResidentNullColor();
        void RemoveColorNullResidentBadColor();
        void RemoveColorNullResidentExistingColor();

        void RemoveColorBadResidentNullColor();
        void RemoveColorBadResidentBadColor();
        void RemoveColorBadResidentExistingColor();

        void RemoveColorExistingResidentNullColor();
        void RemoveColorExistingResidentBadColor();
        void RemoveColorExistingResidentExistingColor();


        void RemoveMediaNullResidentNullMedia();
        void RemoveMediaNullResidentBadMedia();
        void RemoveMediaNullResidentExistingMedia();

        void RemoveMediaBadResidentNullMedia();
        void RemoveMediaBadResidentBadMedia();
        void RemoveMediaBadResidentExistingMedia();

        void RemoveMediaExistingResidentNullMedia();
        void RemoveMediaExistingResidentBadMedia();
        void RemoveMediaExistingResidentExistingMedia();

        #endregion DELETE
    }
}