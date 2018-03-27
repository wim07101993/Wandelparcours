namespace WebAPIUnitTests.ControllerTests.ReceiverModulesControllerTests
{
    public interface IReceiverModulesControllerTests
    {
        void CreateNullItem();
        void CreateDuplicate();
        void CreateItem();

        void GetNullMac();
        void GetBadMac();
        void GetNullProperties();
        void GetEmptyProperties();
        void GetBadProperties();
        void Get();

        void DeleteNullMac();
        void DeleteBadMac();
    }
}