using CoreData;

namespace AgvcCoreData.Users
{
    public class  UserUpdateResult<T>:Result<T>
    {
        public bool RoleChanged { get; set; }
    }
}