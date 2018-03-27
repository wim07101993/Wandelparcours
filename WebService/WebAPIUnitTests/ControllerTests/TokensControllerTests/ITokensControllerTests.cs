namespace WebAPIUnitTests.ControllerTests.TokensControllerTests
{
    public interface ITokensControllerTests
    {
        void CreateTokenNullUserName();
        void CreateTokenBadUsername();
        void CreateTokenNullPassword();
        void CreateTokenBadPassword();
        void CreateToken();
    }
}