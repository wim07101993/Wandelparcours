namespace WebService.Models
{
    /// <summary>
    /// EAuthLevel is an indication of to what an user is authorized
    /// </summary>
    public enum EAuthLevel
    {
        /// <summary>
        /// SysAdmin is the highest level, they are authorized to do anything.
        /// </summary> 
        SysAdmin = 0,

        /// <summary>
        /// Admin is the level of the nurses of the home.
        /// They can ask and manipulate all the data of all the residents and ask for their location-data.
        /// </summary>
        Nurse = 1,

        /// <summary>
        /// User the level of the basic users of the webApp (family of the residents).
        /// They can ask for the data fo the resident they are responsible for and add or remove their media.
        /// </summary>
        User = 2,

        /// <summary>
        /// UnVerifiedUser is a user that has been created but not verified by an admin. They have no rights either.
        /// </summary>
        UnVerifiedUser = 3,

        /// <summary>
        /// Guest is the lowest level. A Guest can only ask for a login.
        /// </summary>
        Guest = 4
    }
}