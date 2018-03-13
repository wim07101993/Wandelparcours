namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public interface IResidentsServiceTest
    {
        void GetOneByUnknownTagAndNoPropertiestToInclude();
        void GetOneByUnknownTagAndEmptyPropertiesToInclude();
        void GetOneByUnknownTagAndPropertiesToInclude();

        void GetOneByKnownTagAndNoPropertiestToInclude();
        void GetOneByKnownTagAndEmptyPropertiesToInclude();
        void GetOneByKnownTagAndPropertiesToInclude();


        void AddByteMediaWithUnknownId();
        void AddNullByteMediaWithUnknownId();
        void AddByteMediaWithKnownId();
        void AddNullByteMediaWithKnownId();

        void AddUrlMediaWithUnknownId();
        void AddNullUrlMediaWithUnknownId();
        void AddUrlMediaWithKnownId();
        void AddNullUrlMediaWithKnownId();


        void RemoveKnownMediaWithUnknownId();
        void RemoveKnownMediaWithKnownId();
        void RemoveUnknownMediaWithKnownId();
    }
}