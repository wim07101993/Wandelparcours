namespace WebService.Models
{
    /// <summary>
    /// EAuthLevel is an indication of to what an user is authorized
    /// </summary>
    public enum EUserType
    {
        SysAdmin,
        Nurse,
        User,
        Module,
        Guest
    }
}