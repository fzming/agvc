namespace CoreRepository
{
    public interface IDynamicRepository
    {
        /// <summary>
        ///     动态集合用于联合查询
        /// </summary>
        dynamic DynamicCollection { get; }
    }
}