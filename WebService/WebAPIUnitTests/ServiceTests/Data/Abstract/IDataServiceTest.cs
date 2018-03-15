namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    public interface IDataServiceTest
    {
        void GetAllWithoutPropertiesToInclude();
        void GetAllWithEmptyPropertiesToInclude();
        void GetAllWithSomePropertiesToInclude();


        void GetOneWithUnknownIdAndNullPropertiesToInclude();
        void GetOneWithUnknownIdAndEmptyPropertiesToInclude();
        void GetOneWithUnknownIdAndSomePropertiesToInclude();

        void GetOneWithKnownIdAndNoPropertiesToInclude();
        void GetOneWithKnownIdAndEmptyPropertiesToInclude();
        void GetOneWithKnownIdAndSomePropertiesToInclude();


        void GetPropertyWithUnknownIdAndNoProperty();
        void GetKnownPropertyWithUnknownId();

        void GetNullPropertyWithKnownId();
        void GetPropertyWithKnownId();


        void CreateNullItem();
        void CreateItem();
        void CreateItemWithId();


        void RemoveUnknownItem();
        void RemoveKnownItem();


        void UpdateNullItemAndNoProperties();
        void UpdateNullItemAndEmptyProperties();
        void UpdateNullItemAndSomeProperties();

        void UpdateUnknownItemAndNoProperties();
        void UpdateUnknownItemAndEmptyProperties();
        void UpdateUnKnownItemAndSomeProperties();

        void UpdateKnownItemAndNoProperties();
        void UpdateKnownItemAndEmptyProperties();
        void UpdateKnownItemAndSomeProperties();


        void UpdatePropertyOfUnknownIdAndCorrectValue();
        void UpdateNullPropertyOfUnknownId();
        void UpdatePropertyOfUnknownIdAndIncorrectValue();

        void UpdatePropertyOfKnownIdAndCorrectValue();
        void UpdateNullPropertyOfKnownId();
        void UpdatePropertyOfKnownIdAndIncorrectValue();
    }
}