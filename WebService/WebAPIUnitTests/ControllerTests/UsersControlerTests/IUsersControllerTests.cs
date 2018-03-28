namespace WebAPIUnitTests.ControllerTests.UsersControlerTests
{
    public interface IUsersControllerTests
    {
        void UpdateNullUser();
        void UpdateBadUser();
        void UpdateNullProperties();
        void UpdateEmptyProperties();
        void Update();

        void UpdatePropertyNullId();
        void UpdatePropertyBadId();
        void UpdatePropertyNullPropertyName();
        void UpdatePropertyBadPropertyName();
        void UpdatePropertyBadValue();
        void UpdateProperty();
    }
}
