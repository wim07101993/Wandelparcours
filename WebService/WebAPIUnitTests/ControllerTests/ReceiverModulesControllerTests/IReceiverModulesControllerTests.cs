namespace WebAPIUnitTests.ControllerTests.ReceiverModulesControllerTests
{
    public interface IReceiverModulesControllerTests
    {
        void CreateNullItem();
        void CreateDuplicate();
        void CreateItem();


        // GetAsync(string[]) is not necessary since it just calls the base method


        void GetNullMacNullProperties();
        void GetNullMacEmptyProperties();
        void GetNullMacBadproperties();
        void GetNullMacSomeProeprties();

        void GetBadMacNullProperties();
        void GetBadMacEmptyProperties();
        void GetBadMacBadProperties();
        void GetBadMacSomeProperties();

        void GetExistingMacNullProperties();
        void GetExistingMacEmptyProperties();
        void GetExistingMacBadProperties();
        void GetExistingMacSomeProerties();


        // GetPropertyAsync(string, string) is not necessary since it just calls the base method

        // UpdateAsync(ReceiverModule, string[]) is not necessary since it just calls the base method
        // UpdatePropertyAsync(string, string, string) is not necessary since it just calls the base method

        // DeleteAsync(string) is not necessary since it just calls the base method
    }
}