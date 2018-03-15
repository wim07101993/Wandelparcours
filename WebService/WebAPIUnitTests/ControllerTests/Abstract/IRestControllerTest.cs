namespace WebAPIUnitTests.ControllerTests.Abstract
{
    public interface IRestControllerTest
    {
        #region PropertySelectors

        void ConvertUnknownStringToSelector();
        void ConvertStringToSelector();

        void ConvertNullStringsToSelectors();
        void ConvertEmptyStringsToSelectors();
        void ConvertUnknownStringsToSelectors();
        void ConvertStringsToSelectorsWithSomeUnknownStrings();
        void ConvertStringsToSelectors();

        #endregion PropertySelectors


        #region Create

        void CreateNull();
        void CreateEmpty();
        void Create();

        #endregion Create


        #region GET

        void GetAllWithoutPropertiesToInclude();
        void GetAllWithEmptyPropertiesToInclude();
        void GetAllWithKnownPropertiesToInclude();
        void GetAllWithUnknownPropertiesToInclude();


        void GetByNullIdAndNoPropertiesToInclude();
        void GetByUnknownIdAndNoPropertiesToInclude();
        void GetByKnownIdAndNoPropertiesToInclude();
        void GetByWrongFormatIdAndNoPropertiesToInclude();

        void GetByNullIdAndEmptyPropertiesToInclude();
        void GetByUnknownIdAndEmptyPropertiesToInclude();
        void GetByKnownIdAndEmptyPropertiesToInclude();
        void GetByWrongFormatIdAndEmptyPropertiesToInclude();

        void GetByNullIdAndKnownPropertiesToInclude();
        void GetByUnknownIdAndKnownPropertiesToInclude();
        void GetByKnownIdAndKnownPropertiesToInclude();
        void GetByWrongFormatIdAndKnownPropertiesToInclude();

        void GetByNullIdAndUnknownPropertiesToInclude();
        void GetByUnknownIdAndUnknownPropertiesToInclude();
        void GetByKnownIdAndUnknownPropertiesToInclude();
        void GetByWrongFormatIdAndUnknownPropertiesToInclude();


        void GetNullPropertyByNullId();
        void GetNullPropertyByUnknownId();
        void GetNullPropertyByKnownId();
        void GetNullPropertyByWrongFormatId();

        void GetUnknownPropertyByNullId();
        void GetUnknownPropertyByUnknownId();
        void GetUnknownPropertyByKnownId();
        void GetUnknownPropertyByWrongFormatId();

        void GetKnownPropertyByNullId();
        void GetKnownPropertyByUnknownId();
        void GetKnownPropertyByKnownId();
        void GetKnownPropertyByWrongFormatId();

        #endregion Get


        #region Update

        void UpdateNullPropertiesOfNullItem();
        void UpdateNullPropertiesOfItem();

        void UpdateEmptyPropertiesOfNullItem();
        void UpdateEmptyPropertiesOfItem();

        void UpdateUnknownPropertiesOfNullItem();
        void UpdateUnknownPropertiesOfItem();

        void UpdateKnownPropertiesOfNullItem();
        void UpdateKnownPropertiesOfItem();


        void UpdateNullPropertyOfNullItem();
        void UpdateNullPropertyOfItem();

        void UpdateUnknownPropertyOfNullItem();
        void UpdateUnknownPropertyOfItem();

        void UpdateKnownPropertyOfNullItem();
        void UpdateKnownPropertyOfItem();

        #endregion Update


        #region Delete

        void DeleteNull();
        void DeleteUnknownId();
        void DeleteKnownId();
        void DeleteWrongFormatId();

        #endregion Delete
    }
}