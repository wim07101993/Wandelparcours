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


        void GetOneNullId();
        void GetOneBadId();
        void GetOneNullProperties();
        void GetOneEmptyProperties();
        void GetOneBadProperties();
        void GetOne();


        void GetNullProperty();
        void GetBadProperty();
        void GetPropertyNullID();
        void GetPropertyBadId();
        void GetProperty();

        #endregion READ


        #region UPDATE

        void UpdateNullItem();
        void UpdateBadItem();
        void UpdateNullProperties();
        void UpdateEmptyProperties();
        void UpdatedBadProperties();
        void Update();

        void UpdatePropertyNullId();
        void UpdatePropertyBadId();
        void UpdateNullProperty();
        void UpdateBadProperty();
        void UpdatePropertyBadValue();
        void UpdateProperty();

        #endregion UPDATE


        #region DELETE

        void DeleteNullId();
        void DeleteBadId();
        void Delete();

        #endregion DELETE
    }
}