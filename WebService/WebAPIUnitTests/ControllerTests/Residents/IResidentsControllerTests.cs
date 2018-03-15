using WebAPIUnitTests.ControllerTests.Abstract;

namespace WebAPIUnitTests.ControllerTests.Residents
{
    public interface IResidentsControllerTests : IRestControllerTest
    {
        #region AddMedia

        void AddNullFormFileMediaToNullId();
        void AddNullFormFileMediaToUnknownId();
        void AddNullFormFileMediaToWrongFormatId();
        void AddNullFormFileMediaToKnownId();

        void AddFormFileWithNoFileMediaToNullId();
        void AddFormFileWithNoFileMediaToUnknownId();
        void AddFormFileWithNoFileMediaToWrongFormatId();
        void AddFormFileWithNoFileMediaToKnownId();

        void AddFormFileMediaToNullId();
        void AddFormFileMediaToUnknownId();
        void AddFormFileMediaToWrongFormatId();
        void AddFormFileMediaToKnownId();


        void AddNullUrlMediaToNullId();
        void AddNullUrlMediaToUnknownId();
        void AddNullUrlMediaToWrongFormatId();
        void AddNullUrlMediaToKnownId();

        void AddUrlMediaToNullId();
        void AddUrlMediaToUnknownId();
        void AddUrlMediaToWrongFormatId();
        void AddUrlMediaToKnownId();

        #endregion AddMedia


        #region GetByTag

        void GetUnknownTagWithNullProperties();
        void GetUnknownTagWithEmptyProperties();
        void GetUnknownTagWithUnknownProperties();
        void GetUnknownTagWithKnownProperties();

        void GetKnownTagWithNullProperties();
        void GetKnownTagWithEmptyProperties();
        void GetKnownTagWithUnknownProperties();
        void GetKnownTagWithKnownProperties();

        #endregion GetByTag


        #region GetMediaByTag

        void GetNullFormFileMediaToUnknownTag();
        void GetNullFormFileMediaToKnownTag();

        void GetFormFileWithNoFileMediaToUnknownTag();
        void GetFormFileWithNoFileMediaToKnownTag();

        void GetFormFileMediaToUnknownTag();
        void GetFormFileMediaToKnownTag();


        void GetNullUrlMediaToUnknownTag();
        void GetNullUrlMediaToKnownTag();

        void GetUrlMediaToUnknownTag();
        void GetUrlMediaToKnownTag();

        #endregion GetMediaByTag
    }
}