namespace WebService.Models
{
    /// <summary>
    /// EAuthLevel is an indication of to what an user is authorized
    /// </summary>
    public enum EAuthLevel
    {
        SysAdmin = 0,
        Nurse = 1,
        User = 2,
        ReadOnlyUser = 3,
        Guest = 4
    }
}