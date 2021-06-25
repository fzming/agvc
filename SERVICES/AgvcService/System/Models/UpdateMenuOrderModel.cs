namespace AgvcService.System.Models
{
    /// <summary>
    /// 更改菜单排序模型
    /// </summary>
    public class UpdateMenuOrderModel
    {
        public string MenuId { get; set; }
        public OrderDirection Direction { get; set; }
    }
}