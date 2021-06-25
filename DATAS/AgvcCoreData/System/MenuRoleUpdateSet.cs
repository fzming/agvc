namespace AgvcCoreData.System
{
    public class MenuRoleUpdateSet
    {
        public string MenuId { get; set; }
        public UpsetType UpsetType { get; set; }
        public string RoleId { get; set; }
    }

    public enum UpsetType
    {
        Add,
        Remove
    }
}