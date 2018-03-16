namespace WebAPIUnitTests.ControllerTests.RestControllerBaseTests
{
    public interface IRestControllerTests
    {
        #region CREATE

        void Create();
        void CreateNull();
        void CreateDuplicate();

        #endregion CREATE

        #region READ

        void GetAllNullProperties();
        void GetAllEmptyProperties();
        void GetAllSomeProperties();


        void GetOneNullIdNullProperties();
        void GetOneNullIdEmptyProperties();
        void GetOneNullIdBadProperties();
        void GetOneNullIdSomeProperties();

        void GetOneExistingIdNullProperties();
        void GetOneExistingIdEmptyProperties();
        void GetOneExistingIdBadProperties();
        void GetOneExistingIdSomeProperties();

        void GetOneBadIdNullProperties();
        void GetOneBadIdEmptyProperties();
        void GetOneBadIdBadProperties();
        void GetOneBadIdSomeProperties();


        void GetNullPropertyNullId();
        void GetNullPropertyExistingId();
        void GetNullPropertyBadId();

        void GetBadPropertyNullId();
        void GetBadPropertyExistingId();
        void GetBadPropertyBadId();

        void GetExistingPropertyNullId();
        void GetExistingPropertyExistingId();
        void GetExistingPropertyBadId();

        #endregion READ

        #region UPDATE

        void UpdateNullItemNullProperties();
        void UpdateNullItemEmptyProperties();
        void UpdateNullItemBadProperties();
        void UpdateNullItemSomeProperties();

        void UpdateBadIdNullProperties();
        void UpdateBadIdEmptyProperties();
        void UpdateBadIdBadProperties();
        void UpdateBadIdSomeProperties();

        void UpdateExistingIdNullProperties();
        void UpdateExistingIdEmptyProperties();
        void UpdateExistingIdBadProperties();
        void UpdateExistingIdSomeProperties();


        void UpdateNullIdNullProperty();
        void UpdateNullIdBadProperty();
        void UpdateNullIdExistingPropertyBadValue();
        void UpdateNullIdExistingPropertyGoodValue();

        void UpdateBadIdNullProperty();
        void UpdateBadIdBadProperty();
        void UpdateBadIdExistingPropertyBadValue();
        void UpdateBadIdExistingPropertyGoodValue();

        void UpdateExistingIdNullProperty();
        void UpdateExistingIdBadProperty();
        void UpdateExistingIdExistingPropertyBadValue();
        void UpdateExistingIdExistingPropertyGoodValue();

        #endregion UPDATE


        #region DELETE

        void DeleteNullId();
        void DeleteBadId();
        void DeleteExistingId();

        #endregion DELETE
    }
}