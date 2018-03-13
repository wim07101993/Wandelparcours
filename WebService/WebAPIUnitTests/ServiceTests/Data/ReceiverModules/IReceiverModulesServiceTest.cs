namespace WebAPIUnitTests.ServiceTests.Data.ReceiverModules
{
    public interface IReceiverModulesServiceTest
    {
        void GetOneWithNullMacAndNoPropertiestToInclude();
        void GetOneWithNullMacAndEmptyPropertiesToInclude();
        void GetOneWithNullMacAndPropertiesToInclude();

        void GetOneWithUnknownMacAndNoPropertiestToInclude();
        void GetOneWithUnknownMacAndEmptyPropertiesToInclude();
        void GetOneWithUnknownMacAndPropertiesToInclude();

        void GetOneWithKnownMacAndNoPropertiestToInclude();
        void GetOneWithKnownMacAndEmptyPropertiesToInclude();
        void GetOneWithKnownMacAndPropertiesToInclude();


        void RemoveNullMac();
        void RemoveUnknownMac();
        void RemoveMac();
    }
}